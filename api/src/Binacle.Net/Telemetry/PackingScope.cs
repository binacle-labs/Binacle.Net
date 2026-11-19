using System.Diagnostics;
using Binacle.Net.Kernel.Logging;
using Binacle.Net.Kernel.Telemetry;

namespace Binacle.Net.Telemetry;

// One packing operation: its span, its duration and its results.
internal sealed class PackingScope : IDisposable
{
	private readonly PackingMetrics metrics;
	private readonly TimedActivityOperation timedOperation;
	private readonly TagList tags;

	private PackingScope(PackingMetrics metrics, TimedActivityOperation timedOperation, TagList tags)
	{
		this.metrics = metrics;
		this.timedOperation = timedOperation;
		this.tags = tags;
	}

	public static PackingScope Start(
		PackingMetrics metrics,
		ILogger logger,
		PackingMode mode,
		Algorithm? algorithm,
		int binCount,
		IReadOnlyList<IWithQuantity> items,
		IOperationParameters parameters
	)
	{
		var quantity = 0;
		foreach (var item in items)
		{
			quantity += item.Quantity;
		}

		var operation = PackingTags.ToTag(parameters.Operation);
		var modeTag = PackingTags.ToTag(mode);
		var tags = new TagList
		{
			{ PackingTags.Operation, operation },
			{ PackingTags.Mode, modeTag },
			{ PackingTags.RequestAlgorithm, algorithm.HasValue ? PackingTags.ToTag(algorithm.Value) : PackingTags.BestAlgorithm }
		};

		var timedOperation = logger.BeginTimedActivityOperation(
			LogLevel.Debug, $"{operation} {modeTag}", Diagnostics.ActivitySource
		);
		if (timedOperation.Activity is { IsAllDataRequested: true } activity)
		{
			foreach (var tag in tags)
			{
				activity.SetTag(tag.Key, tag.Value);
			}

			activity.SetTag(PackingTags.BinCount, binCount);
			activity.SetTag(PackingTags.ItemCount, items.Count);
			activity.SetTag(PackingTags.ItemQuantity, quantity);
		}

		metrics.RecordItems(quantity, operation, modeTag);

		return new PackingScope(metrics, timedOperation, tags);
	}

	public async ValueTask<T> RunAsync<T>(Func<ValueTask<T>> process)
	{
		var start = Stopwatch.GetTimestamp();
		try
		{
			var result = await process();
			this.metrics.RecordDuration(Stopwatch.GetElapsedTime(start), this.tags);
			return result;
		}
		catch (Exception ex)
		{
			var errorTags = this.tags;
			errorTags.Add(TelemetryNames.ErrorType, TelemetryNames.ErrorTypeOf(ex));
			this.metrics.RecordDuration(Stopwatch.GetElapsedTime(start), errorTags);

			this.timedOperation.Activity.RecordError(ex);
			throw;
		}
	}

	public void Record(OperationResult result)
	{
		this.metrics.RecordResult(result, this.tags);

		if (this.timedOperation.Activity is { IsAllDataRequested: true } activity)
		{
			activity.SetTag(PackingTags.ResultAlgorithm, PackingTags.ToTag(result.AlgorithmInfo.Algorithm));
			activity.SetTag(PackingTags.ResultStatus, PackingTags.ToTag(result.Status));
		}
	}

	public void Record(IEnumerable<OperationResult> results)
	{
		foreach (var result in results)
		{
			this.metrics.RecordResult(result, this.tags);
		}
	}

	public void Dispose() => this.timedOperation.Dispose();
}
