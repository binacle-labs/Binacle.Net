namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class FullBase : BenchmarkBase
{
	[ParamsSource(typeof(BischoffSuite.DataProvider), nameof(BischoffSuite.DataProvider.Names))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> BischoffSuite.DataProvider.GetByName(this.ScenarioName!);
}
