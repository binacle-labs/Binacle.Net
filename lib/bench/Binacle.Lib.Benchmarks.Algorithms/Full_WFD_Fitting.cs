namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
public class Full_WFD_Fitting : FullBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Fitting;

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult v1()
		=> this.Run(AlgorithmFactories.WFD_v1);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult v2()
		=> this.Run(AlgorithmFactories.WFD_v2);
}
