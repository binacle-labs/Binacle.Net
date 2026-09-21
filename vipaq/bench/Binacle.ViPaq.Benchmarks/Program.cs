using BenchmarkDotNet.Running;

namespace Binacle.ViPaq.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, BenchmarkConfig.Create());
	}
}
