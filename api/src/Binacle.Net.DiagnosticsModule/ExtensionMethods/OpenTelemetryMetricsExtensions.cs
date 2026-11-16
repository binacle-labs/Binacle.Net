using Binacle.Net.DiagnosticsModule.Configuration.Models;
using OpenTelemetry.Metrics;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class OpenTelemetryMetricsExtensions
{
	public static BinacleOpenTelemetryBuilder WithMetrics(
		this BinacleOpenTelemetryBuilder builder,
		OpenTelemetryMetricsConfigurationOptions options
	)
	{
		builder.WarnIfDeprecatedAttributes(options, "Metrics");

		builder.Builder.WithMetrics(metrics => metrics
			.ConfigureResource(resource => resource.AddOptionalAdditionalAttributes(options))
			.AddRuntimeInstrumentation()
			.AddAspNetCoreInstrumentation()
			.AddMeters(options.AdditionalMeters)
		);

		return builder;
	}

	public static MeterProviderBuilder AddMeters(
		this MeterProviderBuilder meterProviderBuilder,
		string[]? additionalMeters
	)
	{
		if (additionalMeters is null)
		{
			return meterProviderBuilder;
		}

		foreach (var meter in additionalMeters)
		{
			meterProviderBuilder.AddMeter(meter);
		}

		return meterProviderBuilder;
	}
}
