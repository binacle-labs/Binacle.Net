namespace Binacle.Lib.Benchmarks.ParallelBins;

[MemoryDiagnoser]
public class Identical_WFD_Packing : IdenticalBase
{
	protected override Algorithm Algorithm => Algorithm.WFD;

	[Params(1, 2, 4, 8, 16, 24, 32)]
	public override int Bins { get; set; }

	[Params(1)]
	public override int Lines { get; set; }

	[Params(1, 2, 4, 8, 16, 32, 64, 128, 256)]
	public override int Pieces { get; set; }
}
