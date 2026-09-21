namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class SmokeBase : BenchmarkBase
{
	[ParamsSource(typeof(SmokeProblemsProvider), nameof(SmokeProblemsProvider.GetScenarioNames))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> SmokeProblemsProvider.GetScenarioByName(this.ScenarioName!);

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	[BenchmarkCategory("ffd")]
	public OperationResult FFD_v1()
		=> this.Run(AlgorithmFactories.FFD_v1);

	[Benchmark]
	[BenchmarkOrder(20)]
	[BenchmarkCategory("ffd")]
	public OperationResult FFD_v2()
		=> this.Run(AlgorithmFactories.FFD_v2);

	// Deleted with v1.
	[Benchmark]
	[BenchmarkOrder(30)]
	[BenchmarkCategory("wfd")]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1);

	[Benchmark]
	[BenchmarkOrder(40)]
	[BenchmarkCategory("wfd")]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2);

	// Deleted with v1.
	[Benchmark]
	[BenchmarkOrder(50)]
	[BenchmarkCategory("bfd")]
	public OperationResult BFD_v1()
		=> this.Run(AlgorithmFactories.BFD_v1);

	[Benchmark]
	[BenchmarkOrder(60)]
	[BenchmarkCategory("bfd")]
	public OperationResult BFD_v2()
		=> this.Run(AlgorithmFactories.BFD_v2);
}
