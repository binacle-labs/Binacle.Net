using BenchmarkDotNet.Running;

namespace Binacle.Lib.Benchmarks.ResultSelection;

internal class Program
{
	static void Main(string[] args)
	{
		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, BenchmarkConfig.Create());
	}
}
