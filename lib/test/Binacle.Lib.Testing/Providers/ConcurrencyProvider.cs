namespace Binacle.Lib.Testing.Providers;

public static class ConcurrencyProvider
{
	public static int[] GetProcessorCount() =>
		[Environment.ProcessorCount];
}
