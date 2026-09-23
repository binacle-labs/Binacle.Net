using System.Text;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Data.Packed;
using Binacle.ViPaq.Testing.Compact;
using Binacle.ViPaq.Testing.Json;
using Binacle.ViPaq.Testing.Protobuf;
using Binacle.ViPaq.Testing.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.EncodedSize;

// Encodes every real pack once, every format through every codec: ViPaq per codec per layout, and protobuf,
// JSON and compact per codec.
internal sealed class EncodingRunner : IRunner
{
	private static readonly (string Family, IReadOnlyCollection<Scenario> Packs)[] Families =
	[
		("bischoff-suite", BischoffSuite.DataProvider.All),
		("custom-problems", CustomProblems.DataProvider.All),
		("demo-samples", DemoSamples.DataProvider.All)
	];

	private static readonly (string Name, ICompressionCodec Codec)[] CodecList =
	[
		(Codecs.Raw, new NoOpCodec()),
		(Codecs.Deflate, new DeflateCodec()),
		(Codecs.Gzip, new GzipCodec())
	];

	private readonly EncodingBag bag;
	private readonly ILogger<EncodingRunner> logger;

	public EncodingRunner(EncodingBag bag, ILogger<EncodingRunner> logger)
	{
		this.bag = bag;
		this.logger = logger;
	}

	public void Run()
	{
		foreach (var (family, packs) in Families)
		{
			// Embedded-resource order is not stable across rebuilds, so the rows are sorted here.
			foreach (var scenario in packs.OrderBy(x => x.Name, StringComparer.Ordinal))
			{
				this.bag.Packs.Add(Encode(family, scenario));
			}

			this.logger.LogInformation("Encoded {Family}: {Count} packs", family, packs.Count);
		}
	}

	private static EncodedPack Encode(string family, Scenario scenario)
	{
		var vipaq = new Dictionary<string, CodecSizes>();
		foreach (var layout in Layouts.All)
		{
			vipaq[layout.LayoutName] = Sizes(codec => new ViPaqEncoder(codec).Encode(scenario, layout));
		}

		var protobuf = Sizes(codec => new ProtobufEncoder(codec).Encode(scenario));

		// Widths do not depend on the layout, so one label serves both.
		var widths = ViPaqHeader.Create(scenario, EncoderInfo.RowMajor).ToWidthsLabel();

		return new EncodedPack(
			Family: family,
			Algorithm: AlgorithmOf(scenario.Name),
			Name: ProblemOf(scenario.Name),
			Items: scenario.ItemCount,
			Widths: widths,
			Json: TextSizes(JsonEncoder.Encode(scenario)),
			Compact: TextSizes(CompactEncoder.Encode(scenario)),
			Protobuf: protobuf,
			ViPaq: vipaq
		);
	}

	private static CodecSizes Sizes(Func<ICompressionCodec, byte[]> encode)
	{
		var lengths = CodecList.ToDictionary(x => x.Name, x => encode(x.Codec).ToBase64().Length);
		return new CodecSizes(lengths[Codecs.Raw], lengths[Codecs.Deflate], lengths[Codecs.Gzip]);
	}

	// Raw is the text itself; compressing it makes bytes, so those are base64 like every other format.
	private static CodecSizes TextSizes(string text)
	{
		var bytes = Encoding.UTF8.GetBytes(text);
		var lengths = CodecList.ToDictionary(x => x.Name, x => x.Codec.Compress(bytes).ToBase64().Length);
		return new CodecSizes(text.Length, lengths[Codecs.Deflate], lengths[Codecs.Gzip]);
	}

	// "OrLibrary_thpack1_1.ffd" -> "FFD"
	internal static string AlgorithmOf(string name) => name[(name.LastIndexOf('.') + 1)..].ToUpperInvariant();

	// "OrLibrary_thpack1_1.ffd" -> "OrLibrary_thpack1_1"
	private static string ProblemOf(string name) => name[..name.LastIndexOf('.')];
}
