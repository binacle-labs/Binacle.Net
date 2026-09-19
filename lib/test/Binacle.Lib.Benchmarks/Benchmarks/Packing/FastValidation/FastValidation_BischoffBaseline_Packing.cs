using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.Data;
using Binacle.Data.BischoffSuite;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_BischoffBaseline_Packing : FastValidationBenchmarkBase
{
	protected override Scenario? GetScenario() =>
		Scenarios.GetScenarioByName(
			BischoffCuratedProblemsProvider.ScenarioDescriptions["Baseline"]
		);
	
	protected override AlgorithmOperation AlgorithmOperation 
		=> AlgorithmOperation.Packing;
}
