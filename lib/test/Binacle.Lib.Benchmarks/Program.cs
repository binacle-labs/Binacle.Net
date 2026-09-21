using BenchmarkDotNet.Running;
using Binacle.Benchmarking;

namespace Binacle.Lib.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, BenchmarkConfig.Create());
	}
}
