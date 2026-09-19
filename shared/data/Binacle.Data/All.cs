namespace Binacle.Data;

// Every scenario of every set, by name.
public static class All
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static All()
	{
		MultipleScenarioCollectionsProvider[] dataProviders =
		[
			new MultipleScenarioCollectionsProvider(BischoffSuite.Scenarios.Keys),
			new MultipleScenarioCollectionsProvider(CustomProblems.Scenarios.Keys)
		];
		scenarios = new Dictionary<string, Scenario>();
		foreach (var dataProvider in dataProviders)
		{
			foreach (var collectionScenario in dataProvider)
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
