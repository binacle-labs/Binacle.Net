using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Racing;

[MemoryDiagnoser]
[BenchmarkCategory("packing")]
public class Packing_v2 : BenchmarkBase
{
	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
