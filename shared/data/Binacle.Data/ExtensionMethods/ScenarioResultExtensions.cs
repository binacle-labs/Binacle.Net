
namespace Binacle.Data.ExtensionMethods;

public static class ScenarioResultExtensions
{
	public static void EvaluateResult(this AlgorithmResult expected, OperationResult actual)
	{
		var expectedStatus = actual.AlgorithmOperation switch
		{
			AlgorithmOperation.Packing => expected.PackingStatus,
			AlgorithmOperation.Fitting => expected.FittingStatus,
			_ => throw new InvalidOperationException($"Unsupported algorithm operation: {actual.AlgorithmOperation}")
		} ;
		if (expectedStatus != actual.Status)
		{
			throw new InvalidOperationException($"Operation result status mismatch. Expected: {expectedStatus}, Actual: {actual.Status}");
		}
	}
	
}
