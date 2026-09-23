using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.Benchmarks.Threshold;

public abstract class AlgorithmsBase
{
	private LoopAlgorithmProcessor loop = null!;
	private ParallelAlgorithmProcessor parallel = null!;
	private ScenarioBin bin = null!;
	private List<ScenarioItem> items = null!;

	// Each tier picks its own steps of the ladder.
	public abstract int Items { get; set; }

	// The two sets production races: multi-bin routes and single-bin routes.
	[Params("FFD,BFD", "FFD,WFD,BFD")]
	public string Set { get; set; } = null!;

	protected abstract IAlgorithmFactory AlgorithmFactory { get; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		var algorithms = this.Set.Split(',').Select(Enum.Parse<Algorithm>).ToArray();
		this.loop = new LoopAlgorithmProcessor(algorithms, this.AlgorithmFactory);
		this.parallel = new ParallelAlgorithmProcessor(algorithms, this.AlgorithmFactory);
		this.bin = ScenarioBin.FromCompactString(LadderGenerator.MaxSizeBin);
		this.items = LadderGenerator.GetItems(this.Items);
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.Run(this.loop);

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
