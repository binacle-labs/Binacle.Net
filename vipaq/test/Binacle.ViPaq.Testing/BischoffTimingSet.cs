using Binacle.ViPaq.Data.Packed;
using BischoffSuite = Binacle.ViPaq.Data.Packed.BischoffSuite.DataProvider;

namespace Binacle.ViPaq.Testing;

// The Bischoff packs the timing benchmarks run on, all FFD packs, keyed by the column name the report prints.
//
//   OrLibrary_thpack4_1     70 items, width 16/8/16 - the median real pack is 79 items at that width
//   OrLibrary_thpack1_65   365 items - the largest FFD pack, the tail no other pick reaches. The same problem
//                          under BFD holds 371, the largest pack in the data, which is why the column names
//                          the algorithm.
//
// Pack names resolve through the Bischoff data provider, so a stale pick is caught by the curated check.
public static class BischoffTimingSet
{
	private static readonly Dictionary<string, string> columns = new()
	{
		["typical container"] = "OrLibrary_thpack4_1.ffd",
		["largest FFD pack"] = "OrLibrary_thpack1_65.ffd",
	};

	public static IEnumerable<string> Names => columns.Keys;

	public static Scenario GetByName(string column) => BischoffSuite.GetByName(columns[column]);

	// The packs behind the columns, for the gate that checks every pick still exists.
	public static IEnumerable<string> PackNames => columns.Values;
}
