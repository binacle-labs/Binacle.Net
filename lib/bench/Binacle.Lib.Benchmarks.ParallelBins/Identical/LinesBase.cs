namespace Binacle.Lib.Benchmarks.ParallelBins;

// The grid every Lines class runs: the same pieces handed over as one line of many, then as many lines of
// one. Flat means lines cost nothing once they are pieces.
public abstract class LinesBase : IdenticalBase
{
	[Params(4)]
	public override int Bins { get; set; }

	[Params(1, 4, 16, 64)]
	public override int Lines { get; set; }

	[Params(64, 256)]
	public override int Pieces { get; set; }
}
