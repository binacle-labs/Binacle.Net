using System.Collections.Concurrent;
using Binacle.Lib.Abstractions;

namespace Binacle.Lib;

public class ParallelBinProcessor : IBinProcessor
{
	private readonly IAlgorithmFactory algorithmFactory;
	private readonly int? maxDegreeOfParallelism;

	public ParallelBinProcessor(
		IAlgorithmFactory algorithmFactory,
		int? maxDegreeOfParallelism = null
		)
	{
		this.algorithmFactory = algorithmFactory;
		this.maxDegreeOfParallelism = maxDegreeOfParallelism;
	}
	
	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity($"Process Bins: Parallel");
		activity?.SetTag("Operation", parameters.Operation);

		// One bin per thread, and the bin count is only known here, so the default cap resolves here too.
		var degree = this.maxDegreeOfParallelism ?? ParallelLimits.Degree(bins.Count);
		var concurrencyLevel = ParallelLimits.ConcurrencyLevel(bins.Count, degree);
		var results = new ConcurrentDictionary<string, OperationResult>(concurrencyLevel, bins.Count);

		var parallelOptions = new ParallelOptions
		{
			CancellationToken = cancellationToken,
			MaxDegreeOfParallelism = degree
		};
		Parallel.For(0, bins.Count, parallelOptions, i =>
		{
			var bin = bins[i];
			var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
			var result = algorithmInstance.Execute(parameters);
			results[bin.ID] = result;
		});

		return results;
	}
}
