using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

public abstract class BinsBase
{
	private LoopBinProcessor loop = null!;
	private ParallelBinProcessor parallel = null!;
	private List<ScenarioBin> bins = null!;
	private List<ScenarioItem> items = null!;

	// Each tier picks its own steps of the ladder and its own bin counts.
	public abstract int Items { get; set; }
	public abstract int Bins { get; set; }

	// The multi-bin routes never run WFD.
	[Params(Algorithm.FFD, Algorithm.BFD)]
	public Algorithm Algorithm { get; set; }

	protected abstract IAlgorithmFactory AlgorithmFactory { get; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.loop = new LoopBinProcessor(this.AlgorithmFactory);
		this.parallel = new ParallelBinProcessor(this.AlgorithmFactory);
		this.bins = SpecializedScalingProblemsProvider.GetBins(this.Bins);
		this.items = SpecializedScalingProblemsProvider.GetItems(this.Items);
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.Run(this.loop);

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel()
		=> this.Run(this.parallel);

	private IDictionary<string, OperationResult> Run(IBinProcessor processor)
		=> processor.Process(
			this.Algorithm,
			this.bins,
			this.items,
			new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
