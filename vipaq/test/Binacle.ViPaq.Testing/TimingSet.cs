namespace Binacle.ViPaq.Testing;

// The columns of the Encode and Decode timing classes: the real packs by what each covers, then the synthetic
// curve past them. Each set picks its own; this joins them in report order.
public static class TimingSet
{
	private static readonly Dictionary<string, Func<Scenario>> scenarios;

	static TimingSet()
	{
		scenarios = new Dictionary<string, Func<Scenario>>();

		foreach (var column in CustomProblemsTimingSet.Names)
		{
			scenarios[column] = () => CustomProblemsTimingSet.GetByName(column);
		}

		foreach (var column in BischoffTimingSet.Names)
		{
			scenarios[column] = () => BischoffTimingSet.GetByName(column);
		}

		foreach (var column in SyntheticGenerator.Names)
		{
			scenarios[column] = () => SyntheticGenerator.GetByName(column);
		}
	}

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string column) => scenarios[column]();
}
