using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Validators;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// Telemetry is off in a default deployment, so the empty section has to stay valid. What must be caught is a
// value the exporters cannot use. Those surface at startup, long before anyone looks at a dashboard.
[Trait("Behavioral Tests", "Ensures OpenTelemetry configuration is validated as expected")]
public class OpenTelemetryConfigurationOptionsValidatorTests
{
	private readonly OpenTelemetryConfigurationOptionsValidator validator = new();

	private static OpenTelemetryConfigurationOptions OptionsWith(
		string? protocol = null,
		float samplingRatio = 1.0f,
		string? endpoint = null,
		bool otlpEnabled = true
	)
		=> new()
		{
			Otlp = new OtlpExporterConfigurationOptions { Protocol = protocol, Endpoint = endpoint, Enabled = otlpEnabled },
			AzureMonitor = new AzureMonitorConfigurationOptions { SamplingRatio = samplingRatio }
		};

	[Theory]
	[InlineData(null)] // unset falls back to grpc in GetOtlpExportProtocol
	[InlineData("")]
	[InlineData("grpc")]
	[InlineData("http/protobuf")]
	[InlineData("httpProtobuf")] // the old spelling, kept so existing config still starts
	public void A_supported_otlp_protocol_is_accepted(string? protocol)
	{
		this.validator.Validate(OptionsWith(protocol: protocol)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("http")]
	[InlineData("HttpProtobuf")] // the check is case sensitive
	[InlineData("gRPC")]
	[InlineData("http/json")]
	public void An_unsupported_otlp_protocol_fails_validation(string protocol)
	{
		this.validator.Validate(OptionsWith(protocol: protocol)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(null, OtlpExportProtocol.Grpc)]
	[InlineData("grpc", OtlpExportProtocol.Grpc)]
	[InlineData("http/protobuf", OtlpExportProtocol.HttpProtobuf)]
	[InlineData("httpProtobuf", OtlpExportProtocol.HttpProtobuf)]
	public void A_supported_otlp_protocol_maps_to_the_exporter_protocol(string? protocol, OtlpExportProtocol expected)
	{
		new OtlpExporterConfigurationOptions { Protocol = protocol }.GetOtlpExportProtocol().ShouldBe(expected);
	}

	[Theory]
	[InlineData(0.1f)]
	[InlineData(0.5f)]
	[InlineData(1.0f)]
	public void A_sampling_ratio_inside_the_range_is_accepted(float samplingRatio)
	{
		this.validator.Validate(OptionsWith(samplingRatio: samplingRatio)).IsValid.ShouldBeTrue();
	}

	// 0 is the tempting value for "sample nothing" and it is rejected. Turn the exporter off instead.
	[Theory]
	[InlineData(0f)]
	[InlineData(0.05f)]
	[InlineData(1.5f)]
	public void A_sampling_ratio_outside_the_range_fails_validation(float samplingRatio)
	{
		this.validator.Validate(OptionsWith(samplingRatio: samplingRatio)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(null)] // unset falls back to the OTEL_EXPORTER_OTLP_* variables
	[InlineData("")]
	[InlineData("http://localhost:4317")]
	[InlineData("https://collector.example.com:4318/smth/")]
	public void A_supported_otlp_endpoint_is_accepted(string? endpoint)
	{
		this.validator.Validate(OptionsWith(endpoint: endpoint)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("localhost:4317")] // parses as a URI with the scheme "localhost"
	[InlineData("collector:4317")]
	[InlineData("/v1/traces")]
	[InlineData("ftp://localhost:4317")]
	[InlineData("not a url")]
	[InlineData(" ")]
	public void An_unsupported_otlp_endpoint_fails_validation(string endpoint)
	{
		this.validator.Validate(OptionsWith(endpoint: endpoint)).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void An_unsupported_otlp_endpoint_is_accepted_when_otlp_is_off()
	{
		this.validator.Validate(OptionsWith(endpoint: "localhost:4317", otlpEnabled: false)).IsValid.ShouldBeTrue();
	}

	// The module reads these options before ValidateOnStart runs, and new Uri() would throw first.
	[Fact]
	public void ThrowIfInvalid_names_the_bad_endpoint()
	{
		var exception = Should.Throw<OptionsValidationException>(() =>
			OpenTelemetryConfigurationOptionsValidator.ThrowIfInvalid(OptionsWith(endpoint: "localhost:4317"))
		);

		exception.Message.ShouldContain("Otlp.Endpoint");
	}

	[Fact]
	public void ThrowIfInvalid_does_not_throw_on_valid_options()
	{
		Should.NotThrow(() =>
			OpenTelemetryConfigurationOptionsValidator.ThrowIfInvalid(OptionsWith(endpoint: "http://localhost:4317"))
		);
	}
}
