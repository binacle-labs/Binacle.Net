using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace Binacle.Benchmarking;

// Every bench project's Main. BDN exits 0 on a bad job, a filter that matches nothing or a failed case, so
// the exit code is worked out here.
public static class BenchmarkProgram
{
	public static int Run(Assembly assembly, string[] args, IConfig? config = null)
	{
		var summaries = BenchmarkSwitcher
			.FromAssembly(assembly)
			.Run(args, config ?? BenchmarkConfig.Create())
			.ToList();

		if (summaries.Count == 0)
		{
			Console.Error.WriteLine("Nothing ran. Check the job and the filter above.");
			return 1;
		}

		if (summaries.Any(s => s.HasCriticalValidationErrors || s.Reports.Any(r => !r.Success)))
		{
			Console.Error.WriteLine("At least one case failed. Its row shows NA.");
			return 1;
		}

		return 0;
	}
}
