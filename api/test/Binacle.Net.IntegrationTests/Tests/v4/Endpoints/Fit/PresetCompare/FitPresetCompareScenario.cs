using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.Data;
using Binacle.Net.IntegrationTests.v4.ExtensionMethods;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Fit.PresetCompare;

// The custom-problems scenarios are defined against the bins of the custom-problems preset, so every scenario's
// bin appears in the compare results. The scenario's expected outcome is asserted on that bin's own result.
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitPresetCompareScenario
{
	private const string routePath = "/api/v4/fit/compare-bins/{preset}";

	private readonly BinacleApi sut;

	public FitPresetCompareScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = All.GetByName(scenarioName);
		var url = routePath.Replace("{preset}", PresetKeys.CustomProblems);

		var request = new FitPresetCompareRequest
		{
			Parameters = new() { Algorithm = Binacle.Net.v4.Contracts.Algorithm.FFD },
			Items = scenario.Items.Select(x => new Binacle.Net.v4.Contracts.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(
			url,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var compareResult = await response.Content.ReadFromJsonAsync<FitCompareResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		compareResult.ShouldNotBeNull();

		var result = compareResult!.Results.FirstOrDefault(x => x.Bin.ID == scenario.Bin.ID);
		result.ShouldNotBeNull($"Compare returned no result for bin '{scenario.Bin.ID}'");

		result!.Bin.CalculateVolume().ShouldBe(scenario.Metrics.BinVolume);

		var itemsCount = (result.PackedItems?.Count ?? 0)
		                 + (result.UnpackedItems?.Sum(x => x.Quantity) ?? 0);
		itemsCount.ShouldBe(scenario.Metrics.ItemsCount);

		result.PackedBinVolumePercentage
			.ShouldBeLessThanOrEqualTo(scenario.Metrics.Percentage, new PercentageComparer());

		scenario.Result.EvaluateResult(result, request.Parameters);
	}
}
