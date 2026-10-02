using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

// The ideal case: one bin, one item shape, every piece fits.
[MemoryDiagnoser]
public abstract class IdenticalBase
{
	private IAlgorithmProcessor loop = null!;
	private IAlgorithmProcessor parallel = null!;
	private ScenarioBin bin = null!;
	private List<ScenarioItem> items = null!;

	protected abstract Algorithm[] Algorithms { get; }

	// Every class in this folder sweeps these values. Changing them for one class breaks the comparison the
	// scenario exists for.
	[Params(1, 2, 4, 8, 16, 32, 64, 128, 256)]
	public int Pieces { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		JobsByCoreCount.SetCoreCountAndCheck();

		var factory = new AlgorithmFactory_v2();
		this.loop = new LoopAlgorithmProcessor(this.Algorithms, factory);
		this.parallel = new ParallelAlgorithmProcessor(this.Algorithms, factory);

		this.bin = IdenticalCase.Bin();
		this.items = IdenticalCase.Items(this.Pieces);

		var results = this.Run(this.loop);
		if (results.Count != this.Algorithms.Length
			|| results.Values.Any(result => result.Status != OperationResultStatus.FullyPacked))
			throw new InvalidOperationException($"{this.Pieces} items do not all fit in {IdenticalCase.BinSize}.");
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.Run(this.loop);

	// On a class that races one algorithm this is not a race: the default cap resolves to 1, so Parallel.For
	// runs the body inline and nothing is handed to another CPU.
	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel()
		=> this.Run(this.parallel);

	private IDictionary<string, OperationResult> Run(IAlgorithmProcessor processor)
		=> processor.Process(
			this.bin,
			this.items,
			new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
