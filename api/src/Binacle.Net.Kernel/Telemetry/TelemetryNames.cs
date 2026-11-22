namespace Binacle.Net.Kernel.Telemetry;

public static class TelemetryNames
{
	public const string Prefix = "Binacle.Net";

	// Matches the meter and trace source each module names after itself.
	public const string Wildcard = Prefix + "*";

	public const string ErrorType = "error.type";

	public static string ErrorTypeOf(Exception exception) => exception.GetType().FullName ?? exception.GetType().Name;
}
