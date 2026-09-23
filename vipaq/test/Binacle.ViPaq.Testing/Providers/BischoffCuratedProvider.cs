using Binacle.ViPaq.Data.Packed;

namespace Binacle.ViPaq.Testing.Providers;

// The Bischoff packs the benchmarks time, all FFD packs, keyed by the column name the report prints.
//
//   OrLibrary_thpack4_1     70 items, width 16/8/16 - the median real pack is 79 items at that width
//   OrLibrary_thpack1_65   365 items - the largest real pack, the tail no other pick reaches
//   OrLibrary_thpack1_2    108 items - raw 1312 -> deflate 404 b64, 69% saved, the high end of deflate's win;
//                          thpack4_1 is the low end at 54%
//
// Names resolve through BischoffSuite, so a stale pick is caught by the curated check.
public static class BischoffCuratedProvider
{
	public static IReadOnlyDictionary<string, string> TimingColumns { get; } = new Dictionary<string, string>
	{
		["typical container"] = "OrLibrary_thpack4_1.ffd",
		["largest real pack"] = "OrLibrary_thpack1_65.ffd",
	};

	public static IReadOnlyDictionary<string, string> CompressionCostColumns { get; } = new Dictionary<string, string>
	{
		["compression low win"] = "OrLibrary_thpack4_1.ffd",
		["compression high win"] = "OrLibrary_thpack1_2.ffd",
	};

	public static IEnumerable<string> Names
		=> TimingColumns.Values.Concat(CompressionCostColumns.Values).Distinct();

	public static Scenario GetByName(string name) => BischoffSuite.DataProvider.GetByName(name);

	public static IEnumerable<string> GetCompressionCostNames() => CompressionCostColumns.Keys;

	public static Scenario GetCompressionCostByName(string column) => GetByName(CompressionCostColumns[column]);
}
