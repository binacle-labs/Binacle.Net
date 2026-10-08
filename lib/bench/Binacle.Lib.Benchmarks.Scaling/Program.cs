namespace Binacle.Lib.Benchmarks.Scaling;

internal class Program
{
	static int Main(string[] args)
		=> BenchmarkProgram.Run(typeof(Program).Assembly, args);
}
