namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

// The race the single-bin routes run.
public class Identical_FFD_WFD_BFD_Packing : IdenticalBase
{
	protected override Algorithm[] Algorithms => [Algorithm.FFD, Algorithm.WFD, Algorithm.BFD];
}
