namespace Binacle.Lib;

// The two numbers every parallel processor needs, in one place because all three worked them out differently.
internal static class ParallelLimits
{
	// What ParallelOptions.MaxDegreeOfParallelism means by "as many as the scheduler offers".
	internal const int NoLimit = -1;

	// Never ask for more parallelism than there is work, or than the machine can run. This is the default the
	// processors use: it costs nothing when the caller already picked well, and it stops the pathological case
	// where Parallel.For hands work to a thread that cannot run - one work item, or one CPU - which measured
	// many times dearer than not parallelising at all.
	//
	// It is a cap, not a decision. Whether to parallelise at all is the processor factory's call.
	internal static int Degree(int workItems)
	{
		var degree = Math.Min(workItems, Environment.ProcessorCount);

		// ParallelOptions throws on zero, and an empty request reaches here.
		return Math.Max(1, degree);
	}

	// The lock count for a result dictionary. One writer per work item, never more than the threads that can
	// actually run at once. ConcurrentDictionary's own default is the CPU count, which is far too many for a
	// race of two or three: the locks cost memory, and every operation that takes all of them gets dearer.
	internal static int ConcurrencyLevel(int workItems, int maxDegreeOfParallelism)
	{
		var threads = maxDegreeOfParallelism > 0
			? Math.Min(Environment.ProcessorCount, maxDegreeOfParallelism)
			: Environment.ProcessorCount;
		var writers = Math.Min(workItems, threads);

		// ConcurrentDictionary throws below 1, and an empty request reaches here.
		return Math.Max(1, writers);
	}
}
