using Binacle.Lib.Abstractions;
using Binacle.Lib.Data.ResultSelection;

namespace Binacle.Lib.Benchmarks.ResultSelection;

public abstract class BenchmarkBase
{
	private Scenario? scenario;

	public abstract string? ScenarioName { get; set; }

	// Each set resolves names in its own Scenarios; the short names repeat across sets.
	protected abstract Scenario Load(string name);

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.scenario = this.Load(this.ScenarioName!);
	}

	protected OperationResult Run(IResultSelectionStrategy strategy)
		=> strategy.Select(this.scenario!.Results);
}
