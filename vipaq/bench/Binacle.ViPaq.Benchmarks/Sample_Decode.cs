namespace Binacle.ViPaq.Benchmarks;

[MemoryDiagnoser]
public class Sample_Decode : DecodeBase
{
	[ParamsSource(typeof(TimingSet), nameof(TimingSet.Names))]
	public override string ScenarioName { get; set; } = "";
}
