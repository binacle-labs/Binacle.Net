using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.Benchmarks.ParallelOverhead;

// The three race rows and their shared setup. `Bytes` is how much memory each fake algorithm walks: 0 is the
// pure machinery cost, above 0 stands in for a real algorithm's working set.
[MemoryDiagnoser]
public abstract class FakeRaceBase
{
	private IAlgorithmProcessor loop = null!;
	private IAlgorithmProcessor parallelOneThread = null!;
	private IAlgorithmProcessor parallel = null!;
	private ScenarioBin bin = null!;
	private List<ScenarioItem> items = null!;

	// The algorithms the race is given. They never pack; only how many there are matters.
	protected abstract Algorithm[] Algorithms { get; }

	[Params(0, 65_536)]
	public int Bytes { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		JobsByCoreCount.SetCoreCountAndCheck();

		// The smallest case there is; the processors only carry it.
		this.bin = IdenticalCase.Bin();
		this.items = IdenticalCase.Items(1);

		// One real result for every fake algorithm to hand back, because OperationResult cannot be built
		// outside Binacle.Lib.
		var realAlgorithm = AlgorithmFactories.FFD_v2(this.bin, this.items);
		var oneResult = realAlgorithm.Execute(Parameters());

		var factory = new FakeAlgorithmFactory(oneResult, this.Bytes);
		this.loop = new LoopAlgorithmProcessor(this.Algorithms, factory);
		// -1 lifts the processor's default cap, min(work, cores). This project measures what a dispatch costs,
		// so it needs the uncapped path or there is nothing left to measure at 1 CPU.
		this.parallel = new ParallelAlgorithmProcessor(this.Algorithms, factory, maxDegreeOfParallelism: -1);
		this.parallelOneThread = new ParallelAlgorithmProcessor(this.Algorithms, factory, maxDegreeOfParallelism: 1);

		var check = this.Run(this.parallel);
		if (check.Count != this.Algorithms.Length)
			throw new InvalidOperationException(
				$"The race returned {check.Count} results for {this.Algorithms.Length} algorithms.");
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.Run(this.loop);

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel_OneThread()
		=> this.Run(this.parallelOneThread);

	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> Parallel()
		=> this.Run(this.parallel);

	private static TestOperationParameters Parameters()
		=> new() { Operation = AlgorithmOperation.Packing };

	private IDictionary<string, OperationResult> Run(IAlgorithmProcessor processor)
		=> processor.Process(this.bin, this.items, Parameters());
}
