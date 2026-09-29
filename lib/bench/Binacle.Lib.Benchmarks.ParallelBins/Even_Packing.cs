using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.ParallelBins;

// The best case for Parallel: every bin the same, every item the same, and every item fits.
[MemoryDiagnoser]
public class Even_Packing
{
	// Eight by eight by eight items; 256 fills half of it.
	private const string BinSize = "160x120x80";
	private const string ItemSize = "20x15x10";

	private LoopBinProcessor loop = null!;
	private ParallelBinProcessor parallel = null!;
	private List<ScenarioBin> bins = null!;
	private List<ScenarioItem> items = null!;

	[Params(1, 2, 4, 8, 16)]
	public int Bins { get; set; }

	// Item lines. The pieces are split evenly across them.
	[Params(1)]
	public int Lines { get; set; }

	[Params(8, 16, 32, 64, 128, 256)]
	public int Pieces { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		CorePinning.PinAndCheck();

		var factory = new AlgorithmFactory_v2();
		this.loop = new LoopBinProcessor(factory);
		this.parallel = new ParallelBinProcessor(factory);

		var dimensions = ScenarioBin.FromCompactString(BinSize);
		this.bins = Enumerable.Range(1, this.Bins)
			.Select(i => new ScenarioBin($"bin{i}", dimensions))
			.ToList();

		var perLine = this.Pieces / this.Lines;
		this.items = Enumerable.Range(1, this.Lines)
			.Select(_ => ScenarioItem.FromCompactString($"{ItemSize} [{perLine}]"))
			.ToList();

		var results = this.Run(this.loop);
		if (results.Values.Any(result => result.Status != OperationResultStatus.FullyPacked))
			throw new InvalidOperationException($"{this.Pieces} items do not all fit in {BinSize}.");
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
			Algorithm.FFD,
			this.bins,
			this.items,
			new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
