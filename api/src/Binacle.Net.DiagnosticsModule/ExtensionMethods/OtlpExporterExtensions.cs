using Binacle.Net.DiagnosticsModule.Configuration.Models;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class OtlpExporterExtensions
{
	public static BinacleOpenTelemetryBuilder UseOtlpExporter(
		this BinacleOpenTelemetryBuilder builder,
		OtlpExporterConfigurationOptions otlp
	)
	{
		if (!otlp.Enabled)
		{
			return builder;
		}

		builder.Logger.Information("Using {Provider} with OpenTelemetry.", "Otlp Exporter");

		if (otlp.Metrics && otlp.Tracing && otlp.Logging)
		{
			UseOneExporterForAllSignals(builder.Builder, otlp);
			return builder;
		}

		if (!otlp.Metrics && !otlp.Tracing && !otlp.Logging)
		{
			builder.Logger.Warning("OTLP is enabled but Metrics, Tracing and Logging are all off. Nothing is sent over OTLP.");
			return builder;
		}

		AddExporterPerSignal(builder.Builder, otlp);
		return builder;
	}

	// Only UseOtlpExporter reads the per-signal OTEL_EXPORTER_OTLP_<SIGNAL>_* variables.
	private static void UseOneExporterForAllSignals(OpenTelemetryBuilder builder, OtlpExporterConfigurationOptions otlp)
	{
		if (string.IsNullOrEmpty(otlp.Endpoint))
		{
			// Endpoint and protocol come from the OTEL_EXPORTER_OTLP_* environment variables.
			builder.UseOtlpExporter();
			return;
		}

		builder.UseOtlpExporter(otlp.GetOtlpExportProtocol(), new Uri(otlp.Endpoint));
	}

	private static void AddExporterPerSignal(OpenTelemetryBuilder builder, OtlpExporterConfigurationOptions otlp)
	{
		if (otlp.Metrics)
		{
			builder.WithMetrics(metrics =>
				metrics.AddOtlpExporter(options => options.ApplyConfigEndpoint(otlp, otlp.GetMetricsEndpoint()))
			);
		}

		if (otlp.Tracing)
		{
			builder.WithTracing(tracing =>
				tracing.AddOtlpExporter(options => options.ApplyConfigEndpoint(otlp, otlp.GetTracingEndpoint()))
			);
		}

		if (otlp.Logging)
		{
			builder.WithLogging(logging =>
				logging.AddOtlpExporter(options => options.ApplyConfigEndpoint(otlp, otlp.GetLoggingEndpoint()))
			);
		}
	}

	private static void ApplyConfigEndpoint(
		this OtlpExporterOptions options,
		OtlpExporterConfigurationOptions otlp,
		Uri? endpoint
	)
	{
		// With no endpoint in config, the SDK reads OTEL_EXPORTER_OTLP_ENDPOINT and _PROTOCOL itself.
		if (endpoint is null)
		{
			return;
		}

		options.Protocol = otlp.GetOtlpExportProtocol();
		options.Endpoint = endpoint;
	}
}
