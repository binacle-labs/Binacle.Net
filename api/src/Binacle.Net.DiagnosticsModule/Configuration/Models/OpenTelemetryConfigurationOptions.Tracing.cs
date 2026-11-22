using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryTracingConfigurationOptions : IOpenTelemetryAttributes
{
	public string[]? AdditionalSources { get; set; }
	[Obsolete("Deprecated in 3.2.0 and removed in 4.0.0. Use OpenTelemetry.AdditionalAttributes, which applies to every signal.")]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
