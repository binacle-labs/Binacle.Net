using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class Sample_Packing
{
	private ScenarioBin bin = null!;
	private List<ScenarioItem> items = null!;

	// Every step of the ladder, so the curve has no gap. The bin is the largest one and never changes.
	[Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
	public int Items { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.bin = ScenarioBin.FromCompactString(LadderGenerator.MaxSizeBin);
		this.items = LadderGenerator.GetItems(this.Items);
	}

	// Deleted with v1.
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult FFD_v1()
		=> this.Run(AlgorithmFactories.FFD_v1);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult FFD_v2()
		=> this.Run(AlgorithmFactories.FFD_v2);

	[Benchmark]
	[BenchmarkOrder(30)]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1);

	[Benchmark]
	[BenchmarkOrder(40)]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2);

	[Benchmark]
	[BenchmarkOrder(50)]
	public OperationResult BFD_v1()
		=> this.Run(AlgorithmFactories.BFD_v1);

	[Benchmark]
	[BenchmarkOrder(60)]
	public OperationResult BFD_v2()
		=> this.Run(AlgorithmFactories.BFD_v2);

	private OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> factory)
		=> factory(this.bin, this.items)
			.Execute(new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
