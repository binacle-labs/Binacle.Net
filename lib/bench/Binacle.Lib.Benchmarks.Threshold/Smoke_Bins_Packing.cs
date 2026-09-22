using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

[MemoryDiagnoser]
public class Smoke_Bins_Packing : BinsBase
{
	[Params(3, 47, 67, 79)]
	public override int Items { get; set; }

	// 2 is the first count that can win, 3 is every preset, 7 is the top of the ladder.
	[Params(2, 3, 7)]
	public override int Bins { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
