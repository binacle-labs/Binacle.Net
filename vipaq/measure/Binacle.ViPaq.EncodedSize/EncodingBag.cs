namespace Binacle.ViPaq.EncodedSize;

// Everything the reporters read. The runner fills it once; nothing encodes after that.
internal sealed class EncodingBag
{
	public List<EncodedPack> Packs { get; } = new();
}

// One real pack encoded every way. Binary formats are base64 lengths, the stored form; JSON and compact are
// text lengths, because text is its own stored form. Protobuf has no layout, so it is one set of sizes.
internal sealed record EncodedPack(
	string Family,
	string Algorithm,
	string Name,
	int Items,
	string Widths,
	int Json,
	int Compact,
	CodecSizes Protobuf,
	IReadOnlyDictionary<string, CodecSizes> ViPaq
);

// The three codecs' sizes for one format. Raw is the NoOp codec: the body passed through.
internal sealed record CodecSizes(int Raw, int Deflate, int Gzip)
{
	public int Best => Math.Min(this.Raw, Math.Min(this.Deflate, this.Gzip));

	// Raw wins ties, then deflate, so "Raw" means compression did not pay.
	public string BestCodec
	{
		get
		{
			if (this.Raw <= this.Deflate && this.Raw <= this.Gzip)
			{
				return Codecs.Raw;
			}

			return this.Deflate <= this.Gzip ? Codecs.Deflate : Codecs.Gzip;
		}
	}

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
