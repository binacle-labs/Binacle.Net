using System.Collections.Concurrent;
using System.Diagnostics;
using Binacle.Data;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures the processors send the expected spans")]
public class TracingTests : IDisposable
{
	private readonly ActivitySource testSource = new("Binacle.Lib.UnitTests.Tracing");
	private readonly ActivityListener listener;
	private readonly ConcurrentQueue<Activity> activities = new();
	private ActivityTraceId traceId;

	public TracingTests()
	{
		// Other tests in the run also call the processors, so only spans under this test's root are kept. Until the
		// root starts, traceId is default, which is also the parent trace id of every span with no parent.
		this.listener = new ActivityListener
		{
			ShouldListenTo = source => source.Name == "Binacle.Lib" || source == this.testSource,
			Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
				options.Source == this.testSource
				|| (this.traceId != default && options.Parent.TraceId == this.traceId)
					? ActivitySamplingResult.AllDataAndRecorded
					: ActivitySamplingResult.None,
			ActivityStopped = activity => this.activities.Enqueue(activity)
		};
		ActivitySource.AddActivityListener(this.listener);
	}

	public void Dispose()
	{
		this.listener.Dispose();
		this.testSource.Dispose();
	}

	private static List<ScenarioBin> Bins() =>
	[
		new() { ID = "small", Length = 4, Width = 4, Height = 4 },
		new() { ID = "large", Length = 50, Width = 50, Height = 50 }
	];

	private static List<ScenarioItem> Items() =>
	[
		new() { ID = "a", Length = 5, Width = 5, Height = 5, Quantity = 3 },
		new() { ID = "b", Length = 2, Width = 2, Height = 2, Quantity = 4 }
	];

	[Fact]
	public void A_loop_and_a_parallel_processor_name_and_tag_their_spans()
	{
		var algorithmFactory = new AlgorithmFactory();
		var pack = new TestOperationParameters { Operation = AlgorithmOperation.Packing };
		var fit = new TestOperationParameters { Operation = AlgorithmOperation.Fitting };
		var results = new List<OperationResult>();

		using (var root = this.testSource.StartActivity("test")!)
		{
			this.traceId = root.TraceId;
			results.AddRange(new LoopBinProcessor(algorithmFactory)
				.Process(Algorithm.FFD, Bins(), Items(), fit).Values);
			results.AddRange(new ParallelAlgorithmProcessor([Algorithm.FFD, Algorithm.WFD, Algorithm.BFD], algorithmFactory)
				.Process(Bins()[1], Items(), pack).Values);
		}

		var lib = this.activities
			.Where(x => x.Source.Name == "Binacle.Lib" && x.TraceId == this.traceId)
			.ToList();

		var bins = lib.Where(x => x.DisplayName == "process bins").ShouldHaveSingleItem();
		bins.GetTagItem("binacle.lib.processor").ShouldBe("loop");
		bins.GetTagItem("binacle.packing.operation").ShouldBe("fit");

		var algorithms = lib.Where(x => x.DisplayName == "process algorithms").ShouldHaveSingleItem();
		algorithms.GetTagItem("binacle.lib.processor").ShouldBe("parallel");
		algorithms.GetTagItem("binacle.packing.operation").ShouldBe("pack");

		var built = lib.Where(x => x.DisplayName == "build result").ToList();
		built.Count.ShouldBe(results.Count);
		built.Select(x => (string?)x.GetTagItem("binacle.packing.result.algorithm"))
			.ShouldBe(new[] { "ffd", "ffd", "ffd", "wfd", "bfd" }, ignoreOrder: true);
		built.ShouldContain(x => (string?)x.GetTagItem("binacle.packing.result.status") == "early_exit");
		built.ShouldAllBe(x => x.GetTagItem("binacle.packing.result.status") != null);

		lib.Select(x => x.DisplayName).Distinct()
			.ShouldBe(new[] { "process bins", "process algorithms", "build result" }, ignoreOrder: true);
	}
}
