namespace Binacle.ViPaq.EncodedSize.Reporters;

// One row per pack per layout: every format's stored size side by side.
internal sealed class EncodedSizeReporter : IReporter
{
	private readonly EncodingBag bag;

	public EncodedSizeReporter(EncodingBag bag)
	{
		this.bag = bag;
	}

	public ResultFile File => ResultFiles.EncodedSize;

	public ReportSection[] Report()
		=> Layouts.All.Select(layout => this.Section(layout.LayoutName)).ToArray();

	private ReportSection Section(string layout)
	{
		var table = new TableResult(
			"Scenario", "Algorithm", "Items", "Widths",
			"ViPaq raw", "ViPaq deflate", "ViPaq gzip",
			"Proto raw", "Proto deflate", "Proto gzip",
			"JSON", "Compact",
			"ViPaq/Proto", "Best codec", "Saved %");

		foreach (var pack in this.bag.Packs)
		{
			var vipaq = pack.ViPaq[layout];
			var ratio = (double)vipaq.Deflate / pack.Protobuf.Deflate;
			var saved = (vipaq.Raw - vipaq.Best) / (double)vipaq.Raw * 100;

			table.AddRow(
				pack.Name,
				pack.Algorithm,
				pack.Items.ToString(),
				pack.Widths,
				vipaq.Raw.ToString(),
				vipaq.Deflate.ToString(),
				vipaq.Gzip.ToString(),
				pack.Protobuf.Raw.ToString(),
				pack.Protobuf.Deflate.ToString(),
				pack.Protobuf.Gzip.ToString(),
				pack.Json.ToString(),
				pack.Compact.ToString(),
				Format.Fixed(ratio),
				vipaq.BestCodec,
				Format.Fixed(saved, 0)
			);
		}

		return new ReportSection
		{
			Title = $"{layout} layout",
			Description = "ViPaq and protobuf are base64 lengths; JSON and compact are text lengths. Widths are the "
				+ "bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares the two under deflate. Best codec "
				+ "is the smallest ViPaq token, raw winning ties; Saved % is what it saved against raw.",
			Table = table
		};
	}
}
