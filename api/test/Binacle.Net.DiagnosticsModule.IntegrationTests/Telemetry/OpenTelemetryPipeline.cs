using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Binacle.Net.DiagnosticsModule.IntegrationTests.Telemetry;

[Trait("Behavioral Tests", "Ensures the OpenTelemetry pipeline exports the resource, spans, metrics and logs it should")]
public class OpenTelemetryPipeline
{
	private const string PackRoute = "/api/v4/pack/bin";
	private const string FitRoute = "/api/v4/fit/bin";

	private static readonly object SampleRequest = new
	{
		Parameters = new { Algorithm = "Best" },
		Bin = new { ID = "custom_bin", Length = 30, Width = 40, Height = 60 },
		Items = new[]
		{
			new { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
		}
	};

	private readonly BinacleApi sut;

	public OpenTelemetryPipeline(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Fact]
	public void Every_provider_has_the_resource()
	{
		BaseProvider[] providers =
		[
			this.sut.Services.GetRequiredService<TracerProvider>(),
			this.sut.Services.GetRequiredService<MeterProvider>(),
			this.sut.Services.GetRequiredService<LoggerProvider>(),
		];

		foreach (var provider in providers)
		{
			var attributes = provider.GetResource().Attributes.ToDictionary();

			attributes["service.name"].ShouldBe("Binacle.Net");
			attributes["service.namespace"].ShouldBe("binacle");
			attributes["service.instance.id"].ShouldBeOfType<string>().ShouldNotBeEmpty();
			attributes["host.name"].ShouldBe(Environment.MachineName);
			Convert.ToInt64(attributes["process.pid"]).ShouldBe(Environment.ProcessId);
			attributes["deployment.environment.name"].ShouldBe("test");
		}
	}

	[Fact]
	public async Task A_health_check_makes_no_span_and_no_request_metric()
	{
		var health = await this.SendAsync(HttpMethod.Get, BinacleApi.HealthCheckPath);
		health.Response.StatusCode.ShouldBe(HttpStatusCode.OK);

		// The pack request proves spans and the request metric are recorded at all.
		var pack = await this.SendAsync(HttpMethod.Post, PackRoute, SampleRequest);
		await this.WaitForServerSpan(pack.TraceId);
		this.sut.FlushMetrics();

		this.SpansOf(health.TraceId).ShouldBeEmpty();

		var routes = this.LatestPoints("http.server.request.duration")
			.Select(point => Tag(point, "http.route"))
			.ToArray();
		routes.ShouldNotBeEmpty();
		routes.ShouldNotContain(BinacleApi.HealthCheckPath);
	}

	[Theory]
	[InlineData(PackRoute, "pack single_bin")]
	[InlineData(FitRoute, "fit single_bin")]
	public async Task A_packing_request_makes_the_span_tree(string route, string operationSpan)
	{
		var request = await this.SendAsync(HttpMethod.Post, route, SampleRequest);
		request.Response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var server = await this.WaitForServerSpan(request.TraceId);

		var binacleSpans = this.SpansOf(request.TraceId)
			.Where(span => span.Source.Name == "Binacle.Net")
			.ToArray();

		binacleSpans.Select(span => span.DisplayName)
			.ShouldBe([operationSpan, "create response", "queue packing_log"], ignoreOrder: true);
		binacleSpans.ShouldAllBe(span => span.ParentSpanId == server.SpanId);
	}

	[Fact]
	public async Task Fit_and_pack_record_the_binacle_metrics()
	{
		await this.SendAsync(HttpMethod.Post, PackRoute, SampleRequest);
		await this.SendAsync(HttpMethod.Post, FitRoute, SampleRequest);

		// The packing log is written in the background.
		await WaitUntil(() =>
		{
			this.sut.FlushMetrics();
			return this.LatestPoints("binacle.packing_log.processed").Length > 0;
		});

		var expected = new Dictionary<string, string>
		{
			["binacle.packing.operation.duration"] = "s",
			["binacle.packing.operation.items"] = "{item}",
			["binacle.packing.results"] = "{result}",
			["binacle.packing_log.processed"] = "{entry}",
			["binacle.packing_log.queue.size"] = "{entry}",
			["binacle.packing_log.queue.capacity"] = "{entry}",
		};

		var exported = this.sut.Metrics.Snapshot()
			.Where(metric => metric.Name.StartsWith("binacle."))
			.GroupBy(metric => metric.Name)
			.ToDictionary(group => group.Key, group => group.Last().Unit);

		exported.ShouldBe(expected, ignoreOrder: true);

		this.LatestPoints("binacle.packing.operation.duration")
			.Select(point => Tag(point, "binacle.packing.operation"))
			.ShouldBe(["fit", "pack"], ignoreOrder: true);
	}

	[Fact]
	public async Task Every_binacle_scope_carries_the_service_version()
	{
		var request = await this.SendAsync(HttpMethod.Post, PackRoute, SampleRequest);
		await this.WaitForServerSpan(request.TraceId);
		this.sut.FlushMetrics();

		var serviceVersion = this.sut.Services.GetRequiredService<TracerProvider>()
			.GetResource().Attributes.ToDictionary()["service.version"].ShouldBeOfType<string>();
		serviceVersion.ShouldNotBeEmpty();

		var sources = this.SpansOf(request.TraceId)
			.Where(span => span.Source.Name.StartsWith("Binacle."))
			.Select(span => (span.Source.Name, span.Source.Version))
			.Distinct();
		sources.ShouldBe([("Binacle.Net", serviceVersion)]);

		var meters = this.sut.Metrics.Snapshot()
			.Where(metric => metric.MeterName.StartsWith("Binacle."))
			.Select(metric => (metric.MeterName, metric.MeterVersion))
			.Distinct();
		meters.ShouldBe(
			[("Binacle.Net", serviceVersion), ("Binacle.Net.DiagnosticsModule", serviceVersion)],
			ignoreOrder: true
		);
	}

	[Fact]
	public async Task A_binacle_log_record_carries_the_request_trace_id()
	{
		var request = await this.SendAsync(HttpMethod.Post, PackRoute, SampleRequest);
		await this.WaitForServerSpan(request.TraceId);

		this.sut.Logs.Snapshot().ShouldContain(log =>
			log.TraceId == request.TraceId && log.CategoryName!.StartsWith("Binacle.Net"));
	}

	private async Task<(HttpResponseMessage Response, ActivityTraceId TraceId)> SendAsync(
		HttpMethod method,
		string path,
		object? body = null
	)
	{
		var traceId = ActivityTraceId.CreateRandom();
		var message = new HttpRequestMessage(method, path);
		message.Headers.Add("traceparent", $"00-{traceId}-{ActivitySpanId.CreateRandom()}-01");
		if (body is not null)
		{
			message.Content = JsonContent.Create(body);
		}

		var response = await this.sut.Client.SendAsync(message, TestContext.Current.CancellationToken);
		return (response, traceId);
	}

	private Activity[] SpansOf(ActivityTraceId traceId)
		=> this.sut.Spans.Snapshot().Where(span => span.TraceId == traceId).ToArray();

	// The server span ends after the client has the response.
	private async Task<Activity> WaitForServerSpan(ActivityTraceId traceId)
	{
		Activity? server = null;
		await WaitUntil(() =>
		{
			server = this.SpansOf(traceId).FirstOrDefault(span => span.Kind == ActivityKind.Server);
			return server is not null;
		});
		return server!;
	}

	private MetricPoint[] LatestPoints(string name)
	{
		var metric = this.sut.Metrics.Snapshot().LastOrDefault(metric => metric.Name == name);
		return metric?.MetricPoints.ToArray() ?? [];
	}

	private static object? Tag(MetricPoint point, string key)
	{
		foreach (var tag in point.Tags)
		{
			if (tag.Key == key)
			{
				return tag.Value;
			}
		}

		return null;
	}

	private static async Task WaitUntil(Func<bool> condition)
	{
		var timeout = TimeSpan.FromSeconds(10);
		var stopwatch = Stopwatch.StartNew();
		while (!condition())
		{
			stopwatch.Elapsed.ShouldBeLessThan(timeout, "The telemetry never arrived.");
			await Task.Delay(50, TestContext.Current.CancellationToken);
		}
	}
}
