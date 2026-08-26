using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Real placed results for the demo site's sample set: the bin plus the placed items the packer produced.
// Generated offline by Binacle.ViPaq.PackedDataGenerator for every algorithm, committed under
// vipaq/data/packed/demo-samples/ and read here as embedded resources. No token is stored - it is derivable,
// so the benchmark computes it. Do not hand-edit.
public static class DemoSamplesDataProvider
{
	private const string Family = "demo-samples";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static DemoSamplesDataProvider()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			// See BischoffDataProvider: the name's algorithm suffix is what keeps the three algorithms apart.
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
