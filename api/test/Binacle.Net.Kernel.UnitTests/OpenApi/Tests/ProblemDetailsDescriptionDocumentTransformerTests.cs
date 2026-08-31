using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// The framework's own error schemas carry no description, because no contract in this repo owns them. Without
// this every error response in the document is an undocumented blob.
[Trait("Behavioral Tests", "Ensures the framework error schemas are described")]
public class ProblemDetailsDescriptionDocumentTransformerTests
{
	private static OpenApiSchema SchemaWith(params string[] propertyNames)
		=> new()
		{
			Properties = propertyNames.ToDictionary(
				name => name,
				IOpenApiSchema (_) => new OpenApiSchema()
			),
		};

	private static async Task<OpenApiDocument> Transform(params (string Name, OpenApiSchema Schema)[] schemas)
	{
		var document = new OpenApiDocument
		{
			Components = new OpenApiComponents
			{
				Schemas = schemas.ToDictionary(x => x.Name, IOpenApiSchema (x) => x.Schema),
			},
		};

		await new ProblemDetailsDescriptionDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		);

		return document;
	}

	[Fact]
	public async Task The_Error_Schema_And_Its_Fields_Are_Described()
	{
		var schema = SchemaWith("type", "title", "status", "detail", "instance");

		await Transform(("ProblemDetails", schema));

		schema.Description.ShouldNotBeNullOrWhiteSpace();
		foreach (var property in schema.Properties!.Values)
		{
			property.Description.ShouldNotBeNullOrWhiteSpace();
		}
	}

	[Fact]
	public async Task The_Validation_Error_Schema_Describes_Its_Errors_Field()
	{
		var schema = SchemaWith("title", "errors");

		await Transform(("HttpValidationProblemDetails", schema));

		schema.Description.ShouldNotBeNullOrWhiteSpace();
		schema.Properties!["errors"].Description.ShouldNotBeNullOrWhiteSpace();
	}

	// The two schemas only appear once something returns them, so a document without them is normal.
	[Fact]
	public async Task A_Document_Without_Them_Is_Left_Alone()
	{
		var unrelated = SchemaWith("length");

		await Transform(("PackRequest", unrelated));

		unrelated.Description.ShouldBeNull();
		unrelated.Properties!["length"].Description.ShouldBeNull();
	}

	[Fact]
	public async Task A_Document_With_No_Components_Is_Left_Alone()
	{
		var document = new OpenApiDocument();

		await Should.NotThrowAsync(() => new ProblemDetailsDescriptionDocumentTransformer().TransformAsync(
			document,
			TransformerContexts.ForDocument(),
			CancellationToken.None
		));

		document.Components.ShouldBeNull();
	}

	[Fact]
	public async Task An_Error_Schema_With_No_Properties_Still_Gets_Its_Own_Description()
	{
		var schema = new OpenApiSchema();

		await Transform(("ProblemDetails", schema));

		schema.Description.ShouldNotBeNullOrWhiteSpace();
	}

	[Fact]
	public async Task A_Property_The_Rfc_Does_Not_Name_Is_Left_Alone()
	{
		var schema = SchemaWith("title", "traceId");

		await Transform(("ProblemDetails", schema));

		schema.Properties!["title"].Description.ShouldNotBeNullOrWhiteSpace();
		schema.Properties["traceId"].Description.ShouldBeNull();
	}
}
