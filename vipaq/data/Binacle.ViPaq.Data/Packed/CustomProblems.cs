namespace Binacle.ViPaq.Data.Packed;

// Real placed results for the custom, hand-authored problems: the bin plus the placed items the packer
// produced. Generated offline by Binacle.ViPaq.PackedDataGenerator for every algorithm, committed under
// vipaq/data/packed/custom-problems/ and read here as embedded resources. No token is stored - it is derivable,
// so the benchmark computes it. Do not hand-edit.
public static class CustomProblems
{
	private const string Family = "custom-problems";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static CustomProblems()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			// See BischoffSuite: the name's algorithm suffix is what keeps the three algorithms apart.
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
