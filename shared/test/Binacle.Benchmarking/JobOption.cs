using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Binacle.Benchmarking;

// The --job and --launches options every bench project takes. BDN adds a CLI --job beside the config's jobs
// instead of applying it to them, so the option is taken out of the args and the jobs are built from it.
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

	public const string Usage = "Use --job " + Names + " and --launches N, a whole number above 0.";

	// Null when --job named something BDN does not know, or --launches is not a whole number above 0.
	// name is the word that picked the job.
	//
	// --launches is ours, not BDN's --launchCount. One process per case hides a bad start: a tight StdDev
	// around a wrong mean. More launches is the only way to see it.
	public static Job? Extract(string[] args, out string[] rest, out string name)
	{
		name = "default";
		int? launches = null;
		var badLaunches = false;
		var kept = new List<string>();
		for (var i = 0; i < args.Length; i++)
		{
			if ((args[i] == "--job" || args[i] == "-j") && i + 1 < args.Length)
				name = args[++i];
			else if (args[i] == "--launches" && i + 1 < args.Length)
			{
				if (int.TryParse(args[++i], out var n) && n > 0)
					launches = n;
				else
					badLaunches = true;
			}
			else
				kept.Add(args[i]);
		}

		rest = kept.ToArray();
		if (badLaunches || !byName.TryGetValue(name, out var job))
			return null;

		name = name.ToLowerInvariant();
		return launches is { } count ? job.WithLaunchCount(count) : job;
	}

	// One job, its id the word. Null as Extract is.
	public static ManualConfig? CreateConfig(string[] args, out string[] rest)
	{
		var job = Extract(args, out rest, out var name);
		return job is null ? null : BenchmarkConfig.Create().AddJob(job.WithId(name));
	}
}
