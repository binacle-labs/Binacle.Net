using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.Kernel.UnitTests.Serialization.Providers;

namespace Binacle.Net.Kernel.UnitTests.Serialization;

// What a request enum does with a value the enum does not have. Refused rather than read as null, because null
// means "absent" and a request that only requires one field of several would be accepted with the field the
// client actually sent dropped.
[Trait("Behavioral Tests", "Ensures request enum values read and fail as expected")]
public class JsonStringNullableEnumConverterTests
{
	private enum Algorithm
	{
		FFD,
		WFD
	}

	// The shape every request contract uses: the factory, on a nullable enum.
	private class Request
	{
		[JsonConverter(typeof(JsonStringNullableEnumConverter))]
		public Algorithm? Algorithm { get; set; }
	}

	// v3 and v4 nest theirs one level down, which is where the reported property name comes from.
	private class NestedRequest
	{
		public Request Parameters { get; set; } = new();
	}

	private class CollectionRequest
	{
		public List<Request> Parameters { get; set; } = [];
	}

	private static readonly JsonSerializerOptions options = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	[Theory]
	[ClassData(typeof(RejectedEnumValueProvider))]
	public void A_Value_The_Enum_Does_Not_Have_Is_Refused_Rather_Than_Read_As_Absent(string value)
	{
		var exception = Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<Request>($$"""{"algorithm":{{value}}}""", options)
		);

		exception.EnumType.ShouldBe(typeof(Algorithm));
	}

	// The BCL's answer is asserted first: without that contrast the strictness looks like a bug and gets
	// removed. A client that sends 1 would otherwise get an algorithm it never named.
	[Fact]
	public void An_Ordinal_Is_Refused_Rather_Than_Read_As_The_Value_At_That_Index()
	{
		JsonSerializer.Deserialize<Algorithm>("1").ShouldBe(Algorithm.WFD);

		Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<Request>("""{"algorithm":1}""", options)
		);
	}

	[Theory]
	[InlineData("""{"algorithm":null}""")]
	[InlineData("""{"algorithm":""}""")]
	[InlineData("{}")]
	public void An_Absent_Value_Reads_As_Null(string json)
	{
		var request = JsonSerializer.Deserialize<Request>(json, options);

		request.ShouldNotBeNull();
		request.Algorithm.ShouldBeNull();
	}

	[Theory]
	[InlineData("FFD")]
	[InlineData("ffd")]
	[InlineData("fFd")]
	[InlineData("  FFD  ")] // Enum.TryParse trims, so padding is a typo rather than a different value
	public void A_Known_Value_Reads_Regardless_Of_Case_And_Padding(string value)
	{
		var request = JsonSerializer.Deserialize<Request>($$"""{"algorithm":"{{value}}"}""", options);

		request.ShouldNotBeNull();
		request.Algorithm.ShouldBe(Algorithm.FFD);
	}

	// An enum used as a dictionary key goes through ReadAsPropertyName / WriteAsPropertyName instead. The
	// attribute cannot reach them - it applies to the dictionary, not its key - and a Dictionary keyed on a
	// nullable enum is warned about, so the pair is driven here directly.
	private static readonly JsonConverter<Algorithm?> keyConverter =
		(JsonConverter<Algorithm?>)new JsonStringNullableEnumConverter()
			.CreateConverter(typeof(Algorithm?), options);

	[Fact]
	public void A_Key_Reads_As_Its_Name()
	{
		var reader = new Utf8JsonReader("\"WFD\""u8);
		reader.Read();

		keyConverter.ReadAsPropertyName(ref reader, typeof(Algorithm?), options).ShouldBe(Algorithm.WFD);
	}

	[Fact]
	public void A_Key_The_Enum_Does_Not_Have_Is_Refused_The_Same_Way_As_A_Value()
	{
		var exception = Should.Throw<JsonEnumValueException>(() =>
		{
			var reader = new Utf8JsonReader("\"invalid\""u8);
			reader.Read();
			keyConverter.ReadAsPropertyName(ref reader, typeof(Algorithm?), options);
		});

		exception.EnumType.ShouldBe(typeof(Algorithm));
	}

	// Written as a name, not as a value: a value here leaves the object with no key and the writer refuses it.
	[Fact]
	public void A_Key_Writes_As_A_Property_Name()
	{
		using var stream = new MemoryStream();
		using (var writer = new Utf8JsonWriter(stream))
		{
			writer.WriteStartObject();
			keyConverter.WriteAsPropertyName(writer, Algorithm.WFD, options);
			writer.WriteStringValue("x");
			writer.WriteEndObject();
		}

		Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("""{"WFD":"x"}""");
	}

	[Fact]
	public void A_Value_Writes_As_Its_Name_Rather_Than_Its_Ordinal()
	{
		var json = JsonSerializer.Serialize(new Request { Algorithm = Algorithm.WFD }, options);

		json.ShouldBe("""{"algorithm":"WFD"}""");
	}

	// The key has to be the one FluentValidation reports for the same field, or a client gets two different
	// keys for it depending on whether the value was absent or unknown.
	[Fact]
	public void The_Summary_Is_Keyed_On_The_Property_Path_Not_The_Wire_Path()
	{
		var exception = Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<NestedRequest>(
				"""{"parameters":{"algorithm":"invalid"}}""",
				options
			)
		);

		exception.GetValidationSummary(typeof(NestedRequest))
			.ShouldContainKey("Parameters.Algorithm");
	}

	// The message names the field, not the path it sits on.
	[Fact]
	public void The_Summary_Names_The_Property_And_Lists_Every_Value_The_Enum_Has()
	{
		var exception = Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<NestedRequest>(
				"""{"parameters":{"algorithm":"invalid"}}""",
				options
			)
		);

		exception.GetValidationSummary(typeof(NestedRequest))["Parameters.Algorithm"]
			.ShouldHaveSingleItem()
			.ShouldBe("'Algorithm' is required and must be one of the following values: FFD, WFD");
	}

	// Nothing carries an enum inside a collection yet, so the index handling is only covered here.
	[Fact]
	public void The_Summary_Keeps_The_Index_Of_A_Collection_Element()
	{
		var exception = Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<CollectionRequest>(
				"""{"parameters":[{"algorithm":"invalid"}]}""",
				options
			)
		);

		exception.GetValidationSummary(typeof(CollectionRequest))
			.ShouldContainKey("Parameters[0].Algorithm");
	}

	// A field the request type does not declare still has to name something the client can find.
	[Fact]
	public void An_Unresolvable_Path_Falls_Back_To_The_Name_The_Client_Wrote()
	{
		var exception = Should.Throw<JsonEnumValueException>(
			() => JsonSerializer.Deserialize<Request>("""{"algorithm":"invalid"}""", options)
		);

		exception.GetValidationSummary(typeof(NestedRequest))
			.ShouldContainKey("algorithm");
	}
}
