namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

// FFD on its own. The alone classes say how far apart the algorithms are, and whether Loop is their sum.
public class Identical_FFD_Packing : IdenticalBase
{
	protected override Algorithm[] Algorithms => [Algorithm.FFD];
}
