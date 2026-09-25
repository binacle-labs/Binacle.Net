using System.Globalization;
using Binacle.ViPaq.Data.Packed;
using BischoffSuite = Binacle.ViPaq.Data.Packed.BischoffSuite.DataProvider;
using CustomProblems = Binacle.ViPaq.Data.Packed.CustomProblems.DataProvider;
using DemoSamples = Binacle.ViPaq.Data.Packed.DemoSamples.DataProvider;

namespace Binacle.ViPaq.EncodedSize;

// The raw files under vipaq/results/measurements/encoded-size/<algorithm>/. One file per
// group per algorithm per layout - the whole table in one file is more than GitHub will render, and one
// algorithm per file lets a column be read straight down. Every one opens with the same sentence: tool, count,
// group, algorithm - never a date. The count comes from the data, so it cannot drift when the packs are
// regenerated.
internal static class ResultFiles
{
	private static readonly Dictionary<string, int> PackCounts = CountByGroup();

	public static ResultFile EncodedSize(Group group, string algorithm, string layout)
	{
		var packs = PackCounts.GetValueOrDefault(group.Slug) / Algorithms.All.Length;
		var count = packs.ToString("N0", CultureInfo.InvariantCulture);
		var layoutSlug = layout.ToLowerInvariant();

		return new ResultFile
		{
			Filename = $"encoded-size/{algorithm.ToLowerInvariant()}/{layoutSlug}-{group.Slug}",
			Title = $"Encoded size - {group.Label}, {algorithm}, {layoutSlug} layout",
			Description = $"Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over {count} real packs "
				+ $"({group.Slug}), packed by {algorithm}, in the {layoutSlug} layout. One file per group per "
				+ "algorithm per layout; the others sit beside this one. Do not edit."
		};
	}

	private static Dictionary<string, int> CountByGroup()
	{
		var counts = new Dictionary<string, int>();

		Count(Groups.BischoffFamily, BischoffSuite.Names);
		Count("custom-problems", CustomProblems.Names);
		Count("demo-samples", DemoSamples.Names);

		return counts;

		void Count(string family, IEnumerable<string> names)
		{
			foreach (var name in names)
			{
				var slug = Groups.Of(family, name);
				counts[slug] = counts.GetValueOrDefault(slug) + 1;
			}
		}
	}
}
