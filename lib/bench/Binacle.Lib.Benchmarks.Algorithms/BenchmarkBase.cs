using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib.Benchmarks.Algorithms;

public abstract class BenchmarkBase
{
	private Scenario? scenario;

	// Each tier gets its scenarios from its own provider.
	protected abstract Scenario Load();

	protected abstract AlgorithmOperation Operation { get; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.scenario = this.Load();
	}

	protected OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> factory)
		=> factory(this.scenario!.Bin, this.scenario.Items)
			.Execute(new TestOperationParameters { Operation = this.Operation });
}
