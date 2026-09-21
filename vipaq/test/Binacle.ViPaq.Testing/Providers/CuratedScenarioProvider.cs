namespace Binacle.ViPaq.Testing.Providers;

// The columns of the Encode and Decode timing classes: the real packs by what each covers, then the synthetic
// curve past them. Each family provider picks its own; this joins them in report order.
public static class CuratedScenarioProvider
{
	private static readonly Dictionary<string, Func<Scenario>> scenarios;

	static CuratedScenarioProvider()
	{
		scenarios = new Dictionary<string, Func<Scenario>>();

		foreach (var (column, name) in CustomProblemsCuratedProvider.TimingColumns)
		{
			scenarios[column] = () => CustomProblemsCuratedProvider.GetByName(name);
		}

		foreach (var (column, name) in BischoffCuratedProvider.TimingColumns)
		{
			scenarios[column] = () => BischoffCuratedProvider.GetByName(name);
		}

		foreach (var name in SyntheticDataProvider.Names)
		{
			scenarios[name] = () => SyntheticDataProvider.GetByName(name);
		}
	}

	public static IEnumerable<string> GetScenarioNames() => scenarios.Keys;

	public static Scenario GetScenarioByName(string name) => scenarios[name]();
}
