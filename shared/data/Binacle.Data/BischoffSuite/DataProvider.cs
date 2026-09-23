namespace Binacle.Data.BischoffSuite;

public static class DataProvider
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
	private static readonly Dictionary<string, List<Scenario>> byCollection;
	private static readonly List<ScenarioBin> distinctBins;

	static DataProvider()
	{
		var collections = new MultipleScenarioCollectionsReader(Keys);
		scenarios = new Dictionary<string, Scenario>();
		byCollection = new Dictionary<string, List<Scenario>>(StringComparer.OrdinalIgnoreCase);
		foreach (var collectionScenario in collections)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);

			if (!byCollection.TryGetValue(collectionScenario.CollectionKey, out var collection))
			{
				collection = new List<Scenario>();
				byCollection.Add(collectionScenario.CollectionKey, collection);
			}

			collection.Add(scenario);
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

	// One thpack at a time, for a report that labels its rows with the set they came from.
	public static IEnumerable<Scenario> ByCollection(string collectionKey)
		=> byCollection[collectionKey];

	// The bins these scenarios run against, one entry per ID. The API test host registers exactly this set as the
	// `biscoff-suite` preset. See the note on CustomProblems.DataProvider.GetDistinctBins.
	public static IReadOnlyList<ScenarioBin> GetDistinctBins()
		=> distinctBins;
}
