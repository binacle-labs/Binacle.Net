namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryLoggingConfigurationOptions : IOpenTelemetryAttributes
{
	[Obsolete("Deprecated in 3.2.0 and removed in 4.0.0. Use OpenTelemetry.AdditionalAttributes, which applies to every signal.")]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
