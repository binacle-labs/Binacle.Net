namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class SampleBase : BenchmarkBase
{
	[ParamsSource(typeof(SampleSet), nameof(SampleSet.Names))]
	public string? ScenarioName { get; set; }

	protected override Scenario Load()
		=> SampleSet.GetByName(this.ScenarioName!);
}
