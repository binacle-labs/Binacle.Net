using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// Whether an endpoint documents a 429. The core marks endpoints as rate-limited without supplying a limiter, so
// the attribute an optional module attaches is the only thing that says a build actually limits anybody.
[Trait("Behavioral Tests", "Ensures 429 is documented only where a limiter is attached")]
public class RateLimiterResponseOperationTransformerTests
{
	private static async Task<OpenApiOperation> Transform(params object[] endpointMetadata)
	{
		var operation = new OpenApiOperation { Responses = new OpenApiResponses() };
		await new RateLimiterResponseOperationTransformer().TransformAsync(
			operation,
			TransformerContexts.ForOperation(endpointMetadata),
			CancellationToken.None
		);
		return operation;
	}

	[Fact]
	public async Task An_Endpoint_With_A_Limiter_Documents_429()
	{
		var operation = await Transform(new EnableRateLimitingAttribute("AnyPolicy"));

		operation.Responses!.ShouldContainKey("429");
		operation.Responses["429"].Description
			.ShouldBe(RateLimiterResponseOperationTransformer.OpenApiResponseFor429TooManyRequests.Description);
	}

	// The build that limits nobody must not promise a 429 it will never return.
	[Fact]
	public async Task An_Endpoint_With_No_Limiter_Documents_No_429()
	{
		var operation = await Transform();

		operation.Responses!.ShouldNotContainKey("429");
	}
}
