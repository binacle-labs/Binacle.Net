using Binacle.Data.BischoffSuite;

namespace Binacle.Lib.Testing.Providers;

// The smoke tier's four: both size ends, a typical problem, and the one with the most item types (20).
public static class SmokeProblemsProvider
{
	private static readonly Dictionary<string, Func<Scenario>> scenarios = new()
	{
		["full bin, one type"] = CubeScalingProblemsProvider.GetBaseline,
		["small order"] = SpecializedScalingProblemsProvider.GetBaseline,
		["typical container"] = () => Scenarios.GetScenarioByName("OrLibrary_thpack1_7"),
		["most item types"] = () => Scenarios.GetScenarioByName("OrLibrary_thpack7_56"),
	};

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static Scenario GetScenarioByName(string name)
		=> scenarios[name]();
}
