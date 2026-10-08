using Binacle.Lib.Data.ResultSelection;
using Binacle.Lib.ResultSelection;
using BestAlgorithmData = Binacle.Lib.Data.ResultSelection.BestAlgorithm.DataProvider;

namespace Binacle.Lib.Benchmarks.ResultSelection;

[MemoryDiagnoser]
public class BestAlgorithm : BenchmarkBase
{
	[ParamsSource(typeof(BestAlgorithmData), nameof(BestAlgorithmData.Names))]
	public override string? ScenarioName { get; set; }

	protected override Scenario Load(string name)
		=> BestAlgorithmData.GetByName(name);

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult v1()
		=> this.Run(new BestAlgorithm_v1());

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult v2()
		=> this.Run(new BestAlgorithm_v2());
}
