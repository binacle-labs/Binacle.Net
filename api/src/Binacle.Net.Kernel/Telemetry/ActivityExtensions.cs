using System.Diagnostics;

namespace Binacle.Net.Kernel.Telemetry;

public static class ActivityExtensions
{
	public static void RecordError(this Activity? activity, Exception exception)
	{
		if (activity is null)
		{
			return;
		}

		activity.SetStatus(ActivityStatusCode.Error, exception.Message);
		activity.SetTag(TelemetryNames.ErrorType, TelemetryNames.ErrorTypeOf(exception));
		activity.AddException(exception);
	}
}
