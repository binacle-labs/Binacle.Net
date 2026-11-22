using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.DiagnosticsModule.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class AzureMonitorExtensions
{
	public static BinacleOpenTelemetryBuilder UseAzureMonitor(
		this BinacleOpenTelemetryBuilder builder,
		AzureMonitorConfigurationOptions options
	)
	{
		if (!options.Enabled)
		{
			return builder;
		}

		builder.Logger.Information("Using {Provider} with OpenTelemetry.", "Azure Monitor");

		builder.Builder.UseAzureMonitor(azureMonitorOptions =>
		{
			if (!string.IsNullOrEmpty(options.ConnectionString))
			{
				azureMonitorOptions.ConnectionString = options.ConnectionString;
			}

			azureMonitorOptions.SamplingRatio = options.SamplingRatio;
			azureMonitorOptions.EnableLiveMetrics = options.EnableLiveMetrics;
		});

		return builder;
	}
}
