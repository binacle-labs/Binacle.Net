using System.Threading.Channels;
using Binacle.Lib.Abstractions;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Services;

internal class PackingLogBinacleService : IBinacleService
{
	private readonly IBinacleService inner;
	private readonly Channel<AlgorithmOperationLogChannelRequest>? logChannel;
	private readonly ILogger<PackingLogBinacleService> logger;

	public PackingLogBinacleService(
		IBinacleService inner,
		IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>> logChannel,
		ILogger<PackingLogBinacleService> logger
	)
	{
		this.inner = inner;
		this.logChannel = logChannel.Value;
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
		var result = await this.inner.SingleBinAsync(algorithm, bin, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(
			[bin], items, parameters, (result.AlgorithmInfo.GetAlgorithmIdentifierName(), result), this.logger
		);
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
		var result = await this.inner.SingleBinAsync(bin, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(
			[bin], items, parameters, (result.AlgorithmInfo.GetAlgorithmIdentifierName(), result), this.logger
		);
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
		var results = await this.inner.MultipleBinsAsync(algorithm, bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
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
		var results = await this.inner.MultipleBinsAsync(bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, results, this.logger);
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
		var result = await this.inner.SmallestBinAsync(algorithm, bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, (result.Bin.ID, result), this.logger);
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
		var result = await this.inner.SmallestBinAsync(bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, (result.Bin.ID, result), this.logger);
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
		var result = await this.inner.BestBinAsync(algorithm, bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, (result.Bin.ID, result), this.logger);
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
		var result = await this.inner.BestBinAsync(bins, items, parameters, cancellationToken);
		await this.logChannel.WriteToChannelAsync(bins, items, parameters, (result.Bin.ID, result), this.logger);
		return result;
	}
}
