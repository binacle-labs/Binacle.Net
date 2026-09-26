namespace Binacle.ViPaq.Benchmarks;

internal class Program
{
	static int Main(string[] args)
		=> BenchmarkProgram.Run(typeof(Program).Assembly, args, BenchmarkConfig.Create().AddColumn(PackColumns.All));
}
