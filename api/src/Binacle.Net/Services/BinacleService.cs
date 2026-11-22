using Binacle.Lib.Abstractions;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Services;

internal class BinacleService : IBinacleService
{
	private readonly IBinProcessorFactory binProcessorFactory;
	private readonly IAlgorithmProcessorFactory algorithmProcessorFactory;
	private readonly IResultSelector resultSelector;
	private readonly IAlgorithmFactory algorithmFactory;

	public BinacleService(
		IBinProcessorFactory binProcessorFactory,
		IAlgorithmProcessorFactory algorithmProcessorFactory,
		IResultSelector resultSelector,
		IAlgorithmFactory algorithmFactory
	)
	{
		this.binProcessorFactory = binProcessorFactory;
		this.algorithmProcessorFactory = algorithmProcessorFactory;
		this.resultSelector = resultSelector;
		this.algorithmFactory = algorithmFactory;
	}

	public ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var result = this.algorithmFactory.Create(algorithm, bin, items).Execute(parameters);
		return ValueTask.FromResult(result);
	}

	public ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.algorithmProcessorFactory
			.Create(items.Count)
			.Process(bin, items, parameters, cancellationToken);
		return ValueTask.FromResult(this.resultSelector.BestAlgorithm(results));
	}

	public ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.Create(bins.Count, items.Count)
			.Process(algorithm, bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(results);
	}

	public ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.CreateMultiAlgorithm(bins.Count, items.Count)
			.Process(bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(results);
	}

	public ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.Create(bins.Count, items.Count)
			.Process(algorithm, bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(this.resultSelector.SmallestBin(results));
	}

	public ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.CreateMultiAlgorithm(bins.Count, items.Count)
			.Process(bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(this.resultSelector.SmallestBin(results));
	}

	public ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.Create(bins.Count, items.Count)
			.Process(algorithm, bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(this.resultSelector.BestBin(results));
	}

	public ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		var results = this.binProcessorFactory
			.CreateMultiAlgorithm(bins.Count, items.Count)
			.Process(bins, items, parameters, cancellationToken);
		return ValueTask.FromResult(this.resultSelector.BestBin(results));
	}
}
