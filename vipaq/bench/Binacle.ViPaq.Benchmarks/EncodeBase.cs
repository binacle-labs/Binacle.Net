using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Testing.Json;
using Binacle.ViPaq.Testing.Protobuf;
using Binacle.ViPaq.Testing.ViPaq;

namespace Binacle.ViPaq.Benchmarks;

// The codec is NoOp, so this times the format alone; CompressionCost prices the codec.
public abstract class EncodeBase : BenchmarkBase
{
	private ProtobufEncoder protobufEncoder = null!;
	private ViPaqEncoder vipaqEncoder = null!;

	protected override Scenario Load(string name)
		=> TimingSet.GetByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();
		this.protobufEncoder = new ProtobufEncoder(new NoOpCodec());
		this.vipaqEncoder = new ViPaqEncoder(new NoOpCodec());
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public byte[] Protobuf()
		=> this.protobufEncoder.Encode(this.Scenario);

	[Benchmark]
	[BenchmarkOrder(20)]
	public byte[] ViPaq_Row()
		=> this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	[BenchmarkOrder(30)]
	public byte[] ViPaq_Columnar()
		=> this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.Columnar);

	// Text, not bytes, and no codec - the same form the size results measure.
	[Benchmark]
	[BenchmarkOrder(40)]
	public string Json()
		=> JsonEncoder.Encode(this.Scenario);
}
