namespace Binacle.ViPaq.Benchmarks;

[MemoryDiagnoser]
public class Sample_Encode : EncodeBase
{
	[ParamsSource(typeof(CuratedScenarioProvider), nameof(CuratedScenarioProvider.GetScenarioNames))]
	public override string ScenarioName { get; set; } = "";
}
