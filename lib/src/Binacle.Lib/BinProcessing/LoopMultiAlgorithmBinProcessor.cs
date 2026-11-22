using Binacle.Lib.Abstractions;

namespace Binacle.Lib;

public class LoopMultiAlgorithmBinProcessor : IMultiAlgorithmBinProcessor
{
	private readonly IAlgorithmProcessor algorithmProcessor;
	private readonly IResultSelector resultSelector;

	public LoopMultiAlgorithmBinProcessor(
		IAlgorithmProcessor algorithmProcessor,
		IResultSelector resultSelector
	)
	{
		this.algorithmProcessor = algorithmProcessor;
		this.resultSelector = resultSelector;
	}

	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		using var activity = Diagnostics.ActivitySource
			.StartProcessorActivity("process multi_algorithm_bins", DiagnosticsTags.Loop, parameters.Operation);
		var results = new Dictionary<string, OperationResult>(bins.Count);

		for (var i = 0; i < bins.Count; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var bin = bins[i];
			var algorithmResults = this.algorithmProcessor.Process(bin, items, parameters, cancellationToken);
			var selectedAlgorithmResult = this.resultSelector.BestAlgorithm(algorithmResults);
			results[bin.ID] = selectedAlgorithmResult;
		}

		return results;
	}
}
