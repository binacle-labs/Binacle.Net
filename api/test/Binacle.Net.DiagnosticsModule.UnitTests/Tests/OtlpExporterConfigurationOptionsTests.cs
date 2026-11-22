using Binacle.Net.DiagnosticsModule.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures the OTLP endpoint for each signal is built as expected")]
public class OtlpExporterConfigurationOptionsTests
{
	private static OtlpExporterConfigurationOptions OptionsWith(string? endpoint, string? protocol = "http/protobuf")
		=> new() { Endpoint = endpoint, Protocol = protocol };

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void No_endpoint_in_config_gives_no_endpoint(string? endpoint)
	{
		var options = OptionsWith(endpoint);

		options.GetMetricsEndpoint().ShouldBeNull();
		options.GetTracingEndpoint().ShouldBeNull();
		options.GetLoggingEndpoint().ShouldBeNull();
	}

	[Theory]
	[InlineData(null, "http://collector:4317/")]
	[InlineData("grpc", "http://collector:4317/")]
	[InlineData("grpc", "http://collector:4317/smth/")]
	public void Grpc_uses_the_endpoint_as_given(string? protocol, string endpoint)
	{
		var options = OptionsWith(endpoint, protocol);

		options.GetMetricsEndpoint()!.AbsoluteUri.ShouldBe(endpoint);
		options.GetTracingEndpoint()!.AbsoluteUri.ShouldBe(endpoint);
		options.GetLoggingEndpoint()!.AbsoluteUri.ShouldBe(endpoint);
	}

	[Fact]
	public void Http_gives_each_signal_its_own_path()
	{
		var options = OptionsWith("http://collector:4318");

		options.GetMetricsEndpoint()!.AbsoluteUri.ShouldBe("http://collector:4318/v1/metrics");
		options.GetTracingEndpoint()!.AbsoluteUri.ShouldBe("http://collector:4318/v1/traces");
		options.GetLoggingEndpoint()!.AbsoluteUri.ShouldBe("http://collector:4318/v1/logs");
	}

	[Theory]
	[InlineData("http://collector:4318/", "http://collector:4318/v1/traces")]
	[InlineData("http://collector:4318/smth/", "http://collector:4318/smth/v1/traces")]
	[InlineData("http://collector:4318/smth", "http://collector:4318/smth/v1/traces")]
	[InlineData("http://collector:4318/smth/?k=v", "http://collector:4318/smth/v1/traces?k=v")]
	public void Http_keeps_any_path_already_there(string endpoint, string expected)
	{
		OptionsWith(endpoint).GetTracingEndpoint()!.AbsoluteUri.ShouldBe(expected);
	}

	[Fact]
	public void The_old_http_spelling_also_adds_the_signal_path()
	{
		OptionsWith("http://collector:4318", "httpProtobuf")
			.GetTracingEndpoint()!.AbsoluteUri.ShouldBe("http://collector:4318/v1/traces");
	}
}
