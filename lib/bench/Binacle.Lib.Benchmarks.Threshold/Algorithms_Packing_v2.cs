using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

[MemoryDiagnoser]
[BenchmarkCategory("sample", "full", "packing")]
public class Algorithms_Packing_v2 : AlgorithmsBase
{
	[Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
	public override int Items { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
