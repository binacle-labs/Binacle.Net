
namespace Binacle.TestsKernel.Algorithms.Helpers;

public static class ScenarioResultHelper
{
	private static Models.AlgorithmResult ParseFromCompactString(string compactString)
	{
		var parts = compactString.Split(' ');
		if (parts.Length != 2)
		{
			throw new ArgumentException(
				$"Invalid format. Value {compactString} should have format '{{Packing Operation Result Status}} {{Fitting Operation Result Status}}'.");
		}

		var (packingStatus, packingEarlyExitReason) = ParseOperationResultStatus(parts[0]);
		var (fittingStatus, fittingEarlyExitReason) = ParseOperationResultStatus(parts[1]);
		return new Models.AlgorithmResult(
			packingStatus,
			packingEarlyExitReason,
			fittingStatus,
			fittingEarlyExitReason
		);
	}

	public static Models.ScenarioResult ParseFromMap(IReadOnlyDictionary<string, string> byAlgorithm)
	{
		var parsed = new Dictionary<Algorithm, Models.AlgorithmResult>();
		foreach (var (name, compactString) in byAlgorithm)
		{
			// Enum.TryParse also takes a number and any value the enum does not define, so the round trip is
			// what rejects "0" and "ffd".
			if (!Enum.TryParse(name, out Algorithm algorithm) || algorithm.ToString() != name)
			{
				throw new ArgumentException(
					$"Invalid algorithm. Value {name} should be one of {string.Join(", ", Enum.GetNames<Algorithm>())}.");
			}

			if (!parsed.TryAdd(algorithm, ParseFromCompactString(compactString)))
			{
				throw new ArgumentException($"Invalid format. Algorithm {algorithm} is named more than once.");
			}
		}

		var missing = Enum.GetValues<Algorithm>().Where(x => !parsed.ContainsKey(x)).ToArray();
		if (missing.Length > 0)
		{
			throw new ArgumentException(
				$"Invalid format. A result must name every algorithm. Missing {string.Join(", ", missing)}.");
		}

		return new Models.ScenarioResult(parsed);
	}

	private static (OperationResultStatus resultStatus, EarlyExitReason earlyExitReason) ParseOperationResultStatus(
		string compactString)
	{
		if (!compactString.Contains('-'))
		{
			if (!Enum.TryParse(compactString, out OperationResultStatus parsedStatus))
			{
				throw new ArgumentException(
					$"Invalid Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
			}

			return (parsedStatus, EarlyExitReason.None);
		}

		var parts = compactString.Split('-');
		if (parts.Length != 2)
		{
			throw new ArgumentException(
				$"Invalid format. Value {compactString} should have format '{{Operation Result Status}}-{{Early Exit Reason}}'.");
		}

		if (!Enum.TryParse(parts[0], out OperationResultStatus status))
		{
			throw new ArgumentException(
				$"Invalid Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
		}

		if (!Enum.TryParse(parts[1], out EarlyExitReason earlyExitReason))
		{
			throw new ArgumentException(
				$"Invalid Early Exit Reason format. Value {compactString} should be a valid EarlyExitReason.");
		}

		return (status, earlyExitReason);
	}
}
