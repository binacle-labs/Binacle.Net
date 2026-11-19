using Binacle.Net.DiagnosticsModule.Configuration.Models;
using OpenTelemetry.Logs;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class OpenTelemetryLoggingExtensions
{
	public static BinacleOpenTelemetryBuilder WithLogging(
		this BinacleOpenTelemetryBuilder builder,
		OpenTelemetryLoggingConfigurationOptions options
	)
	{
		builder.WarnIfDeprecatedAttributes(options, "Logging");

		builder.Builder.WithLogging(
			logging => logging.ConfigureResource(resource => resource.AddOptionalAdditionalAttributes(options)),
			// Without this the body is the template, with {placeholders}, not the rendered message.
			loggerOptions => loggerOptions.IncludeFormattedMessage = true
		);

		return builder;
	}
}
