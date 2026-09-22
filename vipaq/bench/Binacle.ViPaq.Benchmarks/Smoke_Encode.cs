namespace Binacle.ViPaq.Benchmarks;

// The fixed cost, the typical pack and the largest real one.
[MemoryDiagnoser]
public class Smoke_Encode : EncodeBase
{
	[Params("one item", "typical container", "largest real pack")]
	public override string ScenarioName { get; set; } = "";
}
