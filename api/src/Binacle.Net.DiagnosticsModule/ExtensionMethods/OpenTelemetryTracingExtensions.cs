using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Trace;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class OpenTelemetryTracingExtensions
{
	public static BinacleOpenTelemetryBuilder WithTracing(
		this BinacleOpenTelemetryBuilder builder,
		OpenTelemetryTracingConfigurationOptions options
	)
	{
		builder.WarnIfDeprecatedAttributes(options, "Tracing");

		builder.Builder.WithTracing(tracing => tracing
			.ConfigureResource(resource => resource.AddOptionalAdditionalAttributes(options))
			.AddAspNetCoreInstrumentation(instrumentation => instrumentation.ConfigureAspNetCoreInstrumentation(options))
			.AddHttpClientInstrumentation(instrumentation => instrumentation.ConfigureHttpClientInstrumentation(options))
			.AddSource(Diagnostics.ActivitySource.Name)
			.AddSources(options.AdditionalSources)
		);

		return builder;
	}

	public static TracerProviderBuilder AddSources(
		this TracerProviderBuilder tracerProviderBuilder,
		string[]? additionalSources
	)
	{
		if (additionalSources is null)
		{
			return tracerProviderBuilder;
		}

		foreach (var source in additionalSources)
		{
			tracerProviderBuilder.AddSource(source);
		}

		return tracerProviderBuilder;
	}

	public static void ConfigureAspNetCoreInstrumentation(
		this AspNetCoreTraceInstrumentationOptions options,
		OpenTelemetryTracingConfigurationOptions tracingConfigurationOptions
	)
	{
		options.Filter = context => !IsHealthCheckRequest(context);
	}

	public static void ConfigureHttpClientInstrumentation(
		this HttpClientTraceInstrumentationOptions options,
		OpenTelemetryTracingConfigurationOptions tracingConfigurationOptions
	)
	{
		// Empty on purpose. The seam is wired, so the first setting is an edit here, not at the call site.
	}

	// Read per request: the health-check options are registered after tracing is wired.
	private static bool IsHealthCheckRequest(HttpContext context)
	{
		var healthChecks = context.RequestServices
			.GetRequiredService<IOptions<HealthCheckConfigurationOptions>>()
			.Value;

		if (!healthChecks.Enabled)
		{
			return false;
		}

		return context.Request.Path.StartsWithSegments(healthChecks.Path);
	}
}
