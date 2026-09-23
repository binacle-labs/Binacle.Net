using Binacle.Lib.Abstractions;

namespace Binacle.Lib.Benchmarks.Racing;

[MemoryDiagnoser]
public class Sample_Packing_v2 : BenchmarkBase
{
	[ParamsSource(typeof(RacingSet), nameof(RacingSet.Names))]
	public override string? ScenarioName { get; set; }

	protected override IAlgorithmFactory AlgorithmFactory
		=> new AlgorithmFactory_v2();
}
