using System.Diagnostics.Metrics;
using System.Threading.Channels;

namespace Binacle.Net.DiagnosticsModule.Telemetry;

internal sealed class PackingLogMetrics
{
	public const string DroppedEntriesName = "binacle.packing_log.dropped_entries";
	public const string QueuedEntriesName = "binacle.packing_log.queued_entries";

	private readonly Meter meter;
	private readonly Counter<long> dropped;

	public PackingLogMetrics(IMeterFactory meterFactory)
	{
		this.meter = meterFactory.Create(Diagnostics.Name);

		this.dropped = this.meter.CreateCounter<long>(
			DroppedEntriesName,
			unit: "{entry}",
			description: "Packing log entries thrown away because the queue was full."
		);
	}

	public void EntryDropped() => this.dropped.Add(1);

	public void ObserveQueue<T>(Channel<T> channel)
	{
		this.meter.CreateObservableUpDownCounter(
			QueuedEntriesName,
			() => channel.Reader.Count,
			unit: "{entry}",
			description: "Packing log entries waiting to be written."
		);
	}
}
