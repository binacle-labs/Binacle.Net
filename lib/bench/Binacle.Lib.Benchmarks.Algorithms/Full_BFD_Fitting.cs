namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
[BenchmarkCategory("full", "bfd", "fitting")]
public class Full_BFD_Fitting : FullBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Fitting;

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult v1()
		=> this.Run(AlgorithmFactories.BFD_v1);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult v2()
		=> this.Run(AlgorithmFactories.BFD_v2);
}
