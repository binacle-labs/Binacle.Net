namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
[BenchmarkCategory("full", "bfd", "packing")]
public class Full_BFD_Packing : FullBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Packing;

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
