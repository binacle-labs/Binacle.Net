namespace Binacle.Lib.PackingEfficiency.Reporters;

// The summaries. Every number here is computed from the bag; the rows behind them are in the other files.
internal sealed class ReadmeReporter : IReporter
{
	private static readonly string[] StatColumns = ["Min", "Mean", "Median", "Max", "StdDev"];

	private readonly PackingBag bag;

	public ReadmeReporter(PackingBag bag)
	{
		this.bag = bag;
	}

	public ResultFile File => ResultFiles.Readme;

	public ReportSection[] Report()
		=>
		[
			this.FillPerAlgorithm(),
			this.FillPerSet(),
			this.UsersFill(),
			this.Wins(),
			this.VersionParity(),
			Files()
		];

	private ReportSection FillPerAlgorithm()
	{
		var table = new TableResult(["Algorithm", .. StatColumns]);
		foreach (var family in Algorithms.Families)
		{
			var fills = this.bag.Scenarios.Select(x => (double)x.Fill(family, Algorithms.Shipped));
			table.AddRow([family, .. Stats(fills)]);
		}

		return new ReportSection
		{
			Title = "📊 Fill per algorithm",
			Description = $"Fill as a percentage of the bin, over all {this.bag.Scenarios.Count} scenarios.",
			Table = table
		};
	}

	private ReportSection FillPerSet()
	{
		var table = new TableResult(["Set", "Algorithm", .. StatColumns]);
		foreach (var set in this.bag.Scenarios.GroupBy(x => x.Set))
		{
			var label = $"{set.Key} ({set.First().Types} types)";
			foreach (var family in Algorithms.Families)
			{
				var fills = set.Select(x => (double)x.Fill(family, Algorithms.Shipped));
				table.AddRow([label, family, .. Stats(fills)]);
			}
		}

		return new ReportSection
		{
			Title = "📊 Fill per set",
			Description = "The same, per thpack file. Each set is 100 scenarios with the same number of item types.",
			Table = table
		};
	}

	private ReportSection UsersFill()
	{
		var table = new TableResult(["Picks", .. StatColumns]);
		var raced = this.bag.Scenarios.Select(x => (double)Math.Max(
			x.Fill("FFD", Algorithms.Shipped),
			x.Fill("BFD", Algorithms.Shipped)));
		var all = this.bag.Scenarios.Select(x => (double)Algorithms.Families.Max(family => x.Fill(family, Algorithms.Shipped)));
		table.AddRow(["Best of FFD and BFD (what the API races)", .. Stats(raced)]);
		table.AddRow(["Best of all three", .. Stats(all)]);

		return new ReportSection
		{
			Title = "📊 The user's fill",
			Description = "The fill a caller gets when the best of several algorithms is kept, from the same packings.",
			Table = table
		};
	}

	private ReportSection Wins()
	{
		var table = new TableResult("Algorithm", "Best or tied");
		var wins = this.bag.Scenarios.Select(Reporters.Wins.Of).ToArray();
		foreach (var family in Algorithms.Families)
		{
			var count = wins.Count(x => x.Best.Contains(family));
			table.AddRow(family, count.ToString());
		}

		return new ReportSection
		{
			Title = "🔢 Wins",
			Description = $"How many of the {this.bag.Scenarios.Count} scenarios each algorithm packed at least as well "
				+ "as the other two. A tie counts for every algorithm in it.",
			Table = table
		};
	}

	private ReportSection VersionParity()
	{
		var table = new TableResult("Algorithm", "Same as v1", "Different");
		foreach (var family in Algorithms.Families)
		{
			var different = this.bag.Scenarios.Count(x =>
				x.Fill(family, Algorithms.Previous) != x.Fill(family, Algorithms.Shipped));
			table.AddRow(family, (this.bag.Scenarios.Count - different).ToString(), different.ToString());
		}

		return new ReportSection
		{
			Title = "🔢 Version parity",
			Description = "Scenarios where the shipped v2 packs to the same fill as v1.",
			Table = table
		};
	}

	private static ReportSection Files()
	{
		var table = new TableResult("File", "What it is");
		table.AddRow("[packing-efficiency.md](packing-efficiency.md)", "One row per scenario: fill per algorithm, which won, and by how much");
		table.AddRow("[version-parity.md](version-parity.md)", "Per algorithm, only the scenarios where v1 and v2 pack differently");

		return new ReportSection
		{
			Title = "📂 Files",
			Table = table
		};
	}

	private static string[] Stats(IEnumerable<double> values)
	{
		var stats = Statistics.Of(values);
		return
		[
			Format.Fixed(stats.Min),
			Format.Fixed(stats.Mean),
			Format.Fixed(stats.Median),
			Format.Fixed(stats.Max),
			Format.Fixed(stats.StdDev)
		];
	}
}
