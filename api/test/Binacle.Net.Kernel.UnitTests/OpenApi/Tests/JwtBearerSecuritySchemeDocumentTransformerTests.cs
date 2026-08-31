using Binacle.Net.Kernel.OpenApi.Transformers;
using Binacle.Net.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// Whether the document defines a Bearer scheme at all. The Service Module is what registers it, so a build
// without that module must not describe an authentication scheme nothing implements.
[Trait("Behavioral Tests", "Ensures the Bearer scheme is defined only where it is registered")]
public class JwtBearerSecuritySchemeDocumentTransformerTests
{
	private static async Task<OpenApiDocument> Transform(
		IOptionalDependency<IAuthenticationSchemeProvider> schemeProvider
	)
	{
		var document = new OpenApiDocument();
		await new JwtBearerSecuritySchemeDocumentTransformer(schemeProvider).TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		);
		return document;
	}

	[Fact]
	public async Task A_Registered_Bearer_Scheme_Is_Described_As_An_Http_Bearer_Token()
	{
		var document = await Transform(AuthenticationSchemeProviders.With("Bearer"));

		var scheme = document.Components!.SecuritySchemes!["Bearer"];
		scheme.Type.ShouldBe(SecuritySchemeType.Http);
		scheme.Scheme.ShouldBe("bearer");
		scheme.In.ShouldBe(ParameterLocation.Header);
		scheme.BearerFormat.ShouldNotBeNullOrWhiteSpace();
	}

	[Fact]
	public async Task No_Authentication_At_All_Describes_No_Scheme()
	{
		var document = await Transform(AuthenticationSchemeProviders.Absent());

		document.Components.ShouldBeNull();
	}

	[Fact]
	public async Task Authentication_Without_Bearer_Describes_No_Scheme()
	{
		var document = await Transform(AuthenticationSchemeProviders.With("Cookies"));

		document.Components.ShouldBeNull();
	}
}
