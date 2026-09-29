namespace Binacle.Lib.Benchmarks.ParallelBins;

[MemoryDiagnoser]
public class Even_Packing : EvenBase
{
	[Params(1, 2, 4, 8, 16, 24, 32)]
	public override int Bins { get; set; }

	[Params(1)]
	public override int Lines { get; set; }

	[Params(1, 2, 4, 8, 16, 32, 64, 128, 256)]
	public override int Pieces { get; set; }
}
