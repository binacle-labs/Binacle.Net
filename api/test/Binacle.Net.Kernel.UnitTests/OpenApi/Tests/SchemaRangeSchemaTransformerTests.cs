using Binacle.Net.Kernel.OpenApi.Attributes;
using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// The documented minimum and maximum on a property. Nothing enforces them at the framework level, so this is the
// only place the declared range and the document are checked against each other.
[Trait("Behavioral Tests", "Ensures a declared schema range reaches the document")]
public class SchemaRangeSchemaTransformerTests
{
	private class Contract
	{
		[OpenApiSchemaRange(Minimum = 1, Maximum = 9)]
		public int Both { get; set; }

		[OpenApiSchemaRange(Minimum = 1)]
		public int MinimumOnly { get; set; }

		[OpenApiSchemaRange(Maximum = 9)]
		public int MaximumOnly { get; set; }

		public int Undeclared { get; set; }
	}

	private static async Task<OpenApiSchema> Transform(string propertyName)
	{
		var schema = new OpenApiSchema();
		await new SchemaRangeSchemaTransformer().TransformAsync(
			schema,
			TransformerContexts.ForProperty<Contract>(propertyName),
			CancellationToken.None
		);
		return schema;
	}

	[Fact]
	public async Task Both_Bounds_Reach_The_Schema()
	{
		var schema = await Transform(nameof(Contract.Both));

		schema.Minimum.ShouldBe("1");
		schema.Maximum.ShouldBe("9");
	}

	// NaN is what "unset" means on the attribute, so a one-sided range must leave the other bound off entirely
	// rather than writing NaN into the document.
	[Fact]
	public async Task A_Bound_That_Was_Not_Declared_Is_Left_Off()
	{
		var minimumOnly = await Transform(nameof(Contract.MinimumOnly));
		minimumOnly.Minimum.ShouldBe("1");
		minimumOnly.Maximum.ShouldBeNull();

		var maximumOnly = await Transform(nameof(Contract.MaximumOnly));
		maximumOnly.Minimum.ShouldBeNull();
		maximumOnly.Maximum.ShouldBe("9");
	}

	[Fact]
	public async Task A_Property_With_No_Attribute_Is_Left_Alone()
	{
		var schema = await Transform(nameof(Contract.Undeclared));

		schema.Minimum.ShouldBeNull();
		schema.Maximum.ShouldBeNull();
	}
}
