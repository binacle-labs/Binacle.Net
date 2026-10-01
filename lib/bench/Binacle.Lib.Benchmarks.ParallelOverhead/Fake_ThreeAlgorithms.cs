namespace Binacle.Lib.Benchmarks.ParallelOverhead;

// A three-algorithm race, the one the single-bin routes run.
public class Fake_ThreeAlgorithms : FakeRaceBase
{
	protected override Algorithm[] Algorithms => [Algorithm.FFD, Algorithm.WFD, Algorithm.BFD];
}
