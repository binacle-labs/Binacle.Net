namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class SmokeBase : BenchmarkBase
{
	[ParamsSource(typeof(SmokeSet), nameof(SmokeSet.Names))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> SmokeSet.GetByName(this.ScenarioName!);
}
