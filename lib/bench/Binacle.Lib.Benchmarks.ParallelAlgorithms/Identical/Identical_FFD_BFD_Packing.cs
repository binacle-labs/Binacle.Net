namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

// The race the multi-bin routes run.
public class Identical_FFD_BFD_Packing : IdenticalBase
{
	protected override Algorithm[] Algorithms => [Algorithm.FFD, Algorithm.BFD];
}
