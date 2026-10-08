namespace Binacle.Lib.Benchmarks.Algorithms;

internal class Program
{
	static int Main(string[] args)
		=> BenchmarkProgram.Run(typeof(Program).Assembly, args);
}
