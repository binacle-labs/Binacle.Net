using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.DiagnosticsModule.Telemetry;
using Binacle.Net.Kernel.Logs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorServiceCollectionExtensions
{
	public static void AddOptionsBasedPackingLogProcessor(this IServiceCollection services)
	{
		services.AddSingleton<PackingLogMetrics>();
		services.AddLogProcessor<AlgorithmOperationLogChannelRequest, PackingLogEntry>(
			optionsFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>().Value;
				var metrics = sp.GetRequiredService<PackingLogMetrics>();

				return new LogsProcessorOptions<AlgorithmOperationLogChannelRequest>
				{
					Path = options.Path!,
					FileNameFormat = options.FileName!,
					DateFormat = options.DateFormat!,
					RetentionDays = options.RetentionDays,
					OnWritten = metrics.EntryWritten,
					OnWriteFailed = metrics.EntryFailed,
					OnDiscarded = metrics.EntryDiscarded,
				};
			},
			channelFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>().Value;
				var metrics = sp.GetRequiredService<PackingLogMetrics>();

				int? capacity = options.ChannelLimit is > 0 ? options.ChannelLimit : null;
				Channel<AlgorithmOperationLogChannelRequest> channel;
				if (capacity is not null)
				{
					channel = Channel.CreateBounded<AlgorithmOperationLogChannelRequest>(
						new BoundedChannelOptions(capacity.Value)
						{
							FullMode = BoundedChannelFullMode.DropWrite,
							SingleReader = true,
							SingleWriter = false,
							AllowSynchronousContinuations = false
						},
						_ => metrics.EntryDropped()
					);
				}
				else
				{
					channel = Channel.CreateUnbounded<AlgorithmOperationLogChannelRequest>(
						new UnboundedChannelOptions
						{
							// A single-reader unbounded channel cannot count, and queue.size needs Reader.Count.
							SingleReader = false,
							SingleWriter = false,
							AllowSynchronousContinuations = false
						});
				}

				metrics.ObserveQueue(channel, capacity);

				return new RefusalCountingChannel<AlgorithmOperationLogChannelRequest>(channel, metrics.EntryDiscarded);
			});
	}
}
