namespace Binacle.Lib.Benchmarks.ParallelOverhead;

// A two-algorithm race, the one the multi-bin routes run.
public class Fake_TwoAlgorithms : FakeRaceBase
{
	protected override Algorithm[] Algorithms => [Algorithm.FFD, Algorithm.BFD];
}
