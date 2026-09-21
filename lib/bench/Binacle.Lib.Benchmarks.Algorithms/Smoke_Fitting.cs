namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
[BenchmarkCategory("smoke", "fitting")]
public class Smoke_Fitting : SmokeBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Fitting;
}
