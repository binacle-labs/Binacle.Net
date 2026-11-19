using System.Diagnostics;
using Binacle.Net;
using Binacle.Net.DiagnosticsModule.IntegrationTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

[assembly: AssemblyFixture(typeof(BinacleApi))]

namespace Binacle.Net.DiagnosticsModule.IntegrationTests;

public sealed class BinacleApi : WebApplicationFactory<IApiMarker>
{
	public const string HealthCheckPath = "/_health";

	private static readonly Dictionary<string, string?> ConfigurationValues = new()
	{
		// OpenTelemetry is set up only when an exporter is on. All three signals off: nothing leaves the process.
		{ "OpenTelemetry:Otlp:Enabled", bool.TrueString },
		{ "OpenTelemetry:Otlp:Metrics", bool.FalseString },
		{ "OpenTelemetry:Otlp:Tracing", bool.FalseString },
		{ "OpenTelemetry:Otlp:Logging", bool.FalseString },
		{ "PackingLogs:Enabled", bool.TrueString },
		{ "HealthChecks:Enabled", bool.TrueString },
		{ "HealthChecks:Path", HealthCheckPath },
		// The packing call logs at Debug.
		{ "Serilog:MinimumLevel:Override:Binacle.Net", "Debug" },
	};

	public BinacleApi()
	{
		this.Client = this.CreateClient();
	}

	public HttpClient Client { get; }

	public ExportedItems<Activity> Spans { get; } = new();
	public ExportedItems<MetricSnapshot> Metrics { get; } = new();
	public ExportedItems<LogRecord> Logs { get; } = new();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(ConfigurationValues)
			.Build();

		builder
			.UseEnvironment("Test")
			// Reaches Program.cs as args, so it beats every config file while the modules register.
			.UseConfiguration(configuration);

		builder.ConfigureTestServices(services =>
		{
			services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddInMemoryExporter(this.Spans));
			services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddInMemoryExporter(this.Metrics));
			services.ConfigureOpenTelemetryLoggerProvider(logging => logging.AddInMemoryExporter(this.Logs));
		});
	}

	public void FlushMetrics()
	{
		this.Services.GetRequiredService<MeterProvider>().ForceFlush();
	}
}
