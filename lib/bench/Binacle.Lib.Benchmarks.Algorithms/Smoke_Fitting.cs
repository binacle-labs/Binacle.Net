namespace Binacle.Lib.Benchmarks.Algorithms;

[MemoryDiagnoser]
public class Smoke_Fitting : SmokeBase
{
	protected override AlgorithmOperation Operation
		=> AlgorithmOperation.Fitting;
}
