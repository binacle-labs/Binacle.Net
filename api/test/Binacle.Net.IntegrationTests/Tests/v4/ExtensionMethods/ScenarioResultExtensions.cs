using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.Net.v4.Contracts.Pack;
using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.Net.IntegrationTests.v4.ExtensionMethods;

internal static class ScenarioResultExtensions
{
	public static void EvaluateResult(
		this ScenarioResult expected,
		FitBinResponse actual,
		OperationParameters parameters)
	{
		var expectedForAlgorithm = expected.For(parameters.GetAlgorithm()!.Value);

		var expectedStatus = Binacle.Net.v4.ExtensionMethods.FittingMapperExtensions.MapToBinFitResultStatus(
			expectedForAlgorithm.FittingStatus);
		actual.Status.ShouldBe(expectedStatus);

		var expectedEarlyExitReason = Binacle.Net.v4.ExtensionMethods.FittingMapperExtensions.MapToBinFitEarlyExitReason(
			expectedForAlgorithm.FittingEarlyExitReason);
		actual.EarlyExitReason.ShouldBe(expectedEarlyExitReason);
	}

	public static void EvaluateResult(
		this ScenarioResult expected,
		PackBinResponse actual,
		OperationParameters parameters)
	{
		var expectedStatus = Binacle.Net.v4.ExtensionMethods.PackingMapperExtensions.MapToBinPackResultStatus(
			expected.For(parameters.GetAlgorithm()!.Value).PackingStatus);
		actual.Status.ShouldBe(expectedStatus);
	}
}
