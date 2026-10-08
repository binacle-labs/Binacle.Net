using Binacle.Lib.Data.ResultSelection;
using Binacle.Lib.ResultSelection;
using BestBinData = Binacle.Lib.Data.ResultSelection.BestBin.DataProvider;

namespace Binacle.Lib.Benchmarks.ResultSelection;

[MemoryDiagnoser]
public class BestBin : BenchmarkBase
{
	[ParamsSource(typeof(BestBinData), nameof(BestBinData.Names))]
	public override string? ScenarioName { get; set; }

	protected override Scenario Load(string name)
		=> BestBinData.GetByName(name);

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult v1()
		=> this.Run(new BestBin_v1());

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult v2()
		=> this.Run(new BestBin_v2());
}
