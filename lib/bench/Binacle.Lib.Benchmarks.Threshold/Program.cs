namespace Binacle.Lib.Benchmarks.Threshold;

internal class Program
{
	static int Main(string[] args)
		=> BenchmarkProgram.Run(typeof(Program).Assembly, args);
}
