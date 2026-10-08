namespace Binacle.Data.CustomProblems;

public static class DataProvider
{
	public static readonly string[] Keys =
	[
		"CustomProblems/baseline",
		"CustomProblems/simple",
		"CustomProblems/complex",
	];

	private static readonly Dictionary<string, Scenario> scenarios;
	private static readonly List<ScenarioBin> distinctBins;

	static DataProvider()
	{
		var collections = new MultipleScenarioCollectionsReader(Keys);
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in collections)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);
		}

		distinctBins = scenarios.Values
			.Select(x => x.Bin)
			.DistinctBy(x => x.ID)
			.ToList();
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

	// The bins these scenarios run against, in the order the scenarios introduce them. The API test host
	// registers exactly this set as the `custom-problems` preset, so the list is never written down twice.
	public static IReadOnlyList<ScenarioBin> GetDistinctBins()
		=> distinctBins;

	public static IEnumerable<string> GetDistinctBinIds()
		=> distinctBins.Select(x => x.ID);

	// An item fitting this bin fits every one of them, so a caller can name the expected winner of a
	// smallest-bin or best-fit selection without hardcoding an ID.
	public static ScenarioBin GetSmallestBin()
		=> distinctBins.MinBy(bin => bin.CalculateVolume())!;
}
