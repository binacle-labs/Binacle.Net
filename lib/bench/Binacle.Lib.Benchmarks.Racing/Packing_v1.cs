using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Racing;

// Deleted with v1.
[MemoryDiagnoser]
[BenchmarkCategory("packing")]
public class Packing_v1 : BenchmarkBase
{
	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v1();
}
