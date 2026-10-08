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
			new MultipleScenarioCollectionsReader(CustomProblems.DataProvider.Keys),
			new MultipleScenarioCollectionsReader(DemoSamples.DataProvider.Keys)
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

	public static IEnumerable<string> Names
		=> scenarios.Keys;

	// xUnit's MemberData takes one row per case, so the names come wrapped.
	public static IEnumerable<object[]> TheoryNames
		=> Names.Select(name => new object[] { name });

	// Not `All`, which is this class.
	public static IEnumerable<Scenario> Scenarios
		=> scenarios.Values;

	public static Scenario GetByName(string name)
		=> scenarios[name];
}
