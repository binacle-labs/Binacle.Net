using Binacle.Net.v3.Contracts;
using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.Net.IntegrationTests.v3.ExtensionMethods;

internal static class ScenarioResultExtensions
{
	public static void EvaluateResult<T>(
		this ScenarioResult expected,
		Binacle.Net.v3.Contracts.BinFitResult actual,
		T withAlgorithm)
	where T: IWithAlgorithm
	{
		var expectedForAlgorithm = expected.For(withAlgorithm.Algorithm!.Value.ToLibAlgorithm());
		var expectedStatus = Binacle.Net.v3.Contracts.FitResponse.MapResultStatus(
			expectedForAlgorithm.FittingStatus, expectedForAlgorithm.FittingEarlyExitReason);
		actual.Result.ShouldBe(expectedStatus);
	}

	public static void EvaluateResult<T>(
		this ScenarioResult expected,
		Binacle.Net.v3.Contracts.BinPackResult actual,
		T withAlgorithm)
	where T: IWithAlgorithm
	{
		var expectedForAlgorithm = expected.For(withAlgorithm.Algorithm!.Value.ToLibAlgorithm());
		var expectedStatus = Binacle.Net.v3.Contracts.PackResponse.MapResultStatus(
			expectedForAlgorithm.PackingStatus, expectedForAlgorithm.PackingEarlyExitReason);

		actual.Result.ShouldBe(expectedStatus);
	}
}
