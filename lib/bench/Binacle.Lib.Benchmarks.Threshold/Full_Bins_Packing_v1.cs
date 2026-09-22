using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

// Deleted with v1.
[MemoryDiagnoser]
public class Full_Bins_Packing_v1 : BinsBase
{
	[Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
	public override int Items { get; set; }

	[Params(1, 2, 3, 4, 5, 6, 7)]
	public override int Bins { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v1();
}
