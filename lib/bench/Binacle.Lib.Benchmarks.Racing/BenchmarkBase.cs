using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.Benchmarks.Racing;

public abstract class BenchmarkBase
{
	private LoopAlgorithmProcessor loop = null!;
	private ParallelAlgorithmProcessor parallel = null!;
	private Scenario? scenario;

	// Each tier picks its own problems.
	public abstract string? ScenarioName { get; set; }

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
		this.scenario = RacingSet.GetByName(this.ScenarioName!);
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
			this.scenario!.Bin,
			this.scenario.Items,
			new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
