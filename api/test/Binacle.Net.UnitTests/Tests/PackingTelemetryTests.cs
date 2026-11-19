using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Telemetry;
using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.Services;
using Binacle.Net.Telemetry;
using Binacle.Net.v4.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Binacle.Net.UnitTests;

[Trait("Behavioral Tests", "Ensures packing operations send the expected spans and metrics")]
public class PackingTelemetryTests : IDisposable
{
	private readonly ServiceProvider services;
	private readonly IMeterFactory meterFactory;
	private readonly ActivityListener activityListener;
	private readonly MeterListener meterListener;
	private readonly ConcurrentQueue<Activity> activities = new();
	private readonly ConcurrentQueue<Meter> meters = new();
	private readonly ConcurrentQueue<(string Name, double Value, Dictionary<string, object?> Tags)> measurements = new();

	public PackingTelemetryTests()
	{
		this.services = new ServiceCollection()
			.AddLogging()
			.AddMetrics()
			.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>))
			.AddSingleton(Channel.CreateUnbounded<AlgorithmOperationLogChannelRequest>())
			.AddBinacleServices()
			.BuildServiceProvider();
		this.meterFactory = this.services.GetRequiredService<IMeterFactory>();

		// Other tests in the run share the "Binacle.Net" source, so only spans started on this thread are kept.
		var threadId = Environment.CurrentManagedThreadId;
		this.activityListener = new ActivityListener
		{
			ShouldListenTo = source => source.Name == Diagnostics.Name,
			Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
				Environment.CurrentManagedThreadId == threadId
					? ActivitySamplingResult.AllDataAndRecorded
					: ActivitySamplingResult.None,
			ActivityStopped = activity => this.activities.Enqueue(activity)
		};
		ActivitySource.AddActivityListener(this.activityListener);

		this.meterListener = new MeterListener
		{
			InstrumentPublished = (instrument, listener) =>
			{
				if (instrument.Meter.Scope == this.meterFactory)
				{
					this.meters.Enqueue(instrument.Meter);
					listener.EnableMeasurementEvents(instrument);
				}
			}
		};
		this.meterListener.SetMeasurementEventCallback<double>(this.OnMeasurement);
		this.meterListener.SetMeasurementEventCallback<int>((i, v, t, s) => this.OnMeasurement(i, v, t, s));
		this.meterListener.SetMeasurementEventCallback<long>((i, v, t, s) => this.OnMeasurement(i, v, t, s));
		this.meterListener.Start();
	}

	public void Dispose()
	{
		this.meterListener.Dispose();
		this.activityListener.Dispose();
		this.services.Dispose();
	}

	private void OnMeasurement(
		Instrument instrument,
		double value,
		ReadOnlySpan<KeyValuePair<string, object?>> tags,
		object? state
	)
	{
		var copy = new Dictionary<string, object?>();
		foreach (var tag in tags)
		{
			copy[tag.Key] = tag.Value;
		}
		this.measurements.Enqueue((instrument.Name, value, copy));
	}

	private IReadOnlyList<(string Name, double Value, Dictionary<string, object?> Tags)> Measured(string name)
		=> this.measurements.Where(x => x.Name == name).ToList();

	private static List<Bin> Bins() =>
	[
		new() { ID = "small", Length = 10, Width = 10, Height = 10 },
		new() { ID = "large", Length = 50, Width = 50, Height = 50 }
	];

	private static List<Box> Items() =>
	[
		Box.From("a", 5, 5, 5, 3),
		Box.From("b", 2, 2, 2, 4)
	];

	private static OperationParameters PackParameters() =>
		new OperationParameters { Algorithm = null }.ForPackingOperation();

	private PackingScope Start() => PackingScope.Start(
		this.services.GetRequiredService<PackingMetrics>(),
		NullLogger.Instance, PackingMode.SingleBin, null, 1, Items(), PackParameters()
	);

	private static async Task Run(
		IBinacleService service, string mode, bool withAlgorithm, OperationParameters parameters
	)
	{
		var cancellationToken = TestContext.Current.CancellationToken;
		var algorithm = Binacle.Packing.Algorithm.FFD;

		switch (mode, withAlgorithm)
		{
			case ("single_bin", true):
				await service.SingleBinAsync(algorithm, Bins()[1], Items(), parameters, cancellationToken);
				break;
			case ("single_bin", false):
				await service.SingleBinAsync(Bins()[1], Items(), parameters, cancellationToken);
				break;
			case ("multiple_bins", true):
				await service.MultipleBinsAsync(algorithm, Bins(), Items(), parameters, cancellationToken);
				break;
			case ("multiple_bins", false):
				await service.MultipleBinsAsync(Bins(), Items(), parameters, cancellationToken);
				break;
			case ("smallest_bin", true):
				await service.SmallestBinAsync(algorithm, Bins(), Items(), parameters, cancellationToken);
				break;
			case ("smallest_bin", false):
				await service.SmallestBinAsync(Bins(), Items(), parameters, cancellationToken);
				break;
			case ("best_bin", true):
				await service.BestBinAsync(algorithm, Bins(), Items(), parameters, cancellationToken);
				break;
			case ("best_bin", false):
				await service.BestBinAsync(Bins(), Items(), parameters, cancellationToken);
				break;
		}
	}

	[Theory]
	[InlineData("single_bin", true)]
	[InlineData("single_bin", false)]
	[InlineData("multiple_bins", true)]
	[InlineData("multiple_bins", false)]
	[InlineData("smallest_bin", true)]
	[InlineData("smallest_bin", false)]
	[InlineData("best_bin", true)]
	[InlineData("best_bin", false)]
	public async Task Each_operation_names_its_span_operation_then_mode(string mode, bool withAlgorithm)
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		await Run(service, mode, withAlgorithm, PackParameters());

		var span = this.activities.Where(x => (string?)x.GetTagItem(PackingTags.Mode) == mode).ShouldHaveSingleItem();
		span.DisplayName.ShouldBe($"pack {mode}");
	}

	[Fact]
	public async Task A_fit_operation_names_its_span_fit()
	{
		var service = this.services.GetRequiredService<IBinacleService>();
		var parameters = new OperationParameters { Algorithm = null }.ForFittingOperation();

		await Run(service, "single_bin", false, parameters);

		var span = this.activities.Single(x => (string?)x.GetTagItem(PackingTags.Mode) == "single_bin");
		span.DisplayName.ShouldBe("fit single_bin");
	}

	[Fact]
	public async Task Best_bin_tags_its_span_with_the_request_and_the_result()
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		var result = await service.BestBinAsync(Bins(), Items(), PackParameters(), TestContext.Current.CancellationToken);

		var span = this.activities.Single(x => x.DisplayName == "pack best_bin");
		span.GetTagItem(PackingTags.Operation).ShouldBe("pack");
		span.GetTagItem(PackingTags.Mode).ShouldBe("best_bin");
		span.GetTagItem(PackingTags.RequestAlgorithm).ShouldBe("best");
		span.GetTagItem(PackingTags.BinCount).ShouldBe(2);
		span.GetTagItem(PackingTags.ItemCount).ShouldBe(2);
		span.GetTagItem(PackingTags.ItemQuantity).ShouldBe(7);
		span.GetTagItem(PackingTags.ResultAlgorithm).ShouldBe(PackingTags.ToTag(result.AlgorithmInfo.Algorithm));
		span.GetTagItem(PackingTags.ResultStatus).ShouldBe(PackingTags.ToTag(result.Status));
		span.Status.ShouldBe(ActivityStatusCode.Unset);
	}

	[Fact]
	public async Task Best_bin_records_duration_items_and_one_result()
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		var result = await service.BestBinAsync(Bins(), Items(), PackParameters(), TestContext.Current.CancellationToken);

		var duration = this.Measured(PackingMetrics.DurationName).ShouldHaveSingleItem();
		duration.Value.ShouldBeGreaterThanOrEqualTo(0);
		duration.Tags.ShouldBe(new Dictionary<string, object?>
		{
			[PackingTags.Operation] = "pack",
			[PackingTags.Mode] = "best_bin",
			[PackingTags.RequestAlgorithm] = "best"
		}, ignoreOrder: true);

		var items = this.Measured(PackingMetrics.ItemsName).ShouldHaveSingleItem();
		items.Value.ShouldBe(7);
		items.Tags.Keys.ShouldBe(new[] { PackingTags.Operation, PackingTags.Mode }, ignoreOrder: true);
		this.measurements.ShouldNotContain(x => x.Name == "binacle.packing.operation.size");

		var counted = this.Measured(PackingMetrics.ResultsName).ShouldHaveSingleItem();
		counted.Value.ShouldBe(1);
		counted.Tags[PackingTags.ResultAlgorithm].ShouldBe(PackingTags.ToTag(result.AlgorithmInfo.Algorithm));
		counted.Tags[PackingTags.ResultStatus].ShouldBe(PackingTags.ToTag(result.Status));
	}

	[Fact]
	public async Task The_packing_meter_and_source_carry_the_app_version()
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		await service.BestBinAsync(Bins(), Items(), PackParameters(), TestContext.Current.CancellationToken);

		var span = this.activities.Single(x => x.DisplayName == "pack best_bin");
		span.Source.Version.ShouldNotBeNullOrEmpty();
		span.Source.Version.ShouldBe(Metadata.Version);

		var meter = this.meters.Where(x => x.Name == Diagnostics.Name).Distinct().ShouldHaveSingleItem();
		meter.Version.ShouldBe(Metadata.Version);
	}

	[Fact]
	public async Task Multiple_bins_counts_every_result_returned()
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		var results = await service.MultipleBinsAsync(
			Binacle.Packing.Algorithm.FFD, Bins(), Items(), PackParameters(), TestContext.Current.CancellationToken
		);

		var counted = this.Measured(PackingMetrics.ResultsName);
		counted.Count.ShouldBe(results.Count);
		counted.ShouldAllBe(x => (string?)x.Tags[PackingTags.RequestAlgorithm] == "ffd");
		counted.ShouldAllBe(x => (string?)x.Tags[PackingTags.ResultAlgorithm] == "ffd");
	}

	[Theory]
	[InlineData("single_bin")]
	[InlineData("multiple_bins")]
	[InlineData("smallest_bin")]
	[InlineData("best_bin")]
	public async Task With_no_algorithm_given_the_request_algorithm_is_best(string mode)
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		await Run(service, mode, false, PackParameters());

		var span = this.activities.Single(x => (string?)x.GetTagItem(PackingTags.Mode) == mode);
		span.GetTagItem(PackingTags.RequestAlgorithm).ShouldBe("best");

		var duration = this.Measured(PackingMetrics.DurationName).ShouldHaveSingleItem();
		duration.Tags[PackingTags.RequestAlgorithm].ShouldBe("best");

		var counted = this.Measured(PackingMetrics.ResultsName);
		counted.ShouldNotBeEmpty();
		counted.ShouldAllBe(x => (string?)x.Tags[PackingTags.RequestAlgorithm] == "best");
	}

	[Fact]
	public async Task The_packing_log_write_is_outside_the_operation_span()
	{
		var service = this.services.GetRequiredService<IBinacleService>();

		await service.BestBinAsync(Bins(), Items(), PackParameters(), TestContext.Current.CancellationToken);

		var operation = this.activities.Single(x => x.DisplayName == "pack best_bin");
		var channel = this.activities.Single(x => x.DisplayName == "queue packing_log");
		channel.ParentSpanId.ShouldNotBe(operation.SpanId);
	}

	[Fact]
	public async Task A_failed_operation_marks_the_span_and_the_duration_with_the_same_error_type()
	{
		using (var scope = this.Start())
		{
			await Should.ThrowAsync<InvalidOperationException>(async () =>
				await scope.RunAsync<int>(() => throw new InvalidOperationException("boom"))
			);
		}

		var span = this.activities.Single(x => x.DisplayName == "pack single_bin");
		span.Status.ShouldBe(ActivityStatusCode.Error);
		span.StatusDescription.ShouldBe("boom");
		span.Events.ShouldContain(x => x.Name == "exception");

		var duration = this.Measured(PackingMetrics.DurationName).ShouldHaveSingleItem();
		duration.Tags[TelemetryNames.ErrorType].ShouldBe(typeof(InvalidOperationException).FullName);
		span.GetTagItem(TelemetryNames.ErrorType).ShouldBe(duration.Tags[TelemetryNames.ErrorType]);
	}

	[Fact]
	public async Task A_packing_operation_logs_its_timed_line_at_debug()
	{
		var logger = new CapturingLogger();
		using (var scope = PackingScope.Start(
			       this.services.GetRequiredService<PackingMetrics>(),
			       logger, PackingMode.SingleBin, null, 1, Items(), PackParameters()
		       ))
		{
			await scope.RunAsync(() => ValueTask.FromResult(0));
		}

		var entry = logger.Entries.ShouldHaveSingleItem();
		entry.Level.ShouldBe(LogLevel.Debug);
		entry.Message.ShouldStartWith("pack single_bin completed in ");
	}

	// The SDK's default buckets top out near 10000, sized for milliseconds. A value in seconds would land in
	// the first bucket every time.
	[Fact]
	public async Task The_sdk_uses_the_second_based_buckets_for_duration()
	{
		var exported = new List<Metric>();
		using (var provider = Sdk.CreateMeterProviderBuilder()
			       .AddMeter(Diagnostics.Name)
			       .AddInMemoryLikeExporter(exported)
			       .Build())
		{
			using (var scope = this.Start())
			{
				await scope.RunAsync(() => ValueTask.FromResult(0));
			}

			provider.ForceFlush();
		}

		var metric = exported.First(x => x.Name == PackingMetrics.DurationName);
		var bounds = new List<double>();
		foreach (ref readonly var point in metric.GetMetricPoints())
		{
			foreach (var bucket in point.GetHistogramBuckets())
			{
				bounds.Add(bucket.ExplicitBound);
			}
			break;
		}

		bounds[0].ShouldBe(0.001);
		bounds.ShouldContain(10);
	}
}

file sealed class CapturingLogger : ILogger
{
	public List<(LogLevel Level, string Message)> Entries { get; } = new();

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(
		LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter
	) => this.Entries.Add((logLevel, formatter(state, exception)));
}

file static class MeterProviderBuilderTestExtensions
{
	public static MeterProviderBuilder AddInMemoryLikeExporter(this MeterProviderBuilder builder, List<Metric> exported)
		=> builder.AddReader(new BaseExportingMetricReader(new CollectingExporter(exported))
		{
			TemporalityPreference = MetricReaderTemporalityPreference.Cumulative
		});

	private sealed class CollectingExporter(List<Metric> exported) : BaseExporter<Metric>
	{
		public override ExportResult Export(in Batch<Metric> batch)
		{
			foreach (var metric in batch)
			{
				exported.Add(metric);
			}
			return ExportResult.Success;
		}
	}
}
