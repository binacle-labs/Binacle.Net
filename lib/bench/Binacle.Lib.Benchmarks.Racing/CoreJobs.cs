using System.Numerics;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Binacle.Lib.Benchmarks.Racing;

// One job per core count, each pinned to the first N CPUs.
public static class CoreJobs
{
	public static readonly int[] Counts = [2, 4, 8, 12];

	private static readonly Dictionary<string, Job> baseJobs = new(StringComparer.OrdinalIgnoreCase)
	{
		["dry"] = Job.Dry,
		["short"] = Job.ShortRun,
		["medium"] = Job.MediumRun,
		["long"] = Job.LongRun,
		["verylong"] = Job.VeryLongRun,
		["default"] = Job.Default,
	};

	// BDN adds a CLI --job beside the config's jobs instead of applying it to them, so it is taken out of the
	// args here and the core jobs are built from it. Null when the job is not one BDN knows.
	public static ManualConfig? CreateConfig(string[] args, out string[] rest)
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
		if (!baseJobs.TryGetValue(jobName, out var baseJob))
			return null;

		var config = BenchmarkConfig.Create()
			.AddColumn(new CoresColumn())
			.HideColumns("Job", "Affinity", "EnvironmentVariables");

		foreach (var count in Counts)
		{
			// The mask alone can come too late: BDN pins the child after it starts, and .NET reads its CPU count
			// once at start-up.
			config.AddJob(baseJob
				.WithAffinity((IntPtr)((1L << count) - 1))
				.WithEnvironmentVariable("DOTNET_PROCESSOR_COUNT", count.ToString())
				// Zero-padded so the report sorts 02, 04, 08, 12.
				.WithId($"{count:D2} cores"));
		}

		return config;
	}

	private class CoresColumn : IColumn
	{
		public string Id => nameof(CoresColumn);
		public string ColumnName => "Cores";
		public bool AlwaysShow => true;
		public ColumnCategory Category => ColumnCategory.Params;
		public int PriorityInCategory => 1;
		public bool IsNumeric => true;
		public UnitType UnitType => UnitType.Dimensionless;
		public string Legend => "CPUs the case was pinned to";

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
			=> BitOperations.PopCount((ulong)benchmarkCase.Job.Environment.Affinity).ToString();

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
			=> this.GetValue(summary, benchmarkCase);

		public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
		public bool IsAvailable(Summary summary) => true;
	}
}
