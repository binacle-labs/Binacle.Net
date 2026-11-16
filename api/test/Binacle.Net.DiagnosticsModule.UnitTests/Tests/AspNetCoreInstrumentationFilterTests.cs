using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures health-check requests are left out of traces")]
public class AspNetCoreInstrumentationFilterTests
{
	[Theory]
	[InlineData(true, "/_health", false)]
	[InlineData(true, "/_health/live", false)]
	[InlineData(true, "/_healthy", true)]
	[InlineData(true, "/api/v3/pack", true)]
	[InlineData(false, "/_health", true)]
	public void Only_health_check_requests_are_left_out(bool healthChecksEnabled, string requestPath, bool traced)
	{
		var options = new AspNetCoreTraceInstrumentationOptions();
		options.ConfigureAspNetCoreInstrumentation(new OpenTelemetryTracingConfigurationOptions());

		var context = RequestTo(requestPath, healthChecksEnabled);

		options.Filter!(context).ShouldBe(traced);
	}

	private static DefaultHttpContext RequestTo(string path, bool healthChecksEnabled)
	{
		var healthChecks = new HealthCheckConfigurationOptions
		{
			Enabled = healthChecksEnabled,
			Path = "/_health"
		};

		var services = new ServiceCollection();
		services.AddSingleton(Options.Create(healthChecks));

		var context = new DefaultHttpContext
		{
			RequestServices = services.BuildServiceProvider()
		};
		context.Request.Path = path;

		return context;
	}
}
