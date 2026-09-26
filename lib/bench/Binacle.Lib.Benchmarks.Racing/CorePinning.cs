using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Binacle.Lib.Benchmarks.Racing;

// Runs inside the benchmark process. On Linux, BDN's pin reaches only the main thread; threads the runtime
// started before it keep every CPU. New threads copy the mask of the thread that starts them.
public static class CorePinning
{
	public static void PinAndCheck()
	{
		var count = int.Parse(Environment.GetEnvironmentVariable("DOTNET_PROCESSOR_COUNT")
			?? throw new InvalidOperationException("DOTNET_PROCESSOR_COUNT is not set. Run this class through the racing Program."));
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
}
