using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Testing.Protobuf;
using Binacle.ViPaq.Testing.ViPaq;

namespace Binacle.ViPaq.Benchmarks;

// The codec is NoOp, so this times the format alone. Setup encodes each form once, plus the header ViPaq's
// decode needs, so only the read is timed.
public abstract class DecodeBase : BenchmarkBase
{
	private ProtobufEncoder protobufEncoder = null!;
	private ViPaqEncoder vipaqEncoder = null!;
	private byte[] protobufToken = [];
	private byte[] vipaqTokenRow = [];
	private byte[] vipaqTokenColumnar = [];
	private ViPaqHeader vipaqHeaderRow;
	private ViPaqHeader vipaqHeaderColumnar;

	protected override Scenario Load(string name)
		=> TimingSet.GetByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();

		this.protobufEncoder = new ProtobufEncoder(new NoOpCodec());
		this.vipaqEncoder = new ViPaqEncoder(new NoOpCodec());

		this.protobufToken = this.protobufEncoder.Encode(this.Scenario);

		this.vipaqHeaderRow = ViPaqHeader.Create(this.Scenario, EncoderInfo.RowMajor);
		this.vipaqTokenRow = this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

		this.vipaqHeaderColumnar = ViPaqHeader.Create(this.Scenario, EncoderInfo.Columnar);
		this.vipaqTokenColumnar = this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.Columnar);
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackedResult Protobuf()
		=> this.protobufEncoder.Decode(this.protobufToken);

	[Benchmark]
	[BenchmarkOrder(20)]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq_Row()
		=> this.vipaqEncoder.Decode(this.vipaqTokenRow, this.vipaqHeaderRow);

	[Benchmark]
	[BenchmarkOrder(30)]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq_Columnar()
		=> this.vipaqEncoder.Decode(this.vipaqTokenColumnar, this.vipaqHeaderColumnar);
}
