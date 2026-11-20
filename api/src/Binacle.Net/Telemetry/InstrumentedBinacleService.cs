using Binacle.Lib.Abstractions;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Services;

namespace Binacle.Net.Telemetry;

internal class InstrumentedBinacleService : IBinacleService
{
	private readonly IBinacleService inner;
	private readonly PackingMetrics metrics;
	private readonly ILogger<InstrumentedBinacleService> logger;

	public InstrumentedBinacleService(
		IBinacleService inner,
		PackingMetrics metrics,
		ILogger<InstrumentedBinacleService> logger
	)
	{
		this.inner = inner;
		this.metrics = metrics;
		this.logger = logger;
	}

	public async ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
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
		using var scope = this.Start(PackingMode.SingleBin, algorithm, 1, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.SingleBinAsync(algorithm, bin, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	public async ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
		TBin bin,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var scope = this.Start(PackingMode.SingleBin, null, 1, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.SingleBinAsync(bin, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	public async ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
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
		using var scope = this.Start(PackingMode.MultipleBins, algorithm, bins.Count, items, parameters);
		var results = await scope.RunAsync(() =>
			this.inner.MultipleBinsAsync(algorithm, bins, items, parameters, cancellationToken)
		);
		scope.Record(results.Values);
		return results;
	}

	public async ValueTask<IDictionary<string, OperationResult>> MultipleBinsAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var scope = this.Start(PackingMode.MultipleBins, null, bins.Count, items, parameters);
		var results = await scope.RunAsync(() =>
			this.inner.MultipleBinsAsync(bins, items, parameters, cancellationToken)
		);
		scope.Record(results.Values);
		return results;
	}

	public async ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
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
		using var scope = this.Start(PackingMode.SmallestBin, algorithm, bins.Count, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.SmallestBinAsync(algorithm, bins, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	public async ValueTask<OperationResult> SmallestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var scope = this.Start(PackingMode.SmallestBin, null, bins.Count, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.SmallestBinAsync(bins, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	public async ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
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
		using var scope = this.Start(PackingMode.BestBin, algorithm, bins.Count, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.BestBinAsync(algorithm, bins, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	public async ValueTask<OperationResult> BestBinAsync<TBin, TBox, TParams>(
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IIdentifiableBin
		where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
		where TParams : class, IOperationParameters, ILogParametersProvider
	{
		using var scope = this.Start(PackingMode.BestBin, null, bins.Count, items, parameters);
		var result = await scope.RunAsync(() =>
			this.inner.BestBinAsync(bins, items, parameters, cancellationToken)
		);
		scope.Record(result);
		return result;
	}

	private PackingScope Start(
		PackingMode mode,
		Algorithm? algorithm,
		int binCount,
		IReadOnlyList<IWithQuantity> items,
		IOperationParameters parameters
	) => PackingScope.Start(this.metrics, this.logger, mode, algorithm, binCount, items, parameters);
}
