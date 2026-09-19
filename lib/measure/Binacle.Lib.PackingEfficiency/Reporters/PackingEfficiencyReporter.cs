namespace Binacle.Lib.PackingEfficiency.Reporters;

// One row per scenario: the shipped version's fill per algorithm, which won, and by how much.
internal sealed class PackingEfficiencyReporter : IReporter
{
	private readonly PackingBag bag;

	public PackingEfficiencyReporter(PackingBag bag)
	{
		this.bag = bag;
	}

	public ResultFile File => ResultFiles.PackingEfficiency;

	public ReportSection[] Report()
	{
		var table = new TableResult("Scenario", "Types", "Items", "Ceiling %", "FFD", "WFD", "BFD", "Best", "Margin");
		foreach (var scenario in this.bag.Scenarios)
		{
			var win = Wins.Of(scenario);
			table.AddRow(
				scenario.Name,
				scenario.Types.ToString(),
				scenario.Items.ToString(),
				Format.Fixed(scenario.Ceiling),
				Format.Fixed(scenario.Fill("FFD", Algorithms.Shipped)),
				Format.Fixed(scenario.Fill("WFD", Algorithms.Shipped)),
				Format.Fixed(scenario.Fill("BFD", Algorithms.Shipped)),
				string.Join(", ", win.Best),
				Format.Fixed(win.Margin)
			);
		}

		return
		[
			new ReportSection
			{
				Title = "Fill per scenario",
				Description = "Fill is the packed volume as a percentage of the bin. Best lists every algorithm that "
					+ "reached the top fill; Margin is the top fill minus the next one, in points.",
				Table = table
			}
		];
	}
}
