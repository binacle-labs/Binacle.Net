using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.OpenApi.Transformers;
using Binacle.Net.Kernel.Serialization;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// What an enum property looks like in the document. A generated client types the property off this, so an enum
// left as an integer schema is a client that sends numbers the API refuses.
[Trait("Behavioral Tests", "Ensures enum properties are described as strings")]
public class EnumStringsSchemaTransformerTests
{
	private enum Algorithm
	{
		FFD,
		WFD
	}

	private class Contract
	{
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Algorithm Plain { get; set; }

		[JsonConverter(typeof(JsonStringNullableEnumConverter))]
		public Algorithm? Nullable { get; set; }

		public Algorithm NoConverter { get; set; }

		public int NotAnEnum { get; set; }
	}

	private static async Task<OpenApiSchema> Transform(string propertyName)
	{
		var schema = new OpenApiSchema { Type = JsonSchemaType.Integer };
		await new EnumStringsSchemaTransformer().TransformAsync(
			schema,
			TransformerContexts.ForProperty<Contract>(propertyName),
			CancellationToken.None
		);
		return schema;
	}

	[Fact]
	public async Task An_Enum_With_The_String_Converter_Is_Typed_As_A_String()
	{
		var schema = await Transform(nameof(Contract.Plain));

		schema.Type.ShouldBe(JsonSchemaType.String);
		schema.Enum.ShouldBeNull();
	}

	// The nullable converter is the one that reports the valid values, so the schema lists them by name.
	[Fact]
	public async Task A_Nullable_Enum_Also_Lists_Its_Values_By_Name()
	{
		var schema = await Transform(nameof(Contract.Nullable));

		schema.Type.ShouldBe(JsonSchemaType.String);
		schema.Enum.ShouldNotBeNull();
		schema.Enum.Select(x => x!.GetValue<string>())
			.ShouldBe([nameof(Algorithm.FFD), nameof(Algorithm.WFD)]);
	}

	[Fact]
	public async Task An_Enum_With_No_Converter_Is_Left_Alone()
	{
		var schema = await Transform(nameof(Contract.NoConverter));

		schema.Type.ShouldBe(JsonSchemaType.Integer);
	}

	[Fact]
	public async Task A_Property_That_Is_Not_An_Enum_Is_Left_Alone()
	{
		var schema = await Transform(nameof(Contract.NotAnEnum));

		schema.Type.ShouldBe(JsonSchemaType.Integer);
	}

	[Fact]
	public async Task A_Schema_With_No_Property_Behind_It_Is_Left_Alone()
	{
		var schema = new OpenApiSchema { Type = JsonSchemaType.Integer };

		await new EnumStringsSchemaTransformer().TransformAsync(
			schema,
			TransformerContexts.ForType<Contract>(),
			CancellationToken.None
		);

		schema.Type.ShouldBe(JsonSchemaType.Integer);
	}
}
