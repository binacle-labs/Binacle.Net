namespace Binacle.Lib.Testing;

// What a scenario says the packer should do, checked against what it did.
public static class ScenarioChecks
{
	public static void EvaluateResult(this ScenarioMetrics metrics, OperationResult result)
	{
		var totalItemsVolume = result.TotalItemsVolume();
		if (metrics.ItemsVolume != totalItemsVolume)
		{
			throw new InvalidOperationException($"Items volume mismatch. Expected: {metrics.ItemsVolume}, Actual: {totalItemsVolume}");
		}
		if (metrics.BinVolume != result.Bin.Volume)
		{
			throw new InvalidOperationException($"Bin volume mismatch. Expected: {metrics.BinVolume}, Actual: {result.Bin.Volume}");
		}

		var totalItemsCount = result.TotalItemsCount();
		if (metrics.ItemsCount != totalItemsCount)
		{
			throw new InvalidOperationException($"Items count mismatch. Expected: {metrics.ItemsCount}, Actual: {totalItemsCount}");
		}

		var percentageResult = new PercentageComparer().Compare(result.PackedBinVolumePercentage, metrics.Percentage);
		if (percentageResult > 0)
		{
			throw new InvalidOperationException($"Packed volume percentage too high. Expected at most: {metrics.Percentage}, Actual: {result.PackedBinVolumePercentage}");
		}
	}

	public static void EvaluateResult(this AlgorithmResult expected, OperationResult actual)
	{
		var expectedStatus = actual.AlgorithmOperation switch
		{
			AlgorithmOperation.Packing => expected.PackingStatus,
			AlgorithmOperation.Fitting => expected.FittingStatus,
			_ => throw new InvalidOperationException($"Unsupported algorithm operation: {actual.AlgorithmOperation}")
		};
		if (expectedStatus != actual.Status)
		{
			throw new InvalidOperationException($"Operation result status mismatch. Expected: {expectedStatus}, Actual: {actual.Status}");
		}
	}
}
