using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// A required property typed `SomeEnum?` renders as `oneOf: [null, X]`, which a generated client reads as "null is
// allowed" while the validator answers 422. The nullable C# type is deliberate, so the schema is what gives.
[Trait("Behavioral Tests", "Ensures a required property is not documented as nullable")]
public class RequiredNullableSchemaDocumentTransformerTests
{
	private static OpenApiSchema NullBranch() => new() { Type = JsonSchemaType.Null };
	private static OpenApiSchema ValueBranch() => new() { Type = JsonSchemaType.String };

	private static async Task<OpenApiSchema> Transform(OpenApiSchema schema)
	{
		var document = new OpenApiDocument
		{
			Components = new OpenApiComponents
			{
				Schemas = new Dictionary<string, IOpenApiSchema> { ["Request"] = schema },
			},
		};

		await new RequiredNullableSchemaDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		);

		return schema;
	}

	[Fact]
	public async Task The_Null_Branch_Is_Dropped_From_A_Required_Property()
	{
		var value = ValueBranch();
		var schema = await Transform(new OpenApiSchema
		{
			Required = new HashSet<string> { "algorithm" },
			Properties = new Dictionary<string, IOpenApiSchema>
			{
				["algorithm"] = new OpenApiSchema { OneOf = [NullBranch(), value] },
			},
		});

		schema.Properties!["algorithm"].ShouldBeSameAs(value);
	}

	// Only the null branch is the problem. A real union of two value types is a contract decision and stays.
	[Fact]
	public async Task A_OneOf_With_No_Null_Branch_Is_Left_Alone()
	{
		var union = new OpenApiSchema { OneOf = [ValueBranch(), ValueBranch()] };
		var schema = await Transform(new OpenApiSchema
		{
			Required = new HashSet<string> { "algorithm" },
			Properties = new Dictionary<string, IOpenApiSchema> { ["algorithm"] = union },
		});

		schema.Properties!["algorithm"].ShouldBeSameAs(union);
	}

	[Fact]
	public async Task An_Optional_Property_Keeps_Its_Null_Branch()
	{
		var nullable = new OpenApiSchema { OneOf = [NullBranch(), ValueBranch()] };
		var schema = await Transform(new OpenApiSchema
		{
			Properties = new Dictionary<string, IOpenApiSchema> { ["algorithm"] = nullable },
		});

		schema.Properties!["algorithm"].ShouldBeSameAs(nullable);
	}

	// The three shapes that are not the one being fixed. Each has to fall through untouched, or a schema the
	// generator wrote correctly gets rewritten.
	[Fact]
	public async Task A_Required_Name_With_No_Property_Behind_It_Is_Skipped()
	{
		var schema = await Transform(new OpenApiSchema
		{
			Required = new HashSet<string> { "missing" },
			Properties = new Dictionary<string, IOpenApiSchema> { ["algorithm"] = ValueBranch() },
		});

		schema.Properties!.ShouldNotContainKey("missing");
	}

	[Fact]
	public async Task A_Referenced_Property_Is_Skipped()
	{
		var reference = new OpenApiSchemaReference("Algorithm");
		var schema = await Transform(new OpenApiSchema
		{
			Required = new HashSet<string> { "algorithm" },
			Properties = new Dictionary<string, IOpenApiSchema> { ["algorithm"] = reference },
		});

		schema.Properties!["algorithm"].ShouldBeSameAs(reference);
	}

	[Fact]
	public async Task A_Property_That_Is_Not_A_Two_Branch_Union_Is_Skipped()
	{
		var plain = ValueBranch();
		var threeWay = new OpenApiSchema { OneOf = [NullBranch(), ValueBranch(), ValueBranch()] };

		var schema = await Transform(new OpenApiSchema
		{
			Required = new HashSet<string> { "plain", "threeWay" },
			Properties = new Dictionary<string, IOpenApiSchema>
			{
				["plain"] = plain,
				["threeWay"] = threeWay,
			},
		});

		schema.Properties!["plain"].ShouldBeSameAs(plain);
		schema.Properties["threeWay"].ShouldBeSameAs(threeWay);
	}

	[Fact]
	public async Task A_Document_With_No_Components_Is_Left_Alone()
	{
		var document = new OpenApiDocument();

		await Should.NotThrowAsync(() => new RequiredNullableSchemaDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		));

		document.Components.ShouldBeNull();
	}
}
