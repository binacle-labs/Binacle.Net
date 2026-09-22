using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

[MemoryDiagnoser]
public class Smoke_Algorithms_Packing : AlgorithmsBase
{
	// Thread cost dominates at 3; 47 fits every bin; 67 and 79 are where the algorithms take unequal time.
	[Params(3, 47, 67, 79)]
	public override int Items { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
