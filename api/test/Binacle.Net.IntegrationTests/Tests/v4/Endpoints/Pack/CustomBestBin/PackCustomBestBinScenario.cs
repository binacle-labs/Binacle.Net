using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Binacle.Data;
using Binacle.Net.IntegrationTests.v4.ExtensionMethods;
using CustomProblems = Binacle.Data.CustomProblems.DataProvider;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Pack.CustomBestBin;

// Sends the scenario's bin as a one-element list. With one bin to choose from, selection cannot change the
// answer, so the result must match what the single-bin endpoint produces for the same geometry.
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackCustomBestBinScenario
{
	private const string routePath = "/api/v4/pack/best-bin";

	private readonly BinacleApi sut;

	public PackCustomBestBinScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Theory]
	[MemberData(nameof(CustomProblems.TheoryNames), MemberType = typeof(CustomProblems))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = All.GetByName(scenarioName);

		var request = new PackCustomBestBinRequest
		{
			Parameters = new() { Algorithm = Binacle.Net.v4.Contracts.Algorithm.FFD },
			Bins =
			[
				new()
				{
					ID = scenario.Bin.ID,
					Length = scenario.Bin.Length,
					Width = scenario.Bin.Width,
					Height = scenario.Bin.Height
				}
			],
			Items = scenario.Items.Select(x => new Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<PackBinResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(scenario.Bin.ID);
		result.Bin.CalculateVolume().ShouldBe(scenario.Metrics.BinVolume);

		var itemsCount = (result.PackedItems?.Count ?? 0)
		                 + (result.UnpackedItems?.Sum(x => x.Quantity) ?? 0);
		itemsCount.ShouldBe(scenario.Metrics.ItemsCount);

		result.PackedBinVolumePercentage
			.ShouldBeLessThanOrEqualTo(scenario.Metrics.Percentage, new PercentageComparer());

		scenario.Result.EvaluateResult(result, request.Parameters);
	}
}
