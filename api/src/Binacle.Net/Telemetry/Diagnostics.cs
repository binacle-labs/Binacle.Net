using System.Diagnostics;
using Binacle.Net.Kernel.Telemetry;

namespace Binacle.Net.Telemetry;

internal static class Diagnostics
{
	public const string Name = TelemetryNames.Prefix;

	public static readonly ActivitySource ActivitySource = new(Name);
}
