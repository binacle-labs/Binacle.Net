using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.DiagnosticsModule.Telemetry;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Telemetry;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// DropWrite reports success to the writer, so without the counter a full queue loses entries unseen.
[Trait("Behavioral Tests", "Ensures the packing log queue reports processed entries, its size and its capacity")]
public class PackingLogMetricsTests
{
	private sealed class Recorded : IDisposable
	{
		private readonly MeterListener listener;

		// Keyed by error.type, or "written" when it has none.
		public Dictionary<string, long> Processed { get; } = new();
		public Dictionary<string, int> Observed { get; } = new();

		public Recorded(IMeterFactory meterFactory)
		{
			this.listener = new MeterListener
			{
				InstrumentPublished = (instrument, l) =>
				{
					if (instrument.Meter.Scope == meterFactory)
					{
						l.EnableMeasurementEvents(instrument);
					}
				}
			};
			this.listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
			{
				if (instrument.Name != PackingLogMetrics.ProcessedName)
				{
					return;
				}

				var errorType = "written";
				foreach (var tag in tags)
				{
					if (tag.Key == TelemetryNames.ErrorType)
					{
						errorType = (string)tag.Value!;
					}
				}

				this.Processed[errorType] = this.Processed.GetValueOrDefault(errorType) + value;
			});
			this.listener.SetMeasurementEventCallback<int>((instrument, value, _, _) =>
				this.Observed[instrument.Name] = value);
			this.listener.Start();
		}

		public void Observe() => this.listener.RecordObservableInstruments();

		public void Dispose() => this.listener.Dispose();
	}

	private static ServiceProvider Services(int? channelLimit)
	{
		var collection = new ServiceCollection()
			.AddMetrics()
			.Configure<PackingLogsConfigurationOptions>(options => options.ChannelLimit = channelLimit);
		collection.AddOptionsBasedPackingLogProcessor();
		return collection.BuildServiceProvider();
	}

	private static AlgorithmOperationLogChannelRequest Request() => new()
	{
		Bins = [],
		Items = [],
		Results = new Dictionary<string, Binacle.Packing.OperationResult>()
	};

	[Fact]
	public void A_written_entry_counts_with_no_error_type()
	{
		using var services = Services(channelLimit: 1);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		var options = services.GetRequiredService<LogsProcessorOptions<AlgorithmOperationLogChannelRequest>>();

		options.OnWritten!();

		recorded.Processed.ShouldBe(new Dictionary<string, long> { ["written"] = 1 });
	}

	[Fact]
	public void A_full_queue_counts_each_dropped_entry_as_queue_full()
	{
		using var services = Services(channelLimit: 1);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());

		var channel = services.GetRequiredService<Channel<AlgorithmOperationLogChannelRequest>>();
		for (var i = 0; i < 3; i++)
		{
			channel.Writer.TryWrite(Request()).ShouldBeTrue();
		}
		recorded.Observe();

		recorded.Processed.ShouldBe(new Dictionary<string, long> { [PackingLogMetrics.QueueFull] = 2 });
		recorded.Observed[PackingLogMetrics.QueueSizeName].ShouldBe(1);
	}

	[Fact]
	public void A_write_that_throws_counts_its_exception_type()
	{
		using var services = Services(channelLimit: 1);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		var options = services.GetRequiredService<LogsProcessorOptions<AlgorithmOperationLogChannelRequest>>();

		options.OnWriteFailed!(new IOException());

		recorded.Processed.ShouldBe(new Dictionary<string, long> { ["System.IO.IOException"] = 1 });
	}

	[Fact]
	public void An_entry_left_when_the_processor_stops_counts_as_already_shutdown()
	{
		using var services = Services(channelLimit: 1);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		var options = services.GetRequiredService<LogsProcessorOptions<AlgorithmOperationLogChannelRequest>>();

		options.OnDiscarded!();

		recorded.Processed.ShouldBe(new Dictionary<string, long> { [PackingLogMetrics.AlreadyShutdown] = 1 });
	}

	[Theory]
	[InlineData(1)]
	[InlineData(0)]
	public void A_write_after_the_processor_stops_counts_as_already_shutdown(int channelLimit)
	{
		using var services = Services(channelLimit);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		var channel = services.GetRequiredService<Channel<AlgorithmOperationLogChannelRequest>>();
		channel.Writer.TryComplete();

		channel.Writer.TryWrite(Request()).ShouldBeFalse();

		recorded.Processed.ShouldBe(new Dictionary<string, long> { [PackingLogMetrics.AlreadyShutdown] = 1 });
	}

	[Fact]
	public void A_bounded_queue_publishes_its_capacity()
	{
		using var services = Services(channelLimit: 5);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		services.GetRequiredService<Channel<AlgorithmOperationLogChannelRequest>>();
		recorded.Observe();

		recorded.Observed[PackingLogMetrics.QueueCapacityName].ShouldBe(5);
	}

	[Fact]
	public void An_unbounded_queue_publishes_no_capacity()
	{
		using var services = Services(channelLimit: 0);
		using var recorded = new Recorded(services.GetRequiredService<IMeterFactory>());
		services.GetRequiredService<Channel<AlgorithmOperationLogChannelRequest>>();
		recorded.Observe();

		recorded.Observed.ShouldContainKey(PackingLogMetrics.QueueSizeName);
		recorded.Observed.ShouldNotContainKey(PackingLogMetrics.QueueCapacityName);
	}
}
