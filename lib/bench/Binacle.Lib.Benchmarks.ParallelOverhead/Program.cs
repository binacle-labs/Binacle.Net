namespace Binacle.Lib.Benchmarks.ParallelOverhead;

internal class Program
{
	// One CPU too, and not the cheap end: Parallel.For still hands the work to a worker thread, which on one
	// CPU waits for the caller to be taken off it. Measured dearer than any other count.
	private static readonly int[] CoreCounts = [1, 2, 4, 8, 12];

	static int Main(string[] args)
	{
		var config = JobsByCoreCount.CreateConfig(args, out var rest, CoreCounts);
		if (config is null)
		{
			Console.Error.WriteLine($"Bad --job or --launches. {JobOption.Usage}");
			return 1;
		}

		return BenchmarkProgram.Run(typeof(Program).Assembly, rest, config);
	}
}
