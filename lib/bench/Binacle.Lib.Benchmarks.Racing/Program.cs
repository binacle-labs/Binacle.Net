namespace Binacle.Lib.Benchmarks.Racing;

internal class Program
{
	static int Main(string[] args)
		=> BenchmarkProgram.Run(typeof(Program).Assembly, args);
}
