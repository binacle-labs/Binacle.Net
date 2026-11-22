namespace Binacle.Net.Kernel.Telemetry;

public static class HistogramBuckets
{
	// The SDK's default buckets are sized for milliseconds.
	public static readonly IReadOnlyList<double> Seconds =
		[0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];
}
