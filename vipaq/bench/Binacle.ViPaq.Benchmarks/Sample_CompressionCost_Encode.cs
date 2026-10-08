namespace Binacle.ViPaq.Benchmarks;

[MemoryDiagnoser]
public class Sample_CompressionCost_Encode : CompressionCostBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public byte[] NoOp()
		=> this.NoOpEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	[BenchmarkOrder(20)]
	public byte[] Deflate()
		=> this.DeflateEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	[BenchmarkOrder(30)]
	public byte[] Gzip()
		=> this.GzipEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
}
