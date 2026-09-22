using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Racing;

// Two of the five problems, v2 only.
[MemoryDiagnoser]
public class Smoke_Packing : BenchmarkBase
{
	[Params("typical container", "BFD wins big")]
	public override string? ScenarioName { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
