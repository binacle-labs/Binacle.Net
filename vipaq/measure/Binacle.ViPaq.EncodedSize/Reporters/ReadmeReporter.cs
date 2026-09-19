namespace Binacle.ViPaq.EncodedSize.Reporters;

// The summaries. Every number here is computed from the bag; the rows behind them are in encoded-size.md.
internal sealed class ReadmeReporter : IReporter
{
	private const string HeadlineLayout = "Columnar";
	private static readonly string[] RangeColumns = ["Mean", "Min", "Max"];

	private readonly EncodingBag bag;

	public ReadmeReporter(EncodingBag bag)
	{
		this.bag = bag;
	}

	public ResultFile File => ResultFiles.Readme;

	public ReportSection[] Report()
		=>
		[
			this.ViPaqToProtobuf(),
			this.ViPaqToProtobufPerAlgorithm(),
			this.Lengths(),
			this.UsersNumber(),
			this.Crossover(),
			this.CodecWins(),
			this.LargestToken(),
			Files()
		];

	private ReportSection ViPaqToProtobuf()
	{
		var table = new TableResult(["Codec", "Layout", .. RangeColumns]);
		foreach (var codec in Codecs.All)
		{
			foreach (var layout in Layouts.All)
			{
				var ratios = this.bag.Packs.Select(pack => (double)pack.ViPaq[layout.LayoutName].Of(codec) / pack.Protobuf.Of(codec));
				table.AddRow([codec, layout.LayoutName, .. Range(ratios)]);
			}
		}

		return new ReportSection
		{
			Title = "📊 ViPaq to protobuf",
			Description = "ViPaq base64 length over protobuf base64 length, both under the same codec, per pack; "
				+ "0.65 means ViPaq is 65% the size. Mean of the per-pack ratios.",
			Table = table
		};
	}

	private ReportSection ViPaqToProtobufPerAlgorithm()
	{
		var table = new TableResult(["Algorithm", "Layout", .. RangeColumns]);
		foreach (var algorithm in this.bag.Packs.Select(pack => pack.Algorithm).Distinct())
		{
			foreach (var layout in Layouts.All)
			{
				var ratios = this.bag.Packs
					.Where(pack => pack.Algorithm == algorithm)
					.Select(pack => (double)pack.ViPaq[layout.LayoutName].Deflate / pack.Protobuf.Deflate);
				table.AddRow([algorithm, layout.LayoutName, .. Range(ratios)]);
			}
		}

		return new ReportSection
		{
			Title = "📊 ViPaq to protobuf per algorithm",
			Description = "The same under deflate, split by the algorithm that placed the items.",
			Table = table
		};
	}

	private ReportSection Lengths()
	{
		var table = new TableResult(["Format", .. RangeColumns]);
		this.AddLengthRow(table, "JSON", pack => pack.Json);
		this.AddLengthRow(table, "Compact notation", pack => pack.Compact);
		this.AddLengthRow(table, "Protobuf raw", pack => pack.Protobuf.Raw);
		this.AddLengthRow(table, "Protobuf deflate", pack => pack.Protobuf.Deflate);
		foreach (var layout in Layouts.All)
		{
			this.AddLengthRow(table, $"ViPaq raw, {layout.LayoutName}", pack => pack.ViPaq[layout.LayoutName].Raw);
			this.AddLengthRow(table, $"ViPaq deflate, {layout.LayoutName}", pack => pack.ViPaq[layout.LayoutName].Deflate);
		}

		return new ReportSection
		{
			Title = "📊 Stored length per format",
			Description = "Characters per pack. JSON and compact notation are text; the rest are base64, of which "
				+ "the raw bytes are three quarters. JSON and compact carry the bin and the placed items only, "
				+ "no IDs.",
			Table = table
		};
	}

	// Lengths are whole characters, so min and max carry no decimals; the mean keeps one.
	private void AddLengthRow(TableResult table, string label, Func<EncodedPack, int> length)
	{
		var stats = Statistics.Of(this.bag.Packs.Select(pack => (double)length(pack)));
		table.AddRow(label, Format.Fixed(stats.Mean, 1), Format.Fixed(stats.Min, 0), Format.Fixed(stats.Max, 0));
	}

	private ReportSection UsersNumber()
	{
		var table = new TableResult("Against", "ViPaq is (mean)", "Min", "Max");
		this.AddUsersRow(table, "JSON", pack => pack.Json);
		this.AddUsersRow(table, "Compact notation", pack => pack.Compact);
		this.AddUsersRow(table, "Protobuf raw", pack => pack.Protobuf.Raw);
		this.AddUsersRow(table, "Protobuf deflate", pack => pack.Protobuf.Deflate);

		return new ReportSection
		{
			Title = "📊 The user's number",
			Description = $"A ViPaq token under deflate, {HeadlineLayout.ToLowerInvariant()} layout - the smallest mode - "
				+ "as a share of each other format, per pack.",
			Table = table
		};
	}

	private void AddUsersRow(TableResult table, string label, Func<EncodedPack, int> other)
	{
		var shares = this.bag.Packs.Select(pack => (double)pack.ViPaq[HeadlineLayout].Deflate / other(pack) * 100).ToArray();
		var stats = Statistics.Of(shares);
		table.AddRow(label, $"{Format.Fixed(stats.Mean, 0)}%", $"{Format.Fixed(stats.Min, 0)}%", $"{Format.Fixed(stats.Max, 0)}%");
	}

	private ReportSection Crossover()
	{
		var table = new TableResult("Layout", "First pack where compression pays", "Items", "Packs where it pays");
		foreach (var layout in Layouts.All)
		{
			var paying = this.bag.Packs
				.Where(pack => pack.ViPaq[layout.LayoutName].Best < pack.ViPaq[layout.LayoutName].Raw)
				.OrderBy(pack => pack.Items)
				.ThenBy(pack => pack.Name, StringComparer.Ordinal)
				.ToArray();
			var first = paying.FirstOrDefault();
			table.AddRow(
				layout.LayoutName,
				first is null ? "never" : $"{first.Name} ({first.Algorithm})",
				first is null ? "-" : first.Items.ToString(),
				$"{paying.Length} of {this.bag.Packs.Count}"
			);
		}

		return new ReportSection
		{
			Title = "🔢 Crossover",
			Description = "The smallest pack, by item count, where deflate or gzip beats raw.",
			Table = table
		};
	}

	private ReportSection CodecWins()
	{
		var table = new TableResult(["Layout", .. Codecs.All]);
		foreach (var layout in Layouts.All)
		{
			var wins = this.bag.Packs.GroupBy(pack => pack.ViPaq[layout.LayoutName].BestCodec).ToDictionary(x => x.Key, x => x.Count());
			table.AddRow([layout.LayoutName, .. Codecs.All.Select(codec => wins.GetValueOrDefault(codec).ToString())]);
		}

		return new ReportSection
		{
			Title = "🔢 Codec wins",
			Description = $"How many of the {this.bag.Packs.Count} packs each codec stored smallest. Raw wins a tie, then deflate.",
			Table = table
		};
	}

	private ReportSection LargestToken()
	{
		var table = new TableResult("Layout", "Largest deflate token", "Pack", "Items");
		var largestOverall = 0;
		foreach (var layout in Layouts.All)
		{
			var largest = this.bag.Packs.MaxBy(pack => pack.ViPaq[layout.LayoutName].Deflate)!;
			var length = largest.ViPaq[layout.LayoutName].Deflate;
			largestOverall = Math.Max(largestOverall, length);
			table.AddRow(layout.LayoutName, length.ToString(), $"{largest.Name} ({largest.Algorithm})", largest.Items.ToString());
		}

		return new ReportSection
		{
			Title = "📏 The largest token",
			Description = $"Every real pack deflates to at most {largestOverall} base64 characters, in either layout. "
				+ "That is the size to plan a URL or a field around.",
			Table = table
		};
	}

	private static ReportSection Files()
	{
		var table = new TableResult("File", "What it is");
		table.AddRow("[encoded-size.md](encoded-size.md)", "One row per pack per layout: every format's size, the ratio, the best codec");

		return new ReportSection
		{
			Title = "📂 Files",
			Table = table
		};
	}

	private static string[] Range(IEnumerable<double> values)
	{
		var stats = Statistics.Of(values);
		return [Format.Fixed(stats.Mean), Format.Fixed(stats.Min), Format.Fixed(stats.Max)];
	}
}
