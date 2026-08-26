namespace Binacle.TestsKernel.Algorithms.Models;

public class ScenarioResult
{
	// Keyed by algorithm, never by version - every version of an algorithm must agree, so there is no way to
	// name FFD_v1 and FFD_v2 apart.
	private readonly IReadOnlyDictionary<Algorithm, AlgorithmResult> byAlgorithm;

	public ScenarioResult(IReadOnlyDictionary<Algorithm, AlgorithmResult> byAlgorithm)
	{
		this.byAlgorithm = byAlgorithm;
	}

	public AlgorithmResult For(Algorithm algorithm)
	{
		if (!this.byAlgorithm.TryGetValue(algorithm, out var result))
		{
			throw new InvalidOperationException(
				$"No expected result for {algorithm}. The scenario names {string.Join(", ", this.byAlgorithm.Keys)}.");
		}

		return result;
	}
}
