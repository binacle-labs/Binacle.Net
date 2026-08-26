using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Real placed results for the Bischoff suite (thpack1..7): the bin plus the placed items the packer produced.
// Generated offline by Binacle.ViPaq.PackedDataGenerator for every algorithm, committed under
// vipaq/data/packed/bischoff-suite/ and read here as embedded resources. No token is stored - it is derivable,
// so the benchmark computes it. Do not hand-edit.
public static class BischoffDataProvider
{
	private const string Family = "bischoff-suite";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static BischoffDataProvider()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			// Scenario.Name already ends in the algorithm suffix, so the same problem under FFD, WFD and BFD
			// are three distinct keys.
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
