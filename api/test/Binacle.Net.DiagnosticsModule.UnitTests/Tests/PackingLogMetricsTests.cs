using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.DiagnosticsModule.Telemetry;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// DropWrite reports success to the writer, so without the counter a full queue loses entries unseen.
[Trait("Behavioral Tests", "Ensures the packing log queue reports dropped and queued entries")]
public class PackingLogMetricsTests
{
	[Fact]
	public void A_full_queue_counts_each_dropped_entry_and_reports_its_length()
	{
		var collection = new ServiceCollection()
			.AddMetrics()
			.Configure<PackingLogsConfigurationOptions>(options => options.ChannelLimit = 1);
		collection.AddOptionsBasedPackingLogProcessor();
		using var services = collection.BuildServiceProvider();
		var meterFactory = services.GetRequiredService<IMeterFactory>();

		long dropped = 0;
		long queued = -1;
		using var listener = new MeterListener
		{
			InstrumentPublished = (instrument, l) =>
			{
				if (instrument.Meter.Scope == meterFactory)
				{
					l.EnableMeasurementEvents(instrument);
				}
			}
		};
		listener.SetMeasurementEventCallback<long>((instrument, value, _, _) =>
		{
			if (instrument.Name == PackingLogMetrics.DroppedEntriesName)
			{
				dropped += value;
			}
		});
		listener.SetMeasurementEventCallback<int>((instrument, value, _, _) =>
		{
			if (instrument.Name == PackingLogMetrics.QueuedEntriesName)
			{
				queued = value;
			}
		});
		listener.Start();

		var channel = services.GetRequiredService<Channel<AlgorithmOperationLogChannelRequest>>();
		for (var i = 0; i < 3; i++)
		{
			channel.Writer.TryWrite(new AlgorithmOperationLogChannelRequest
			{
				Bins = [],
				Items = [],
				Results = new Dictionary<string, Binacle.Packing.OperationResult>()
			}).ShouldBeTrue();
		}
		listener.RecordObservableInstruments();

		dropped.ShouldBe(2);
		queued.ShouldBe(1);
	}
}
