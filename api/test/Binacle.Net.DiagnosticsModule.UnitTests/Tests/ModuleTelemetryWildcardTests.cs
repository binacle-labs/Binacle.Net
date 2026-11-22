using System.Diagnostics;
using System.Diagnostics.Metrics;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// Each module names its meter and source after itself, so the setup must pick them up without naming them.
[Trait("Behavioral Tests", "Ensures every module's meter and trace source is collected by the wildcard")]
public class ModuleTelemetryWildcardTests
{
	[Theory]
	[InlineData("Binacle.Net")]
	[InlineData("Binacle.Net.DiagnosticsModule")]
	[InlineData("Binacle.Net.ServiceModule")]
	public void A_module_meter_is_collected(string meterName)
	{
		var exported = new List<Metric>();
		using (var provider = Build(metrics => metrics.AddReader(Collecting(exported))))
		{
			var meterProvider = provider.GetRequiredService<MeterProvider>();
			using var meter = new Meter(meterName);
			meter.CreateCounter<long>("wildcard.check").Add(1);
			meterProvider.ForceFlush();
		}

		exported.ShouldContain(x => x.MeterName == meterName && x.Name == "wildcard.check");
	}

	[Fact]
	public void A_meter_outside_binacle_is_not_collected()
	{
		var exported = new List<Metric>();
		using (var provider = Build(metrics => metrics.AddReader(Collecting(exported))))
		{
			var meterProvider = provider.GetRequiredService<MeterProvider>();
			using var meter = new Meter("Other.Library");
			meter.CreateCounter<long>("wildcard.check").Add(1);
			meterProvider.ForceFlush();
		}

		exported.ShouldNotContain(x => x.MeterName == "Other.Library");
	}

	[Theory]
	[InlineData("Binacle.Net", true)]
	[InlineData("Binacle.Net.DiagnosticsModule", true)]
	[InlineData("Binacle.Net.ServiceModule", true)]
	[InlineData("Other.Library", false)]
	public void A_module_source_is_traced(string sourceName, bool traced)
	{
		using var provider = Build(_ => { });
		_ = provider.GetRequiredService<TracerProvider>();

		using var source = new ActivitySource(sourceName);
		source.HasListeners().ShouldBe(traced);
	}

	private static ServiceProvider Build(Action<MeterProviderBuilder> addReader)
	{
		var services = new ServiceCollection();
		services.AddLogging();
		var otel = services.AddOpenTelemetry().WithMetrics(addReader);
		new BinacleOpenTelemetryBuilder(otel, Serilog.Core.Logger.None)
			.WithMetrics(new OpenTelemetryMetricsConfigurationOptions())
			.WithTracing(new OpenTelemetryTracingConfigurationOptions());
		return services.BuildServiceProvider();
	}

	private static MetricReader Collecting(List<Metric> exported)
		=> new BaseExportingMetricReader(new CollectingExporter(exported))
		{
			TemporalityPreference = MetricReaderTemporalityPreference.Cumulative
		};

	private sealed class CollectingExporter(List<Metric> exported) : BaseExporter<Metric>
	{
		public override ExportResult Export(in Batch<Metric> batch)
		{
			foreach (var metric in batch)
			{
				exported.Add(metric);
			}
			return ExportResult.Success;
		}
	}
}
