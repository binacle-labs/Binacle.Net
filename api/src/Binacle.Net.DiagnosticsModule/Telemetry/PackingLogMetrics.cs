using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Binacle.Net.Kernel.Telemetry;

namespace Binacle.Net.DiagnosticsModule.Telemetry;

internal sealed class PackingLogMetrics
{
	public const string ProcessedName = "binacle.packing_log.processed";
	public const string QueueSizeName = "binacle.packing_log.queue.size";
	public const string QueueCapacityName = "binacle.packing_log.queue.capacity";

	public const string QueueFull = "queue_full";
	public const string AlreadyShutdown = "already_shutdown";

	private readonly Meter meter;
	private readonly Counter<long> processed;

	public PackingLogMetrics(IMeterFactory meterFactory)
	{
		this.meter = meterFactory.Create(new MeterOptions(Diagnostics.Name) { Version = Metadata.Version });

		this.processed = this.meter.CreateCounter<long>(
			ProcessedName,
			unit: "{entry}",
			description: "Packing log entries handled: written, dropped because the queue was full, lost because the write failed, or not written because the writer had stopped."
		);
	}

	public void EntryWritten() => this.processed.Add(1);

	public void EntryDropped() => this.processed.Add(1, new KeyValuePair<string, object?>(TelemetryNames.ErrorType, QueueFull));

	public void EntryFailed(Exception exception)
		=> this.processed.Add(1, new KeyValuePair<string, object?>(TelemetryNames.ErrorType, TelemetryNames.ErrorTypeOf(exception)));

	public void EntryDiscarded() => this.processed.Add(1, new KeyValuePair<string, object?>(TelemetryNames.ErrorType, AlreadyShutdown));

	public void ObserveQueue<T>(Channel<T> channel, int? capacity)
	{
		this.meter.CreateObservableUpDownCounter(
			QueueSizeName,
			() => channel.Reader.Count,
			unit: "{entry}",
			description: "Packing log entries waiting to be written."
		);

		if (capacity is int limit)
		{
			this.meter.CreateObservableUpDownCounter(
				QueueCapacityName,
				() => limit,
				unit: "{entry}",
				description: "The most packing log entries the queue holds before it drops new ones."
			);
		}
	}
}
