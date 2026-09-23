using Binacle.ViPaq.Data;
using Binacle.ViPaq.Data.Packed;

namespace Binacle.ViPaq.UnitTests.Providers;

// Every real pack from the three packed-data families as theory rows, one per name, so a failure names the pack.
internal static class PackedScenarioProvider
{
	private static readonly Dictionary<string, Scenario> scenarios;

	static PackedScenarioProvider()
	{
		scenarios = new Dictionary<string, Scenario>();
		foreach (var scenario in BischoffSuite.DataProvider.All.Concat(CustomProblems.DataProvider.All).Concat(DemoSamples.DataProvider.All))
		{
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IEnumerable<object[]> Names
		=> scenarios.Keys.Select(name => new object[] { name });

	// PROTOCOL.md §4 keeps both item widths Eight for an empty pack, so a forced-wide header is not encodable
	// for one. Nine packs today: six custom, three demo.
	public static IEnumerable<object[]> NonEmptyNames
		=> scenarios.Values.Where(scenario => scenario.ItemCount > 0).Select(scenario => new object[] { scenario.Name });

	public static Scenario Get(string name)
		=> scenarios[name];
}
