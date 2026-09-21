using Binacle.Data.BischoffSuite;

namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class FullBase : BenchmarkBase
{
	[ParamsSource(typeof(Scenarios), nameof(Scenarios.GetScenarioNames))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> Scenarios.GetScenarioByName(this.ScenarioName!);
}
