using Binacle.Net.Kernel.OpenApi.Attributes;
using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// Which response properties a client can rely on being present. Nullability is read off the CLR type, so this is
// what keeps `required` in step with the contract instead of a hand-kept list.
[Trait("Behavioral Tests", "Ensures non-nullable response properties are marked required")]
public class RequiredNonNullableSchemaTransformerTests
{
	[OpenApiRequireNonNullable]
	private class Response
	{
		public int Number { get; set; }
		public int? NullableNumber { get; set; }
		public string Text { get; set; } = string.Empty;
		public string? NullableText { get; set; }
	}

	[OpenApiRequireNonNullable]
	private class BaseResponse
	{
		public int Number { get; set; }
	}

	private class DerivedResponse : BaseResponse
	{
		public string Text { get; set; } = string.Empty;
	}

	private class Unmarked
	{
		public int Number { get; set; }
	}

	private static async Task<OpenApiSchema> Transform<T>()
	{
		var schema = new OpenApiSchema();
		await new RequiredNonNullableSchemaTransformer().TransformAsync(
			schema,
			TransformerContexts.ForType<T>(),
			CancellationToken.None
		);
		return schema;
	}

	[Fact]
	public async Task Only_The_Non_Nullable_Properties_Are_Required()
	{
		var schema = await Transform<Response>();

		schema.Required.ShouldNotBeNull();
		schema.Required.Order().ShouldBe([nameof(Response.Number), nameof(Response.Text)]);
	}

	// The attribute is inherited, so a base response covers everything derived from it.
	[Fact]
	public async Task A_Derived_Type_Inherits_The_Marker()
	{
		var schema = await Transform<DerivedResponse>();

		schema.Required.ShouldNotBeNull();
		schema.Required.Order().ShouldBe([nameof(BaseResponse.Number), nameof(DerivedResponse.Text)]);
	}

	[Fact]
	public async Task An_Unmarked_Type_Gets_No_Required_Set()
	{
		var schema = await Transform<Unmarked>();

		schema.Required.ShouldBeNull();
	}
}
