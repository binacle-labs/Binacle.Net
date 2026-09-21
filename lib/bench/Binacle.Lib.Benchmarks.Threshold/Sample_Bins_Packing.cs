using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Threshold;

[MemoryDiagnoser]
[BenchmarkCategory("sample", "packing")]
public class Sample_Bins_Packing : BinsBase
{
	[Params(3, 47, 79)]
	public override int Items { get; set; }

	[Params(1, 2, 3, 4, 5, 6, 7)]
	public override int Bins { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
