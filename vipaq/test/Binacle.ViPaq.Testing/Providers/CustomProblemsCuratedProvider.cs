using Binacle.ViPaq.Data.Packed;

namespace Binacle.ViPaq.Testing.Providers;

// The custom packs the benchmarks time, all FFD packs, keyed by the column name the report prints. Every timing
// class runs the raw path, so the picks differ by item count and width only: the fixed cost of one item, the
// one all-16-bit real pack, a small 8-bit pack, and the big 8-bit end.
public static class CustomProblemsCuratedProvider
{
	public static IReadOnlyDictionary<string, string> TimingColumns { get; } = new Dictionary<string, string>
	{
		["one item"] = "Baseline_5x5x5-1_FitsIn_60x40x10.ffd",
		["small, all 16-bit"] = "Simple_16bit-4_FitIn_600x400x300.ffd",
		["small, 8-bit"] = "Complex_FitsInMedium_1.ffd",
		["100 cubes, 8-bit"] = "Simple_5x5x5-100_FitIn_60x40x10.ffd",
	};

	public static IEnumerable<string> Names => TimingColumns.Values;

	public static Scenario GetByName(string name) => CustomProblems.GetByName(name);
}
