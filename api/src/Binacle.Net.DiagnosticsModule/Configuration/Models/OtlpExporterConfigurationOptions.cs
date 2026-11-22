using OpenTelemetry.Exporter;

namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class OtlpExporterConfigurationOptions
{
	public bool Enabled { get; set; }
	public string? Endpoint { get; set; }
	public string? Protocol { get; set; }
	public bool Metrics { get; set; } = true;
	public bool Tracing { get; set; } = true;
	public bool Logging { get; set; } = true;
	
	public OtlpExportProtocol GetOtlpExportProtocol()
	{
		if(string.IsNullOrEmpty(this.Protocol))
		{
			return OtlpExportProtocol.Grpc;
		}
		return this.Protocol switch
		{
			"grpc" => OtlpExportProtocol.Grpc,
			"http/protobuf" or "httpProtobuf" => OtlpExportProtocol.HttpProtobuf,
			_ => throw new ArgumentOutOfRangeException(nameof(this.Protocol), this.Protocol, null)
		};
	}

	public Uri? GetMetricsEndpoint() => this.GetSignalEndpoint("v1/metrics");
	public Uri? GetTracingEndpoint() => this.GetSignalEndpoint("v1/traces");
	public Uri? GetLoggingEndpoint() => this.GetSignalEndpoint("v1/logs");

	private Uri? GetSignalEndpoint(string signalPath)
	{
		if (string.IsNullOrEmpty(this.Endpoint))
		{
			return null;
		}

		var endpoint = new Uri(this.Endpoint);
		if (this.GetOtlpExportProtocol() is OtlpExportProtocol.Grpc)
		{
			return endpoint;
		}

		var builder = new UriBuilder(endpoint);
		builder.Path = $"{builder.Path.TrimEnd('/')}/{signalPath}";
		return builder.Uri;
	}
}
