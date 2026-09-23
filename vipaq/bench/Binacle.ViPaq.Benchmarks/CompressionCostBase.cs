using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Testing.ViPaq;

namespace Binacle.ViPaq.Benchmarks;

// Prices the codec, which Encode and Decode leave out by running NoOp. NoOp passes the body straight through,
// so `Deflate - NoOp` is what deflate's squeezing costs and `Gzip - Deflate` is gzip's extra framing. Row-major.
public abstract class CompressionCostBase : BenchmarkBase
{
	[ParamsSource(typeof(CompressionCostSet), nameof(CompressionCostSet.Names))]
	public override string ScenarioName { get; set; } = "";

	protected ViPaqEncoder NoOpEncoder { get; private set; } = null!;
	protected ViPaqEncoder DeflateEncoder { get; private set; } = null!;
	protected ViPaqEncoder GzipEncoder { get; private set; } = null!;

	protected override Scenario Load(string name)
		=> CompressionCostSet.GetByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();
		this.NoOpEncoder = new ViPaqEncoder(new NoOpCodec());
		this.DeflateEncoder = new ViPaqEncoder(new DeflateCodec());
		this.GzipEncoder = new ViPaqEncoder(new GzipCodec());
	}
}
