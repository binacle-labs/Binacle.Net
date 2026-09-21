using BenchmarkDotNet.Running;

namespace Binacle.Lib.Benchmarks.Algorithms;

internal class Program
{
	static void Main(string[] args)
	{
		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, BenchmarkConfig.Create());
	}
}
