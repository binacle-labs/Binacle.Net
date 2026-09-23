namespace Binacle.Data.DemoSamples;

public static class Scenarios
{
	public static readonly string[] Keys =
	[
		"DemoSamples/00-two-winners",
		"DemoSamples/01-opening-set",
		"DemoSamples/02-packs-nowhere",
		"DemoSamples/03-three-answers",
		"DemoSamples/04-bfd-loses",
		"DemoSamples/05-one-of-each",
		"DemoSamples/06-long-items",
		"DemoSamples/07-tall-items",
		"DemoSamples/08-cube-bin",
		"DemoSamples/09-bfd-fits-more",
		"DemoSamples/10-six-types",
		"DemoSamples/11-seven-types",
		"DemoSamples/12-middle-bin-wins",
		"DemoSamples/13-twenty-four-cubes",
		"DemoSamples/14-flat-items",
		"DemoSamples/15-same-volume-different-shape",
		"DemoSamples/16-only-bfd-fully-packs",
		"DemoSamples/17-four-bins-bfd-ahead",
		"DemoSamples/18-four-bins-bfd-fully-packs",
		"DemoSamples/19-five-bins",
		"DemoSamples/20-wfd-wins",
	];

	private static readonly Dictionary<string, Scenario> scenarios;

	static Scenarios()
	{
		var collections = new MultipleScenarioCollectionsReader(Keys);
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in collections)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static IEnumerable<object[]> ScenarioNames
		=> GetScenarioNames().Select(name => new object[] { name });

	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;

	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];
}
