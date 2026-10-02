namespace Binacle.ViPaq.Benchmarks;

internal class Program
{
	static int Main(string[] args)
	{
		var config = JobOption.CreateConfig(args, out var rest);
		if (config is null)
		{
			Console.Error.WriteLine($"Bad --job or --launches. {JobOption.Usage}");
			return 1;
		}

		return BenchmarkProgram.Run(typeof(Program).Assembly, rest, config.AddColumn(PackColumns.All));
	}
}
