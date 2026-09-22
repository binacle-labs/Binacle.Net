namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class SmokeBase : BenchmarkBase
{
	[ParamsSource(typeof(SmokeProblemsProvider), nameof(SmokeProblemsProvider.GetScenarioNames))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> SmokeProblemsProvider.GetScenarioByName(this.ScenarioName!);
}
