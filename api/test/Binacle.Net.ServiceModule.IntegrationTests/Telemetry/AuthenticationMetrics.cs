using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;

namespace Binacle.Net.ServiceModule.IntegrationTests.Telemetry;

[Trait("Telemetry Tests", "Ensures the built-in ASP.NET Core metrics arrive with the ServiceModule on")]
public class AuthenticationMetrics
{
	private readonly BinacleApi sut;

	public AuthenticationMetrics(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/admin/accounts";

	[Fact(DisplayName = "A request with a bearer token records aspnetcore.authentication.authenticate.duration")]
	public async Task ARequestWithABearerToken_Records_AnAuthentication()
	{
		using var recorder = new InstrumentRecorder(
			this.sut.Services,
			"Microsoft.AspNetCore.Authentication",
			"aspnetcore.authentication.authenticate.duration"
		);
		var client = this.sut.CreateClient();
		using var authentication = this.sut.StartAuthenticationScope(client, this.sut.Admin);

		var response = await client.GetAsync(routePath, TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		recorder.Measurements.ShouldContain(tags =>
			Equals(tags.GetValueOrDefault("aspnetcore.authentication.result"), "success")
			&& Equals(tags.GetValueOrDefault("aspnetcore.authentication.scheme"), "Bearer")
		);
	}
}
