using System.Collections.Concurrent;
using Binacle.Lib.Abstractions;

namespace Binacle.Lib.AlgorithmProcessing;

public class ParallelAlgorithmProcessor: IAlgorithmProcessor
{
    private readonly Algorithm[] supportedAlgorithms;
    private readonly IAlgorithmFactory algorithmFactory;
    private readonly int maxDegreeOfParallelism;
    private readonly int concurrencyLevel;

    public ParallelAlgorithmProcessor(
        Algorithm[] supportedAlgorithms,
        IAlgorithmFactory algorithmFactory,
        int? maxDegreeOfParallelism = null
    )
    {
        this.supportedAlgorithms = supportedAlgorithms;
        this.algorithmFactory = algorithmFactory;
        // Capped to the race width by default; pass ParallelLimits.NoLimit to lift it.
        this.maxDegreeOfParallelism = maxDegreeOfParallelism ?? ParallelLimits.Degree(supportedAlgorithms.Length);

        // One race is one algorithm per thread, and the algorithms are known here.
        this.concurrencyLevel = ParallelLimits.ConcurrencyLevel(
            supportedAlgorithms.Length,
            this.maxDegreeOfParallelism);
    }
    
    public IDictionary<string, OperationResult> Process<TBin, TItem>(
        TBin bin, 
        IList<TItem> items, 
        IOperationParameters parameters,
        CancellationToken cancellationToken = default
    ) 
        where TBin : class, IWithID, IWithReadOnlyDimensions 
        where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
    {
        using var activity = Diagnostics.ActivitySource
            .StartActivity($"Process Algorithms: Parallel");
        activity?.SetTag("Operation", parameters.Operation);
        var results = new ConcurrentDictionary<string, OperationResult>(this.concurrencyLevel, this.supportedAlgorithms.Length);

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = this.maxDegreeOfParallelism
        };
        Parallel.For(0, this.supportedAlgorithms.Length, parallelOptions, i =>
        {
            var algorithm = this.supportedAlgorithms[i];
            var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
            var result = algorithmInstance.Execute(parameters);
            results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
        });
        return results;
    }
}
