namespace Binacle.Lib.Testing.Providers;

// The smoke tier's four: both size ends, a typical problem, and one with many item types (20, like every thpack7).
public static class SmokeProblemsProvider
{
	private static readonly Dictionary<string, Func<Scenario>> scenarios = new()
	{
		["full bin, one type"] = CubeScalingProblemsProvider.GetBaseline,
		["small order"] = SpecializedScalingProblemsProvider.GetBaseline,
		["typical container"] = () => BischoffSuite.DataProvider.GetByName("OrLibrary_thpack1_7"),
		["many item types"] = () => BischoffSuite.DataProvider.GetByName("OrLibrary_thpack7_56"),
	};

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static Scenario GetScenarioByName(string name)
		=> scenarios[name]();
}
