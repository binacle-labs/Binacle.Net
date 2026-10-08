using System.Net;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints;

// The module registers its own ServiceApi policy and every route in it requires it. The token route has no
// auth and the admin group has, and a preflight carries no token, so both are checked the same way.
[Trait("Behavioral Tests", "Ensures the ServiceModule CORS policy allows a configured origin and nothing else")]
public class CorsTests
{
	private const string tokenPath = "/api/auth/token";
	private const string accountsPath = "/api/admin/accounts";
	private const string allowOriginHeaderName = "Access-Control-Allow-Origin";
	private const string unconfiguredOrigin = "https://not-allowed.example.com";

	private readonly BinacleApi sut;

	public CorsTests(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Theory(DisplayName = "OPTIONS. Preflight From The Configured Origin Returns Access-Control-Allow-Origin")]
	[InlineData(tokenPath, "POST")]
	[InlineData(accountsPath, "GET")]
	public async Task Preflight_FromConfiguredOrigin_ReturnsAllowOriginHeader(string path, string method)
	{
		var request = BuildPreflightRequest(path, method, BinacleApi.AllowedCorsOrigin);

		var response = await this.sut.Client.SendAsync(request, TestContext.Current.CancellationToken);

		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
		response.Headers.Contains(allowOriginHeaderName).ShouldBeTrue();
		response.Headers.GetValues(allowOriginHeaderName).ShouldContain(BinacleApi.AllowedCorsOrigin);
	}

	[Theory(DisplayName = "OPTIONS. Preflight From An Unconfigured Origin Has No Access-Control-Allow-Origin")]
	[InlineData(tokenPath, "POST")]
	[InlineData(accountsPath, "GET")]
	public async Task Preflight_FromUnconfiguredOrigin_HasNoAllowOriginHeader(string path, string method)
	{
		var request = BuildPreflightRequest(path, method, unconfiguredOrigin);

		var response = await this.sut.Client.SendAsync(request, TestContext.Current.CancellationToken);

		// Without this a 404 would satisfy the header assertion and the test would guard nothing.
		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
		response.Headers.Contains(allowOriginHeaderName).ShouldBeFalse();
	}

	private static HttpRequestMessage BuildPreflightRequest(string path, string method, string origin)
	{
		var request = new HttpRequestMessage(HttpMethod.Options, path);
		request.Headers.Add("Origin", origin);
		request.Headers.Add("Access-Control-Request-Method", method);
		return request;
	}
}
