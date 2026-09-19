namespace Binacle.Lib.PackingEfficiency.Reporters;

// One section per algorithm, listing only the scenarios where v2 packs differently from v1. A count, not a
// proof of sameness.
internal sealed class VersionParityReporter : IReporter
{
	private readonly PackingBag bag;

	public VersionParityReporter(PackingBag bag)
	{
		this.bag = bag;
	}

	public ResultFile File => ResultFiles.VersionParity;

	public ReportSection[] Report()
		=> Algorithms.Families.Select(this.Section).ToArray();

	private ReportSection Section(string family)
	{
		var table = new TableResult("Scenario", "v1", "v2", "Difference");
		var differing = this.bag.Scenarios.Where(x => Differs(x, family)).ToArray();
		foreach (var scenario in differing)
		{
			var previous = scenario.Fill(family, Algorithms.Previous);
			var shipped = scenario.Fill(family, Algorithms.Shipped);
			table.AddRow(
				scenario.Name,
				Format.Fixed(previous),
				Format.Fixed(shipped),
				Format.Fixed(shipped - previous)
			);
		}

		return new ReportSection
		{
			Title = family,
			Description = $"{differing.Length} of {this.bag.Scenarios.Count} scenarios pack to a different fill under "
				+ $"{family} v2 than under v1. Difference is v2 minus v1, in points.",
			Table = table
		};
	}

	private static bool Differs(PackedScenario scenario, string family)
		=> scenario.Fill(family, Algorithms.Previous) != scenario.Fill(family, Algorithms.Shipped);
}
