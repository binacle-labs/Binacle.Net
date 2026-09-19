using Binacle.Lib.Testing.Providers;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Data;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class AlgorithmParallelizationThresholdBenchmarkBase
{
    private ParallelAlgorithmProcessor parallelAlgorithmProcessor = null!;
    private LoopAlgorithmProcessor loopAlgorithmProcessor = null!;

    [ParamsSource(typeof(ConcurrencyProvider), nameof(ConcurrencyProvider.GetProcessorCount))]
    public int ProcessorCount { get; set; }

    [Params("BFD,WFD", "FFD,BFD", "FFD,WFD", "FFD,BFD,WFD")]
    public string Algorithms { get; set; } = null!;
	
    [Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
    public int ItemCount { get; set; }
    
    public ScenarioBin Bin { get; set; } = null!;
    public List<ScenarioItem> Items { get; set; } = null!;
	
    protected abstract AlgorithmOperation AlgorithmOperation { get; }
    protected abstract IAlgorithmFactory AlgorithmFactory { get; }
	
    [GlobalSetup]
    public void GlobalSetup()
    {
        var algorithms = this.Algorithms.Split(',').Select(Enum.Parse<Algorithm>).ToArray();
        this.loopAlgorithmProcessor = new LoopAlgorithmProcessor(algorithms, this.AlgorithmFactory);
        this.parallelAlgorithmProcessor = new ParallelAlgorithmProcessor(algorithms, this.AlgorithmFactory, this.ProcessorCount);
        this.Bin = ScenarioBin.FromCompactString(SpecializedScalingProblemsProvider.MaxSizeBin);
        this.Items = SpecializedScalingProblemsProvider.GetItems(this.ItemCount);
    }
	
    [Benchmark(Baseline = true)]
    [BenchmarkOrder(10)]
    public IDictionary<string,OperationResult> Loop()
    {
	    return this.loopAlgorithmProcessor.Process(
		    this.Bin,
		    this.Items,
		    new TestOperationParameters()
		    {
			    Operation = this.AlgorithmOperation
		    }
	    );
    }

    [Benchmark]
    [BenchmarkOrder(20)]
    public IDictionary<string, OperationResult> Parallel()
    {
	    return this.parallelAlgorithmProcessor.Process(
		    this.Bin,
		    this.Items,
		    new TestOperationParameters()
		    {
			    Operation = this.AlgorithmOperation
		    }
	    );
    }
}
