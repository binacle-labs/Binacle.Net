using BischoffSuite = Binacle.Data.BischoffSuite.DataProvider;

namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class FullBase : BenchmarkBase
{
	[ParamsSource(typeof(BischoffSuite), nameof(BischoffSuite.Names))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> BischoffSuite.GetByName(this.ScenarioName!);
}
