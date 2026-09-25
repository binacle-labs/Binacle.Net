using BischoffSuite = Binacle.Data.BischoffSuite.DataProvider;

namespace Binacle.Lib.Testing;

// Thirty of the 700, picked from lib/results/measurements/packing-efficiency.md on 2026-09-21: every outcome
// the fills show, both size ends, both fit ends. The name printed is "<category> (<id>)".
public static class SampleSet
{
	private static readonly (string Category, string Id)[] picks =
	[
		("typical container", "thpack1_7"),
		("BFD wins big", "thpack4_77"),
		("BFD wins big", "thpack2_51"),
		("BFD wins big", "thpack5_26"),
		("BFD wins big", "thpack6_39"),
		("BFD wins big", "thpack7_48"),
		("BFD wins big", "thpack1_44"),
		("WFD falls over", "thpack2_59"),
		("WFD falls over", "thpack3_98"),
		("WFD falls over", "thpack6_78"),
		("WFD falls over", "thpack2_35"),
		("FFD falls over", "thpack5_47"),
		("WFD wins", "thpack6_93"),
		("WFD wins", "thpack1_58"),
		("WFD wins", "thpack3_43"),
		("FFD wins", "thpack4_25"),
		("FFD wins", "thpack3_35"),
		("FFD wins", "thpack4_93"),
		("all three tie", "thpack1_39"),
		("all three tie", "thpack3_17"),
		("near tie", "thpack2_30"),
		("near tie", "thpack5_29"),
		("tightest fit", "thpack1_54"),
		("tightest fit", "thpack7_4"),
		("loosest fit", "thpack4_23"),
		("most items", "thpack1_65"),
		("fewest items", "thpack1_84"),
		("BFD best fill", "thpack2_33"),
		("many item types", "thpack7_56"),
		("v1 and v2 differ", "thpack7_45"),
	];

	private static readonly Dictionary<string, string> scenarios
		= picks.ToDictionary(p => $"{p.Category} ({p.Id})", p => $"OrLibrary_{p.Id}");

	public static IEnumerable<string> Names
		=> scenarios.Keys;

	public static Scenario GetByName(string name)
		=> BischoffSuite.GetByName(scenarios[name]);
}
