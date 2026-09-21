using Binacle.Lib.Data.ResultSelection;
using Binacle.Lib.ResultSelection;
using Scenarios = Binacle.Lib.Data.ResultSelection.SmallestBin.Scenarios;

namespace Binacle.Lib.Benchmarks.ResultSelection;

[MemoryDiagnoser]
public class SmallestBin : BenchmarkBase
{
	[ParamsSource(typeof(Scenarios), nameof(Scenarios.GetScenarioNames))]
	public override string? ScenarioName { get; set; }

	protected override Scenario Load(string name)
		=> Scenarios.GetScenarioByName(name);

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult v1()
		=> this.Run(new SmallestBin_v1());

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult v2()
		=> this.Run(new SmallestBin_v2());
}
