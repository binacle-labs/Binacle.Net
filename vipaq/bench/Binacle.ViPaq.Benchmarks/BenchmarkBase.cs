namespace Binacle.ViPaq.Benchmarks;

public abstract class BenchmarkBase
{
	// Each class points its column source at its own provider.
	public abstract string ScenarioName { get; set; }

	protected Scenario Scenario { get; private set; } = null!;

	protected abstract Scenario Load(string name);

	[GlobalSetup]
	public virtual void GlobalSetup() => this.Scenario = this.Load(this.ScenarioName);
}
