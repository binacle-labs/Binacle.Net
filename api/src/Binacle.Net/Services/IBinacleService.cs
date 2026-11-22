using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Services;

internal interface IBinacleService
{
	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;


	ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		Algorithm algorithm,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;

	ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider;
}
