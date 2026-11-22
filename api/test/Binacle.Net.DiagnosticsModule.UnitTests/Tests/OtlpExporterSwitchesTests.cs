using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures the OTLP switches decide which signals are sent")]
public class OtlpExporterSwitchesTests : IAsyncLifetime
{
	private readonly ConcurrentBag<string> paths = new();
	private readonly CapturingSink sink = new();
	private WebApplication receiver = null!;
	private string address = null!;

	public async ValueTask InitializeAsync()
	{
		var builder = WebApplication.CreateSlimBuilder();
		builder.WebHost.UseUrls("http://127.0.0.1:0");
		this.receiver = builder.Build();
		this.receiver.MapFallback(context =>
		{
			this.paths.Add(context.Request.Path);
			return Task.CompletedTask;
		});
		await this.receiver.StartAsync();
		this.address = this.receiver.Urls.First();
	}

	public async ValueTask DisposeAsync() => await this.receiver.DisposeAsync();

	[Theory]
	[InlineData("")]
	[InlineData("/smth/")]
	public void All_switches_on_sends_all_three(string basePath)
	{
		this.SendOneOfEach(this.Otlp(basePath));

		var prefix = basePath.TrimEnd('/');
		this.paths.Distinct().ShouldBe(
			[$"{prefix}/v1/metrics", $"{prefix}/v1/traces", $"{prefix}/v1/logs"], ignoreOrder: true);
	}

	[Theory]
	[InlineData("metrics")]
	[InlineData("traces")]
	[InlineData("logs")]
	public void A_signal_switched_off_is_not_sent(string off)
	{
		this.SendOneOfEach(this.Otlp("", metrics: off != "metrics", tracing: off != "traces", logging: off != "logs"));

		string[] expected = ["metrics", "traces", "logs"];
		this.paths.Distinct().ShouldBe(expected.Where(s => s != off).Select(s => $"/v1/{s}"), ignoreOrder: true);
	}

	[Fact]
	public void A_signal_switched_off_keeps_the_path_on_the_others()
	{
		this.SendOneOfEach(this.Otlp("/smth/", tracing: false));

		this.paths.Distinct().ShouldBe(["/smth/v1/metrics", "/smth/v1/logs"], ignoreOrder: true);
	}

	[Fact]
	public void All_switches_off_sends_nothing_and_warns()
	{
		this.SendOneOfEach(this.Otlp("", metrics: false, tracing: false, logging: false));

		this.paths.ShouldBeEmpty();
		this.sink.Warnings.ShouldHaveSingleItem().ShouldStartWith("OTLP is enabled but Metrics, Tracing and Logging are all off");
	}

	private OtlpExporterConfigurationOptions Otlp(
		string basePath, bool metrics = true, bool tracing = true, bool logging = true)
		=> new()
		{
			Enabled = true,
			Endpoint = this.address + basePath,
			Protocol = "http/protobuf",
			Metrics = metrics,
			Tracing = tracing,
			Logging = logging
		};

	private void SendOneOfEach(OtlpExporterConfigurationOptions otlp)
	{
		var name = Guid.NewGuid().ToString();
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddOpenTelemetry()
			.WithMetrics(metrics => metrics.AddMeter(name))
			.WithTracing(tracing => tracing.AddSource(name))
			.WithLogging()
			.WithLogger(new LoggerConfiguration().WriteTo.Sink(this.sink).CreateLogger())
			.UseOtlpExporter(otlp);

		using var provider = services.BuildServiceProvider();
		var tracerProvider = provider.GetRequiredService<TracerProvider>();
		var meterProvider = provider.GetRequiredService<MeterProvider>();
		var loggerProvider = provider.GetRequiredService<LoggerProvider>();

		using var source = new ActivitySource(name);
		using var meter = new Meter(name);
		source.StartActivity("span")?.Dispose();
		meter.CreateCounter<long>("counter").Add(1);
		provider.GetRequiredService<ILoggerFactory>().CreateLogger(name).LogInformation("log");

		tracerProvider.ForceFlush();
		meterProvider.ForceFlush();
		loggerProvider.ForceFlush();
	}
}
