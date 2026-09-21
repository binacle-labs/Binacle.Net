namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class SampleBase : BenchmarkBase
{
	[ParamsSource(typeof(BischoffSampleProblemsProvider), nameof(BischoffSampleProblemsProvider.GetScenarioNames))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> BischoffSampleProblemsProvider.GetScenarioByName(this.ScenarioName!);
}
