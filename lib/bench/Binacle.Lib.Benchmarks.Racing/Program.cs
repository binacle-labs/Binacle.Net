namespace Binacle.Lib.Benchmarks.Racing;

internal class Program
{
	static int Main(string[] args)
	{
		var config = CoreJobs.CreateConfig(args, out var rest);
		if (config is null)
		{
			Console.Error.WriteLine("Unknown --job. Use dry, short, medium, long, verylong or default.");
			return 1;
		}

		return BenchmarkProgram.Run(typeof(Program).Assembly, rest, config);
	}
}
