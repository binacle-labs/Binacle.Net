using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Binacle.Benchmarking;

// One BDN job per core count, each pinned to the first N CPUs. CreateConfig builds the jobs;
// SetCoreCountAndCheck is what a class calls first in its setup to make the pin stick and prove it did.
public static class JobsByCoreCount
{
	// Changing this moves every kept run's job set.
	public static readonly int[] Counts = [2, 4, 8, 12];

	// Null when --job or --launches is bad, as JobOption.Extract says.
	public static ManualConfig? CreateConfig(string[] args, out string[] rest, int[]? counts = null)
	{
		var baseJob = JobOption.Extract(args, out rest, out var word);
		if (baseJob is null)
			return null;

		var config = BenchmarkConfig.Create()
			.AddColumn(new CoresColumn())
			.HideColumns("Job", "Affinity", "EnvironmentVariables");

		foreach (var count in counts ?? Counts)
		{
			// The mask alone can come too late: BDN pins the child after it starts, and .NET reads its CPU count
			// once at start-up.
			config.AddJob(baseJob
				.WithAffinity((IntPtr)((1L << count) - 1))
				.WithEnvironmentVariable("DOTNET_PROCESSOR_COUNT", count.ToString())
				// The job word leads, so a kept report's header says which job made it. Zero-padded so it sorts 02, 04, 08, 12.
				.WithId($"{word} {count:D2} cores"));
		}

		return config;
	}

	// Runs first in every pinned class's [GlobalSetup]. On Linux, BDN's pin reaches only the main thread; threads
	// started earlier keep every CPU and new threads copy their starter's mask - so every thread is pinned, then checked.
	public static void SetCoreCountAndCheck()
	{
		var count = int.Parse(Environment.GetEnvironmentVariable("DOTNET_PROCESSOR_COUNT")
			?? throw new InvalidOperationException(
				"DOTNET_PROCESSOR_COUNT is not set. Run this class through a Program that builds its jobs from JobsByCoreCount."));
		var mask = (1UL << count) - 1;

		if (!OperatingSystem.IsLinux())
			throw new PlatformNotSupportedException("Pinning to cores is written for Linux only.");

		if (Environment.ProcessorCount != count)
			throw new InvalidOperationException($"ProcessorCount is {Environment.ProcessorCount}, the job asks for {count}.");

		if ((ulong)Process.GetCurrentProcess().ProcessorAffinity != mask)
			throw new InvalidOperationException($"The main thread is not pinned to {count} CPUs.");

		foreach (var tid in ThreadIds())
		{
			var threadMask = mask;
			if (sched_setaffinity(tid, sizeof(ulong), ref threadMask) != 0)
			{
				// The thread ended between the listing and the call.
				if (Marshal.GetLastPInvokeError() == ESRCH)
					continue;
				throw new InvalidOperationException($"Could not pin thread {tid}: errno {Marshal.GetLastPInvokeError()}.");
			}
		}

		foreach (var tid in ThreadIds())
		{
			var allowed = ReadAllowedMask(tid);
			if (allowed is not null && allowed != mask)
				throw new InvalidOperationException(
					$"Thread {tid} runs on {BitOperations.PopCount(allowed.Value)} CPUs, not {count}.");
		}
	}

	private const int ESRCH = 3;

	[DllImport("libc", SetLastError = true)]
	private static extern int sched_setaffinity(int pid, nint cpusetsize, ref ulong mask);

	private static IEnumerable<int> ThreadIds()
		=> Directory.GetDirectories("/proc/self/task").Select(d => int.Parse(Path.GetFileName(d)));

	// Null when the thread has ended.
	private static ulong? ReadAllowedMask(int tid)
	{
		string[] lines;
		try
		{
			lines = File.ReadAllLines($"/proc/self/task/{tid}/status");
		}
		catch (IOException)
		{
			return null;
		}

		// "Cpus_allowed:	fff", in 32-bit groups split by commas.
		var hex = lines.First(l => l.StartsWith("Cpus_allowed:")).Split(':')[1].Trim().Replace(",", "");
		return ulong.Parse(hex, NumberStyles.HexNumber);
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
