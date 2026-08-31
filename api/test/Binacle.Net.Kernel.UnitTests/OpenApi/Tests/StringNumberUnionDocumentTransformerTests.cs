using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// The web JSON defaults accept a number as a number or a numeric string, and ASP.NET mirrors that as an
// `[integer, string]` union, which makes a generated SDK type the property `int | string`. The document is
// collapsed back; runtime parsing is untouched, so the server still accepts numeric strings.
[Trait("Behavioral Tests", "Ensures numeric properties are not documented as string unions")]
public class StringNumberUnionDocumentTransformerTests
{
	private static async Task<OpenApiSchema> Transform(OpenApiSchema property)
	{
		var document = new OpenApiDocument
		{
			Components = new OpenApiComponents
			{
				Schemas = new Dictionary<string, IOpenApiSchema>
				{
					["Request"] = new OpenApiSchema
					{
						Properties = new Dictionary<string, IOpenApiSchema> { ["length"] = property },
					},
				},
			},
		};

		await new StringNumberUnionDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		);

		return property;
	}

	[Fact]
	public async Task An_Integer_String_Union_Collapses_To_Integer()
	{
		var property = await Transform(new OpenApiSchema
		{
			Type = JsonSchemaType.Integer | JsonSchemaType.String,
			Pattern = "^-?(?:0|[1-9]\\d*)$",
		});

		property.Type.ShouldBe(JsonSchemaType.Integer);
	}

	// The pattern only exists to describe the string form, so it goes with the branch it described.
	[Fact]
	public async Task The_String_Form_Pattern_Goes_With_It()
	{
		var property = await Transform(new OpenApiSchema
		{
			Type = JsonSchemaType.Number | JsonSchemaType.String,
			Pattern = "^-?\\d+(\\.\\d+)?$",
		});

		property.Type.ShouldBe(JsonSchemaType.Number);
		property.Pattern.ShouldBeNull();
	}

	// A null branch is how a nullable property is described, and dropping it here would undo the required-nullable
	// handling that runs alongside this.
	[Fact]
	public async Task A_Null_Branch_Survives()
	{
		var property = await Transform(new OpenApiSchema
		{
			Type = JsonSchemaType.Integer | JsonSchemaType.String | JsonSchemaType.Null,
		});

		property.Type.ShouldBe(JsonSchemaType.Integer | JsonSchemaType.Null);
	}

	[Fact]
	public async Task A_Property_That_Is_Only_A_String_Is_Left_Alone()
	{
		var property = await Transform(new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Pattern = "^[a-z]+$",
		});

		property.Type.ShouldBe(JsonSchemaType.String);
		property.Pattern.ShouldBe("^[a-z]+$");
	}

	// A schema with no type at all is what a `$ref` or a free-form object looks like here.
	[Fact]
	public async Task A_Property_With_No_Type_Is_Left_Alone()
	{
		var property = await Transform(new OpenApiSchema { Pattern = "^[a-z]+$" });

		property.Type.ShouldBeNull();
		property.Pattern.ShouldBe("^[a-z]+$");
	}

	[Fact]
	public async Task A_Schema_With_No_Properties_Is_Skipped()
	{
		var document = new OpenApiDocument
		{
			Components = new OpenApiComponents
			{
				Schemas = new Dictionary<string, IOpenApiSchema> { ["Empty"] = new OpenApiSchema() },
			},
		};

		await Should.NotThrowAsync(() => new StringNumberUnionDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		));
	}

	[Fact]
	public async Task A_Document_With_No_Components_Is_Left_Alone()
	{
		var document = new OpenApiDocument();

		await Should.NotThrowAsync(() => new StringNumberUnionDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		));

		document.Components.ShouldBeNull();
	}
}
