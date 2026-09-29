using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

[MemoryDiagnoser]
public class Sample_Packing : LadderBase
{
	[Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
	public override int Items { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
