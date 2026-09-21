namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
[BenchmarkCategory("smoke", "packing")]
public class Smoke_Packing : SmokeBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Packing;
}
