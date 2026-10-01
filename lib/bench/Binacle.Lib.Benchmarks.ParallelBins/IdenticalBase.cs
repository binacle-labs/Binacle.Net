using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.ParallelBins;

// The best case for Parallel: every bin the same, every item the same, and every item fits.
public abstract class IdenticalBase
{
	private LoopBinProcessor loop = null!;
	private ParallelBinProcessor parallel = null!;
	private List<ScenarioBin> bins = null!;
	private List<ScenarioItem> items = null!;

	protected abstract Algorithm Algorithm { get; }

	public abstract int Bins { get; set; }

	// Item lines. The pieces are split evenly across them.
	public abstract int Lines { get; set; }

	public abstract int Pieces { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		JobsByCoreCount.SetCoreCountAndCheck();

		var factory = new AlgorithmFactory_v2();
		this.loop = new LoopBinProcessor(factory);
		this.parallel = new ParallelBinProcessor(factory);

		var dimensions = IdenticalCase.Bin();
		this.bins = Enumerable.Range(1, this.Bins)
			.Select(i => new ScenarioBin($"bin{i}", dimensions))
			.ToList();
		this.items = IdenticalCase.Items(this.Pieces, this.Lines);

		var results = this.Run(this.loop);
		if (results.Values.Any(result => result.Status != OperationResultStatus.FullyPacked))
			throw new InvalidOperationException($"{this.Pieces} items do not all fit in {IdenticalCase.BinSize}.");
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
