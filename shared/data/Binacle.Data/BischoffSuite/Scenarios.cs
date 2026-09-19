namespace Binacle.Data.BischoffSuite;

public static class Scenarios
{
	public static readonly string[] Keys =
	[
		"BischoffSuite/orlib_thpack1",
		"BischoffSuite/orlib_thpack2",
		"BischoffSuite/orlib_thpack3",
		"BischoffSuite/orlib_thpack4",
		"BischoffSuite/orlib_thpack5",
		"BischoffSuite/orlib_thpack6",
		"BischoffSuite/orlib_thpack7",
	];

	private static readonly Dictionary<string, Scenario> scenarios;
	private static readonly List<ScenarioBin> distinctBins;

	static Scenarios()
	{
		var dataProvider = new MultipleScenarioCollectionsProvider(Keys);
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in dataProvider)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);
		}

		distinctBins = scenarios.Values
			.Select(x => x.Bin)
			.DistinctBy(x => x.ID)
			.ToList();
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static IEnumerable<object[]> ScenarioNames
		=> GetScenarioNames().Select(name => new object[] { name });

	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;

	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];

	// The bins these scenarios run against, one entry per ID. The API test host registers exactly this set as the
	// `biscoff-suite` preset. See the note on CustomProblems.Scenarios.GetDistinctBins.
	public static IReadOnlyList<ScenarioBin> GetDistinctBins()
		=> distinctBins;
}
