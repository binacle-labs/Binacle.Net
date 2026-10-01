namespace Binacle.Lib.Benchmarks.ParallelOverhead;

internal class Program
{
	// One CPU as well as the counts JobsByCoreCount runs everywhere else. It is not the cheap end: Parallel.For
	// still hands the work to a worker thread, and on one CPU that worker has to wait for the caller to be
	// taken off it, which measured dearer than any other count.
	private static readonly int[] CoreCounts = [1, 2, 4, 8, 12];

	static int Main(string[] args)
	{
		var config = JobsByCoreCount.CreateConfig(args, out var rest, CoreCounts);
		if (config is null)
		{
			Console.Error.WriteLine($"Unknown --job. Use {JobOption.Names}.");
			return 1;
		}

		return BenchmarkProgram.Run(typeof(Program).Assembly, rest, config);
	}
}
