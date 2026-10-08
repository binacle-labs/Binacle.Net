namespace Binacle.ViPaq.EncodedSize.Reporters;

// One instance per group per algorithm per layout; one table per codec, one row per pack, the four formats
// side by side.
internal sealed class EncodedSizeReporter : IReporter
{
	private readonly EncodingBag bag;
	private readonly Group group;
	private readonly string algorithm;
	private readonly string layout;

	public EncodedSizeReporter(EncodingBag bag, Group group, string algorithm, string layout)
	{
		this.bag = bag;
		this.group = group;
		this.algorithm = algorithm;
		this.layout = layout;
	}

	public ResultFile File => ResultFiles.EncodedSize(this.group, this.algorithm, this.layout);

	public ReportSection[] Report()
		=> Codecs.All.Select(this.Section).ToArray();

	private ReportSection Section(string codec)
	{
		var table = new TableResult("Scenario", "Items", "Widths", "ViPaq", "Proto", "JSON", "Compact", "ViPaq/Proto");

		foreach (var pack in this.Rows())
		{
			var vipaq = pack.ViPaq[this.layout].Of(codec);
			var protobuf = pack.Protobuf.Of(codec);

			table.AddRow(
				pack.Name,
				pack.Items.ToString(),
				pack.Widths,
				vipaq.ToString(),
				protobuf.ToString(),
				pack.Json.Of(codec).ToString(),
				pack.Compact.Of(codec).ToString(),
				Format.Fixed((double)vipaq / protobuf)
			);
		}

		return new ReportSection
		{
			Title = codec,
			Description = Description(codec),
			Table = table
		};
	}

	private static string Description(string codec)
	{
		var stored = codec == Codecs.Raw
			? "ViPaq and protobuf are base64 lengths; JSON and compact are text lengths, because text is its own stored form."
			: $"Every column is the base64 length after {codec.ToLowerInvariant()}.";

		return stored + " Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two"
			+ " columns, in this codec.";
	}

	private IEnumerable<EncodedPack> Rows()
		=> this.bag.Packs.Where(pack
			=> pack.Algorithm == this.algorithm && Groups.Of(pack.Family, pack.Name) == this.group.Slug);
}
