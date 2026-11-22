using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.IntegrationTests.Telemetry;

// Records the tags of every measurement one instrument makes in one host.
internal sealed class InstrumentRecorder : IDisposable
{
	private readonly MeterListener listener = new();
	private readonly ConcurrentQueue<IReadOnlyDictionary<string, object?>> measurements = new();

	public InstrumentRecorder(IServiceProvider services, string meterName, string instrumentName)
	{
		var meterFactory = services.GetRequiredService<IMeterFactory>();

		this.listener.InstrumentPublished = (instrument, listener) =>
		{
			// Every host in the run publishes the same instruments. The meter's scope is the host's factory.
			if (instrument.Meter.Scope == meterFactory
			    && instrument.Meter.Name == meterName
			    && instrument.Name == instrumentName)
			{
				listener.EnableMeasurementEvents(instrument);
			}
		};
		this.listener.SetMeasurementEventCallback<long>(this.Record);
		this.listener.SetMeasurementEventCallback<double>(this.Record);
		this.listener.Start();
	}

	public IReadOnlyCollection<IReadOnlyDictionary<string, object?>> Measurements => this.measurements.ToArray();

	private void Record<T>(
		Instrument instrument,
		T measurement,
		ReadOnlySpan<KeyValuePair<string, object?>> tags,
		object? state
	)
		where T : struct
	{
		var copy = new Dictionary<string, object?>();
		foreach (var tag in tags)
		{
			copy[tag.Key] = tag.Value;
		}

		this.measurements.Enqueue(copy);
	}

	public void Dispose() => this.listener.Dispose();
}
