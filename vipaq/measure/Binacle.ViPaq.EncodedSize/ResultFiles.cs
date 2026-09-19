using System.Globalization;
using Binacle.ViPaq.Data.Packed;

namespace Binacle.ViPaq.EncodedSize;

// The files under vipaq/results/. Every one opens with the same sentence: tool, count, families - never a date.
// The count comes from the data, so it cannot drift when the packs are regenerated.
internal static class ResultFiles
{
	private static readonly int PackCount = BischoffSuite.All.Count + CustomProblems.All.Count + DemoSamples.All.Count;

	private static readonly string Header =
		$"Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over {PackCount.ToString("N0", CultureInfo.InvariantCulture)} real packs "
		+ "(bischoff-suite, custom-problems, demo-samples). Do not edit.";

	public static readonly ResultFile Readme = new()
	{
		Filename = "README",
		Title = "Encoded size results",
		Description = Header
	};

	public static readonly ResultFile EncodedSize = new()
	{
		Filename = "encoded-size",
		Title = "Encoded size",
		Description = Header
	};
}
