namespace Binacle.Lib.Data.ResultSelection.BestAlgorithm;

public static class Scenarios
{
	public static readonly string[] Keys =
	[
		"BestAlgorithm/baseline",
	];

	private static readonly Dictionary<string, Scenario> scenarios;

	static Scenarios()
	{
		var dataProvider = new MultipleScenarioCollectionsProvider(Keys);
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in dataProvider)
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
