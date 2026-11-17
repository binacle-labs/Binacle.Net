using Binacle.Net.DiagnosticsModule.Configuration.Models;
using OpenTelemetry;
using OpenTelemetry.Resources;
using Serilog;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class OpenTelemetryConfigurationExtensions
{
	public static BinacleOpenTelemetryBuilder WithLogger(this OpenTelemetryBuilder builder, ILogger logger)
	{
		return new BinacleOpenTelemetryBuilder(builder, logger);
	}

	public static BinacleOpenTelemetryBuilder ConfigureResource(
		this BinacleOpenTelemetryBuilder builder,
		OpenTelemetryConfigurationOptions options
	)
	{
		builder.Builder.ConfigureResource(resource => resource
			.AddService(
				serviceName: "Binacle.Net",
				serviceVersion: Metadata.Version,
				serviceNamespace: options.ServiceNamespace,
				serviceInstanceId: options.ServiceInstanceId
			)
			.AddHostAndProcess()
			// Read again so OTEL_RESOURCE_ATTRIBUTES and OTEL_SERVICE_NAME beat the values above.
			.AddEnvironmentVariableDetector()
			.AddOptionalAdditionalAttributes(options)
		);

		return builder;
	}

	public static ResourceBuilder AddHostAndProcess(this ResourceBuilder builder)
	{
		var attributes = new Dictionary<string, object>
		{
			["host.name"] = Environment.MachineName,
			["process.pid"] = Environment.ProcessId
		};

		return builder.AddAttributes(attributes);
	}

	public static ResourceBuilder AddOptionalAdditionalAttributes(
		this ResourceBuilder builder,
		IOpenTelemetryAttributes model
	)
	{
		if (HasAttributes(model))
		{
			builder.AddAttributes(model.AdditionalAttributes!);
		}

		return builder;
	}

	public static void WarnIfDeprecatedAttributes(
		this BinacleOpenTelemetryBuilder builder,
		IOpenTelemetryAttributes signal,
		string signalName
	)
	{
		if (!HasAttributes(signal))
		{
			return;
		}

		var sectionName = OpenTelemetryConfigurationOptions.SectionName;

		builder.Logger.Warning(
			"{Setting:l} is deprecated and will be removed in 4.0.0. Move its values to {Replacement:l}, which applies to every signal.",
			$"{sectionName}:{signalName}:AdditionalAttributes",
			$"{sectionName}:AdditionalAttributes"
		);
	}

	private static bool HasAttributes(IOpenTelemetryAttributes model)
	{
		return model.AdditionalAttributes?.Count > 0;
	}
}
