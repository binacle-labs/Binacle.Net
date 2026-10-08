using Binacle.ViPaq.Testing.ViPaq;

namespace Binacle.ViPaq.Benchmarks;

// Setup encodes each form once, plus the header decode needs, so only the read is timed.
[MemoryDiagnoser]
public class Sample_CompressionCost_Decode : CompressionCostBase
{
	private ViPaqHeader header;
	private byte[] noopToken = [];
	private byte[] deflateToken = [];
	private byte[] gzipToken = [];

	public override void GlobalSetup()
	{
		base.GlobalSetup();

		// One header, row-major with the compressed bit set; the codec decides whether the body is squeezed.
		this.header = ViPaqHeader.Create(this.Scenario, EncoderInfo.RowMajor);
		this.noopToken = this.NoOpEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
		this.deflateToken = this.DeflateEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
		this.gzipToken = this.GzipEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) NoOp()
		=> this.NoOpEncoder.Decode(this.noopToken, this.header);

	[Benchmark]
	[BenchmarkOrder(20)]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Deflate()
		=> this.DeflateEncoder.Decode(this.deflateToken, this.header);

	[Benchmark]
	[BenchmarkOrder(30)]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Gzip()
		=> this.GzipEncoder.Decode(this.gzipToken, this.header);
}
