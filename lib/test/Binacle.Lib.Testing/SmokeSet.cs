using BischoffSuite = Binacle.Data.BischoffSuite.DataProvider;

namespace Binacle.Lib.Testing;

// The smoke tier's four: both size ends, a typical problem, and one with many item types (20, like every thpack7).
public static class SmokeSet
{
	private static readonly Dictionary<string, Func<Scenario>> scenarios = new()
	{
		["full bin, one type"] = CubeGenerator.GetBaseline,
		["small order"] = LadderGenerator.GetBaseline,
		["typical container"] = () => BischoffSuite.GetByName("OrLibrary_thpack1_7"),
		["many item types"] = () => BischoffSuite.GetByName("OrLibrary_thpack7_56"),
	};

	public static IEnumerable<string> Names
		=> scenarios.Keys;

	public static Scenario GetByName(string name)
		=> scenarios[name]();
}
