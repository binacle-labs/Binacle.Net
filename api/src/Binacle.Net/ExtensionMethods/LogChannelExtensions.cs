using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.Telemetry;

namespace Binacle.Net.ExtensionMethods;

internal static class LogChannelExtensions
{
	internal static ValueTask WriteToChannelAsync<TBin, TBox, TParams>(
		this Channel<AlgorithmOperationLogChannelRequest>? logChannel,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		(string Key, OperationResult Result) result,
		ILogger? logger = null
	)
		where TBin : class, IIdentifiableBin
		where TBox : class, IIdentifiableItem
		where TParams : class, ILogParametersProvider
	{
		if (logChannel is null)
		{
			return ValueTask.CompletedTask;
		}

		return logChannel.WriteToChannelAsync(
			bins,
			items,
			parameters,
			new Dictionary<string, OperationResult> { { result.Key, result.Result } },
			logger
		);
	}

	internal static ValueTask WriteToChannelAsync<TBin, TBox, TParams>(
		this Channel<AlgorithmOperationLogChannelRequest>? logChannel,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		IDictionary<string, OperationResult> results,
		ILogger? logger = null
	)
		where TBin : class, IIdentifiableBin
		where TBox : class, IIdentifiableItem
		where TParams : class, ILogParametersProvider
	{
		if (logChannel is null)
		{
			return ValueTask.CompletedTask;
		}

		using var channelActivity = Diagnostics.ActivitySource.StartActivity("queue packing_log");

		try
		{
			// False once the packing log has stopped. Its meter counts that, so it is not logged per request.
			logChannel.Writer.TryWrite(AlgorithmOperationLogChannelRequest.From(bins, items, parameters, results));
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error while writing to channel");
		}

		return ValueTask.CompletedTask;
	}
}
