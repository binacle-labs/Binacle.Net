using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures deprecated per-signal attributes log a startup warning")]
public class OpenTelemetryDeprecatedAttributesTests
{
	private readonly CapturingSink sink = new();

	[Fact]
	public void No_per_signal_attributes_logs_no_warning()
	{
		this.Configure(new OpenTelemetryConfigurationOptions());

		this.sink.Warnings.ShouldBeEmpty();
	}

	[Fact]
	public void An_empty_set_of_attributes_logs_no_warning()
	{
		var options = new OpenTelemetryConfigurationOptions();
		SetAttributes(options.Metrics, new Dictionary<string, object>());

		this.Configure(options);

		this.sink.Warnings.ShouldBeEmpty();
	}

	[Fact]
	public void Only_the_signal_with_attributes_is_named()
	{
		var options = new OpenTelemetryConfigurationOptions();
		SetAttributes(options.Metrics, OneAttribute());

		this.Configure(options);

		this.sink.Warnings.Count.ShouldBe(1);
		this.sink.Warnings[0].ShouldStartWith("OpenTelemetry:Metrics:AdditionalAttributes is deprecated");
	}

	[Fact]
	public void Every_signal_with_attributes_is_named()
	{
		var options = new OpenTelemetryConfigurationOptions();
		SetAttributes(options.Metrics, OneAttribute());
		SetAttributes(options.Tracing, OneAttribute());
		SetAttributes(options.Logging, OneAttribute());

		this.Configure(options);

		this.sink.Warnings.Count.ShouldBe(3);
		this.sink.Warnings[0].ShouldStartWith("OpenTelemetry:Metrics:AdditionalAttributes is deprecated");
		this.sink.Warnings[1].ShouldStartWith("OpenTelemetry:Tracing:AdditionalAttributes is deprecated");
		this.sink.Warnings[2].ShouldStartWith("OpenTelemetry:Logging:AdditionalAttributes is deprecated");
	}

	private void Configure(OpenTelemetryConfigurationOptions options)
	{
		var logger = new LoggerConfiguration()
			.WriteTo.Sink(this.sink)
			.CreateLogger();

		new ServiceCollection()
			.AddOpenTelemetry()
			.WithLogger(logger)
			.WithMetrics(options.Metrics)
			.WithTracing(options.Tracing)
			.WithLogging(options.Logging);
	}

	// Set through the interface so the [Obsolete] property raises no build warning.
	private static void SetAttributes(IOpenTelemetryAttributes signal, Dictionary<string, object> attributes)
	{
		signal.AdditionalAttributes = attributes;
	}

	private static Dictionary<string, object> OneAttribute()
	{
		return new Dictionary<string, object> { ["team"] = "packing" };
	}
}
