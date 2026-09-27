namespace Binacle.Lib.PackingEfficiency.Reporters;

// One section per algorithm, one column per version, listing only the scenarios where any version packs
// differently. A count, not a proof of sameness.
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
		var versions = Algorithms.Versions(family);
		var names = string.Join(", ", versions.Select(v => $"v{v}"));

		var table = new TableResult(["Scenario", .. versions.Select(v => $"v{v}")]);
		var differing = this.bag.Scenarios
			.Where(x => versions.Select(v => x.Fill(family, v)).Distinct().Count() > 1)
			.ToArray();
		foreach (var scenario in differing)
			table.AddRow([scenario.Name, .. versions.Select(v => Format.Fixed(scenario.Fill(family, v)))]);

		var description = differing.Length == 0
			? $"All {this.bag.Scenarios.Count} scenarios pack to the same fill under {family} {names}."
			: $"{differing.Length} of {this.bag.Scenarios.Count} scenarios pack to a different fill under at least one "
				+ $"of {family} {names}.";

		return new ReportSection
		{
			Title = family,
			Description = description,
			Table = differing.Length == 0 ? null : table
		};
	}
}
