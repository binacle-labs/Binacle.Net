using Binacle.Net.Kernel.OpenApi.Transformers;
using Binacle.Net.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// Which operations are marked as needing a token. Both halves have to hold: the endpoint asks for authorization,
// and the build actually registered a Bearer scheme.
[Trait("Behavioral Tests", "Ensures only authorized operations carry a security requirement")]
public class JwtBearerSecuritySchemeOperationTransformerTests
{
	private static async Task<OpenApiOperation> Transform(
		IOptionalDependency<IAuthenticationSchemeProvider> schemeProvider,
		params object[] endpointMetadata
	)
	{
		var operation = new OpenApiOperation { Security = [] };
		await new JwtBearerSecuritySchemeOperationTransformer(schemeProvider).TransformAsync(
			operation,
			TransformerContexts.ForOperation(endpointMetadata),
			CancellationToken.None
		);
		return operation;
	}

	[Fact]
	public async Task An_Authorized_Operation_Gets_A_Security_Requirement()
	{
		var operation = await Transform(
			AuthenticationSchemeProviders.With("Bearer"),
			new AuthorizeAttribute()
		);

		operation.Security!.Count.ShouldBe(1);
	}

	[Fact]
	public async Task An_Anonymous_Operation_Gets_Nothing()
	{
		var operation = await Transform(AuthenticationSchemeProviders.With("Bearer"));

		operation.Security!.ShouldBeEmpty();
	}

	// The Service Module is what registers Bearer. With it off, an [Authorize] left behind must not advertise a
	// scheme the document does not define.
	[Fact]
	public async Task No_Bearer_Scheme_Means_No_Security_Requirement()
	{
		var withOtherScheme = await Transform(
			AuthenticationSchemeProviders.With("Cookies"),
			new AuthorizeAttribute()
		);
		withOtherScheme.Security!.ShouldBeEmpty();

		var withNoProvider = await Transform(
			AuthenticationSchemeProviders.Absent(),
			new AuthorizeAttribute()
		);
		withNoProvider.Security!.ShouldBeEmpty();
	}
}
