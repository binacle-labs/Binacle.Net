using System.Net;

namespace Binacle.Net.IntegrationTests;

// Regression cover for the 2026-09-01 break: a preflight from the demo origin came back with no
// Access-Control-Allow-Origin header. Program.cs registers the CoreApi policy unconditionally and every
// core endpoint requires it, so /api/v4/presets is enough to exercise it - no need for a real packing request.
[Trait("Behavioral Tests", "Ensures the CORS policy allows a configured origin and nothing else")]
public class CorsTests : IClassFixture<CorsConfiguredBinacleApi>
{
	private const string routePath = "/api/v4/presets";
	private const string allowOriginHeaderName = "Access-Control-Allow-Origin";
	private const string unconfiguredOrigin = "https://not-allowed.example.com";

	private readonly BinacleApi defaultApi;
	private readonly CorsConfiguredBinacleApi configuredApi;

	public CorsTests(BinacleApi defaultApi, CorsConfiguredBinacleApi configuredApi)
	{
		this.defaultApi = defaultApi;
		this.configuredApi = configuredApi;
	}

	[Fact(DisplayName = $"OPTIONS {routePath}. Preflight From The Configured Origin Returns Access-Control-Allow-Origin")]
	public async Task Preflight_FromConfiguredOrigin_ReturnsAllowOriginHeader()
	{
		var request = BuildPreflightRequest(CorsConfiguredBinacleApi.AllowedOrigin);

		var response = await this.configuredApi.Client.SendAsync(request, TestContext.Current.CancellationToken);

		response.Headers.Contains(allowOriginHeaderName).ShouldBeTrue();
		response.Headers.GetValues(allowOriginHeaderName).ShouldContain(CorsConfiguredBinacleApi.AllowedOrigin);
	}

	[Fact(DisplayName = $"GET {routePath}. Request From The Configured Origin Returns Access-Control-Allow-Origin")]
	public async Task Get_FromConfiguredOrigin_ReturnsAllowOriginHeader()
	{
		var request = BuildSimpleRequest(CorsConfiguredBinacleApi.AllowedOrigin);

		var response = await this.configuredApi.Client.SendAsync(request, TestContext.Current.CancellationToken);

		response.Headers.Contains(allowOriginHeaderName).ShouldBeTrue();
		response.Headers.GetValues(allowOriginHeaderName).ShouldContain(CorsConfiguredBinacleApi.AllowedOrigin);
	}

	[Fact(DisplayName = $"OPTIONS {routePath}. Preflight From An Unconfigured Origin Has No Access-Control-Allow-Origin")]
	public async Task Preflight_FromUnconfiguredOrigin_HasNoAllowOriginHeader()
	{
		var request = BuildPreflightRequest(unconfiguredOrigin);

		var response = await this.configuredApi.Client.SendAsync(request, TestContext.Current.CancellationToken);

		// Without this a 404 would satisfy the header assertion and the test would guard nothing.
		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
		response.Headers.Contains(allowOriginHeaderName).ShouldBeFalse();
	}

	[Fact(DisplayName = $"OPTIONS {routePath}. With No Cors.json, Preflight Has No Access-Control-Allow-Origin For Any Origin")]
	public async Task Preflight_WithNoCorsConfigured_HasNoAllowOriginHeader()
	{
		var request = BuildPreflightRequest(unconfiguredOrigin);

		var response = await this.defaultApi.Client.SendAsync(request, TestContext.Current.CancellationToken);

		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
		response.Headers.Contains(allowOriginHeaderName).ShouldBeFalse();
	}

	private static HttpRequestMessage BuildPreflightRequest(string origin)
	{
		var request = new HttpRequestMessage(HttpMethod.Options, routePath);
		request.Headers.Add("Origin", origin);
		request.Headers.Add("Access-Control-Request-Method", HttpMethod.Get.Method);
		return request;
	}

	private static HttpRequestMessage BuildSimpleRequest(string origin)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, routePath);
		request.Headers.Add("Origin", origin);
		return request;
	}
}
