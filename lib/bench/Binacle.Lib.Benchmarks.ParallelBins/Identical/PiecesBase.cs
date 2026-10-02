namespace Binacle.Lib.Benchmarks.ParallelBins;

// The grid every Pieces class runs. It lives here so the three cannot drift apart - their reports are only
// comparable while the values match.
public abstract class PiecesBase : IdenticalBase
{
	[Params(1, 2, 4, 8, 16, 24, 32)]
	public override int Bins { get; set; }

	[Params(1)]
	public override int Lines { get; set; }

	[Params(1, 2, 4, 8, 16, 32, 64, 128, 256)]
	public override int Pieces { get; set; }
}
