using Binacle.Reporting;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Testing.Protobuf;
using Binacle.ViPaq.Testing.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.EncodedSize.Tests;

// Stored size for every scenario, ViPaq against protobuf, at one (codec, layout).
//
// Protobuf runs the same codec as ViPaq, so only the format differs. Comparing a compressed ViPaq token against
// raw protobuf is the unfairness this rule exists to stop.
//
// Base64 is the stored form and the headline, with raw byte counts beside it. No layout column: layout is fixed
// per table and does not move raw size. Synthetic data is never size-measured - gzip can't grip random data.
internal class VipaqProtobufSizeComparisonTest : IReporter
{
	private readonly IReadOnlyCollection<Scenario> scenarios;
	private readonly ICompressionCodec compressionCodec;
	private readonly EncoderInfo encoderInfo;
	private readonly string title;
	private readonly string description;
	private readonly ILogger<VipaqProtobufSizeComparisonTest> logger;

	public ResultFile File { get; }

	public VipaqProtobufSizeComparisonTest(
		IReadOnlyCollection<Scenario> scenarios,
		ICompressionCodec compressionCodec,
		EncoderInfo encoderInfo,
		string title,
		string description,
		ResultFile file,
		ILogger<VipaqProtobufSizeComparisonTest> logger
	)
	{
		this.scenarios = scenarios;
		this.compressionCodec = compressionCodec;
		this.encoderInfo = encoderInfo;
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
	}

	public ReportSection[] Report()
	{
		var table = new TableResult(
			"Scenario",
			"Items",
			"Widths b/i/c",
			"ViPaq bytes",
			"ViPaq b64",
			"Proto bytes",
			"Proto b64",
			"ViPaq/Proto"
		);

		var vipaqEncoder = new ViPaqEncoder(this.compressionCodec);
		var protobufEncoder = new ProtobufEncoder(this.compressionCodec);

		foreach (var scenario in this.scenarios)
		{
			var vipaqHeader = ViPaqHeader.Create(scenario, this.encoderInfo);
			var vipaqToken = vipaqEncoder.Encode(scenario, this.encoderInfo);
			var vipaqTokenBase64 = vipaqToken.ToBase64();

			// Same codec on protobuf as the ViPaq side, so the comparison measures format, not compression.
			var protobufToken = protobufEncoder.Encode(scenario);
			var protobufTokenBase64 = protobufToken.ToBase64();

			// Stored form against stored form: 0.30 means ViPaq is 30% the size of protobuf.
			var ratio = (double)vipaqTokenBase64.Length / protobufTokenBase64.Length;

			table.AddRow(
				scenario.Name,
				scenario.ItemCount.ToString(),
				vipaqHeader.ToWidthsLabel(),
				vipaqToken.Length.ToString(),
				vipaqTokenBase64.Length.ToString(),
				protobufToken.Length.ToString(),
				protobufTokenBase64.Length.ToString(),
				$"{ratio:F2}"
			);
		}

		return
		[
			new ReportSection
			{
				Title = this.title,
				Description = this.description,
				Table = table
			}
		];
	}
}
