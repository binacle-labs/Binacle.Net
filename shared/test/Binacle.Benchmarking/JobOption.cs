using BenchmarkDotNet.Jobs;

namespace Binacle.Benchmarking;

// The --job option every bench project takes: the names it accepts, and the BDN job each one means.
//
// BDN adds a CLI --job beside the config's jobs instead of applying it to them, so the option is taken out of
// the args here and the jobs a config builds are made from what it named.
public static class JobOption
{
	private static readonly Dictionary<string, Job> byName = new(StringComparer.OrdinalIgnoreCase)
	{
		["dry"] = Job.Dry,
		["short"] = Job.ShortRun,
		["medium"] = Job.MediumRun,
		["long"] = Job.LongRun,
		["verylong"] = Job.VeryLongRun,
		["default"] = Job.Default,
	};

	public const string Names = "dry, short, medium, long, verylong or default";

	// Null when --job named something BDN does not know. Defaults to the default job when the flag is absent.
	public static Job? Extract(string[] args, out string[] rest)
	{
		var jobName = "default";
		var kept = new List<string>();
		for (var i = 0; i < args.Length; i++)
		{
			if ((args[i] == "--job" || args[i] == "-j") && i + 1 < args.Length)
				jobName = args[++i];
			else
				kept.Add(args[i]);
		}

		rest = kept.ToArray();
		return byName.TryGetValue(jobName, out var job) ? job : null;
	}
}
