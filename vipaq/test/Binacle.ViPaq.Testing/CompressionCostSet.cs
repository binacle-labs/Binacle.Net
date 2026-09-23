using Binacle.ViPaq.Data.Packed;
using BischoffSuite = Binacle.ViPaq.Data.Packed.BischoffSuite.DataProvider;

namespace Binacle.ViPaq.Testing;

// The two Bischoff packs the CompressionCost benchmarks run on, both FFD, keyed by the column name the report
// prints. Not a timing question, which is why these are not in BischoffTimingSet.
//
//   OrLibrary_thpack4_1    the low end of deflate's win, 54% saved
//   OrLibrary_thpack1_2    108 items, raw 1312 -> deflate 404 b64, 69% saved - the high end
public static class CompressionCostSet
{
	private static readonly Dictionary<string, string> columns = new()
	{
		["compression low win"] = "OrLibrary_thpack4_1.ffd",
		["compression high win"] = "OrLibrary_thpack1_2.ffd",
	};

	public static IEnumerable<string> Names => columns.Keys;

	public static Scenario GetByName(string column) => BischoffSuite.GetByName(columns[column]);

	// The packs behind the columns, for the gate that checks every pick still exists.
	public static IEnumerable<string> PackNames => columns.Values;
}
