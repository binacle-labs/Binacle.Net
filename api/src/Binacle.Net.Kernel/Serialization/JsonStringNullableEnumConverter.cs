using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binacle.Net.Kernel.Serialization;

public class JsonStringNullableEnumConverter : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert)
	{
		var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
		if (underlyingType is null)
		{
			return false;
		}
		return underlyingType.IsEnum;
	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		Type converterType = typeof(NullableEnumConverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(converterType)!;
	}
}

internal sealed class NullableEnumConverter<T> : JsonConverter<T>
{
	private readonly Type underlyingType;

	public NullableEnumConverter()
	{
		this.underlyingType = Nullable.GetUnderlyingType(typeof(T))!;
	}

	public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!EnumValueReader.TryRead(ref reader, this.underlyingType, out object? result, out var value))
		{
			throw new JsonEnumValueException(this.underlyingType, value);
		}

		return (T)result!;
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(WriteValue(value));
	}

	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!EnumValueReader.TryRead(ref reader, this.underlyingType, out object? result, out var value))
		{
			throw new JsonEnumValueException(this.underlyingType, value);
		}

		return (T)result!;
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(WriteValue(value));
	}

	private static ReadOnlySpan<char> WriteValue(T value)
	{
		return value?.ToString();
	}
}

internal static class EnumValueReader
{
	public static bool TryRead(
		ref Utf8JsonReader reader,
		Type enumType,
		out object? result,
		out string? value
	)
	{
		result = null;
		value = null;

		if (reader.TokenType == JsonTokenType.Null)
		{
			return true;
		}

		if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.PropertyName)
		{
			return false;
		}

		value = reader.GetString();
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// for performance, parse with ignoreCase:false first.
		return Enum.TryParse(enumType, value, ignoreCase: false, out result)
		       || Enum.TryParse(enumType, value, ignoreCase: true, out result);
	}
}
