namespace Binacle.ViPaq.EncodedSize;

// Everything the reporters read. The runner fills it once; nothing encodes after that.
internal sealed class EncodingBag
{
	public List<EncodedPack> Packs { get; } = new();
}

// One real pack encoded every way, each format through all three codecs. Protobuf has no layout, so it is one
// set of sizes.
internal sealed record EncodedPack(
	string Family,
	string Algorithm,
	string Name,
	int Items,
	string Widths,
	CodecSizes Json,
	CodecSizes Compact,
	CodecSizes Protobuf,
	IReadOnlyDictionary<string, CodecSizes> ViPaq
);

// The three codecs' sizes for one format. Raw is the NoOp codec: the body passed through. A binary format is
// measured as base64, the stored form; a text format's raw size is the text itself, since text is its own
// stored form, and its compressed sizes are base64 like everything else.
internal sealed record CodecSizes(int Raw, int Deflate, int Gzip)
{
	public int Of(string codec)
		=> codec switch
		{
			Codecs.Raw => this.Raw,
			Codecs.Deflate => this.Deflate,
			Codecs.Gzip => this.Gzip,
			_ => throw new ArgumentOutOfRangeException(nameof(codec), codec, "Unknown codec.")
		};
}

internal static class Codecs
{
	public const string Raw = "Raw";
	public const string Deflate = "Deflate";
	public const string Gzip = "Gzip";

	public static readonly string[] All = [Raw, Deflate, Gzip];
}

internal static class Layouts
{
	public static readonly EncoderInfo[] All = [EncoderInfo.RowMajor, EncoderInfo.Columnar];
}
