using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib.Benchmarks.Racing;

// The two races, and each algorithm alone, on every core count in CoreJobs. The category is the row's block: each
// race has its own Loop baseline, and the alone rows have none.
[MemoryDiagnoser]
public class Cores_Packing
{
	private const string TwoAlgorithms = "FFD,BFD";
	private const string ThreeAlgorithms = "FFD,WFD,BFD";
	private const string Alone = "alone";

	private readonly IAlgorithmFactory algorithmFactory = new AlgorithmFactory_v2();
	private IAlgorithmProcessor loopTwo = null!;
	private IAlgorithmProcessor parallelTwo = null!;
	private IAlgorithmProcessor loopThree = null!;
	private IAlgorithmProcessor parallelThree = null!;
	private IAlgorithmProcessor ffd = null!;
	private IAlgorithmProcessor wfd = null!;
	private IAlgorithmProcessor bfd = null!;
	private Scenario? scenario;

	[ParamsSource(typeof(CoresSet), nameof(CoresSet.Names))]
	public string ScenarioName { get; set; } = null!;

	[GlobalSetup]
	public void GlobalSetup()
	{
		CorePinning.PinAndCheck();

		Algorithm[] two = [Algorithm.FFD, Algorithm.BFD];
		Algorithm[] three = [Algorithm.FFD, Algorithm.WFD, Algorithm.BFD];
		this.loopTwo = new LoopAlgorithmProcessor(two, this.algorithmFactory);
		this.parallelTwo = new ParallelAlgorithmProcessor(two, this.algorithmFactory);
		this.loopThree = new LoopAlgorithmProcessor(three, this.algorithmFactory);
		this.parallelThree = new ParallelAlgorithmProcessor(three, this.algorithmFactory);

		// A loop of one, so an alone row runs the same code as one step of a Loop row.
		this.ffd = new LoopAlgorithmProcessor([Algorithm.FFD], this.algorithmFactory);
		this.wfd = new LoopAlgorithmProcessor([Algorithm.WFD], this.algorithmFactory);
		this.bfd = new LoopAlgorithmProcessor([Algorithm.BFD], this.algorithmFactory);

		this.scenario = CoresSet.GetByName(this.ScenarioName);
	}

	[Benchmark(Baseline = true)]
	[BenchmarkCategory(TwoAlgorithms)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop_FFD_BFD()
		=> this.Run(this.loopTwo);

	[Benchmark]
	[BenchmarkCategory(TwoAlgorithms)]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel_FFD_BFD()
		=> this.Run(this.parallelTwo);

	[Benchmark(Baseline = true)]
	[BenchmarkCategory(ThreeAlgorithms)]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> Loop_FFD_WFD_BFD()
		=> this.Run(this.loopThree);

	[Benchmark]
	[BenchmarkCategory(ThreeAlgorithms)]
	[BenchmarkOrder(40)]
	public IDictionary<string, OperationResult> Parallel_FFD_WFD_BFD()
		=> this.Run(this.parallelThree);

	[Benchmark]
	[BenchmarkCategory(Alone)]
	[BenchmarkOrder(50)]
	public IDictionary<string, OperationResult> FFD()
		=> this.Run(this.ffd);

	[Benchmark]
	[BenchmarkCategory(Alone)]
	[BenchmarkOrder(60)]
	public IDictionary<string, OperationResult> WFD()
		=> this.Run(this.wfd);

	[Benchmark]
	[BenchmarkCategory(Alone)]
	[BenchmarkOrder(70)]
	public IDictionary<string, OperationResult> BFD()
		=> this.Run(this.bfd);

	private IDictionary<string, OperationResult> Run(IAlgorithmProcessor processor)
		=> processor.Process(
			this.scenario!.Bin,
			this.scenario.Items,
			new TestOperationParameters { Operation = AlgorithmOperation.Packing });
}
