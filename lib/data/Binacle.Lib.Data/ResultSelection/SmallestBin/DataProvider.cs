namespace Binacle.Lib.Data.ResultSelection.SmallestBin;

public static class DataProvider
{
	public static readonly string[] Keys =
	[
		"SmallestBin/baseline",
	];

	private static readonly Dictionary<string, Scenario> scenarios;

	static DataProvider()
	{
		var collections = new MultipleScenarioCollectionsReader(Keys);
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in collections)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IEnumerable<string> Names
		=> scenarios.Keys;

	// xUnit's MemberData takes one row per case, so the names come wrapped.
	public static IEnumerable<object[]> TheoryNames
		=> Names.Select(name => new object[] { name });

	public static IEnumerable<Scenario> All
		=> scenarios.Values;

	public static Scenario GetByName(string name)
		=> scenarios[name];
}
