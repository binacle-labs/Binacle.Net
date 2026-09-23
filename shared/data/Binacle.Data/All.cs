namespace Binacle.Data;

// Every scenario of every set, by name.
public static class All
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static All()
	{
		MultipleScenarioCollectionsReader[] readers =
		[
			new MultipleScenarioCollectionsReader(BischoffSuite.DataProvider.Keys),
			new MultipleScenarioCollectionsReader(CustomProblems.Scenarios.Keys),
			new MultipleScenarioCollectionsReader(DemoSamples.Scenarios.Keys)
		];
		scenarios = new Dictionary<string, Scenario>();
		foreach (var reader in readers)
		{
			foreach (var collectionScenario in reader)
			{
				var scenario = collectionScenario.Scenario;
				scenarios.Add(scenario.Name, scenario);
			}
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
