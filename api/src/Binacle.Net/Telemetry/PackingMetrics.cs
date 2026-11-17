using System.Diagnostics;
using System.Diagnostics.Metrics;
using Binacle.Net.Kernel.Telemetry;

namespace Binacle.Net.Telemetry;

internal sealed class PackingMetrics
{
	public const string DurationName = "binacle.packing.operation.duration";
	public const string SizeName = "binacle.packing.operation.size";
	public const string ResultsName = "binacle.packing.results";

	private readonly Histogram<double> duration;
	private readonly Histogram<int> size;
	private readonly Counter<long> results;

	public PackingMetrics(IMeterFactory meterFactory)
	{
		var meter = meterFactory.Create(Diagnostics.Name);

		this.duration = meter.CreateHistogram(
			DurationName,
			unit: "s",
			description: "Time spent packing in one operation, without the request around it.",
			advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = HistogramBuckets.Seconds }
		);

		this.size = meter.CreateHistogram(
			SizeName,
			unit: "{item}",
			description: "Items in one operation, counting each unit of quantity.",
			advice: new InstrumentAdvice<int>
			{
				HistogramBucketBoundaries = [1, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000]
			}
		);

		this.results = meter.CreateCounter<long>(
			ResultsName,
			unit: "{result}",
			description: "Results returned, by the algorithm that produced them and their status."
		);
	}

	public void RecordSize(int quantity, string operation, string mode)
	{
		var tags = new TagList
		{
			{ PackingTags.Operation, operation },
			{ PackingTags.Mode, mode }
		};
		this.size.Record(quantity, tags);
	}

	public void RecordDuration(TimeSpan elapsed, in TagList tags) => this.duration.Record(elapsed.TotalSeconds, tags);

	public void RecordResult(OperationResult result, TagList tags)
	{
		tags.Add(PackingTags.ResultAlgorithm, PackingTags.ToTag(result.AlgorithmInfo.Algorithm));
		tags.Add(PackingTags.ResultStatus, PackingTags.ToTag(result.Status));
		this.results.Add(1, tags);
	}
}
