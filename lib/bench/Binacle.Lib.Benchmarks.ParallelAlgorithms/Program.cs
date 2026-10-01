namespace Binacle.Lib.Benchmarks.ParallelAlgorithms;

internal class Program
{
	static int Main(string[] args)
	{
		var config = JobsByCoreCount.CreateConfig(args, out var rest);
		if (config is null)
		{
			Console.Error.WriteLine("Unknown --job. Use dry, short, medium, long, verylong or default.");
			return 1;
		}

		return BenchmarkProgram.Run(typeof(Program).Assembly, rest, config);
	}
}
