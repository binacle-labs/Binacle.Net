using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

namespace Binacle.Net.ServiceModule.IntegrationTests.Telemetry;

[Trait("Telemetry Tests", "Ensures the built-in ASP.NET Core metrics arrive with the ServiceModule on")]
[Collection(nameof(RateLimiterCollection))]
public class RateLimitingMetrics
{
	private const string routePath = "/api/auth/token";

	[Fact(DisplayName = "A request past the limit records aspnetcore.rate_limiting.requests as rejected")]
	public async Task ARequestPastTheLimit_Records_ARejection()
	{
		await using var api = new RateLimitedBinacleApi();
		var client = api.CreateClient();
		using var recorder = new InstrumentRecorder(
			api.Services,
			"Microsoft.AspNetCore.RateLimiting",
			"aspnetcore.rate_limiting.requests"
		);

		HttpResponseMessage response = null!;
		for (var request = 1; request <= RateLimitedBinacleApi.AuthTokenPermitLimit + 1; request++)
		{
			response = await client.PostAsync(
				routePath,
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);
		}

		response.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);

		// ASP.NET Core names a rejection by the limiter that made it, not "rejected".
		recorder.Measurements.ShouldContain(tags =>
			Equals(tags.GetValueOrDefault("aspnetcore.rate_limiting.result"), "endpoint_limiter")
			&& Equals(tags.GetValueOrDefault("aspnetcore.rate_limiting.policy"), "AuthToken")
		);
	}
}
