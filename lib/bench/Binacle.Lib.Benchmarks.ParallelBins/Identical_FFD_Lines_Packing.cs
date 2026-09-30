namespace Binacle.Lib.Benchmarks.ParallelBins;

// The same pieces over more lines. Flat if lines cost nothing once they are turned into pieces.
[MemoryDiagnoser]
public class Identical_FFD_Lines_Packing : IdenticalBase
{
	protected override Algorithm Algorithm => Algorithm.FFD;

	[Params(4)]
	public override int Bins { get; set; }

	[Params(1, 4, 16, 64)]
	public override int Lines { get; set; }

	[Params(64, 256)]
	public override int Pieces { get; set; }
}
