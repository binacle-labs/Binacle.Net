using System.Reflection;
using System.Text.Json;
using Binacle.Net.Kernel.Validation;

namespace Binacle.Net.Kernel.Serialization;

// Thrown instead of reading the value as null, because null means "absent" and a request that only requires one
// field of several would be accepted with the field the client actually sent dropped.
public sealed class JsonEnumValueException : JsonException
{
	public JsonEnumValueException(Type enumType, string? value)
		: base($"Could not convert \"{value}\" to {enumType.Name}")
	{
		this.EnumType = enumType;
	}

	public Type EnumType { get; }

	// Keyed and worded the same as a missing value, so the field reports one error either way. Path is filled
	// in by System.Text.Json after the throw and carries wire names; requestType is what turns them back into
	// the ones FluentValidation reports.
	public Dictionary<string, string[]> GetValidationSummary(Type requestType)
	{
		var (path, property) = JsonPropertyPath.Resolve(requestType, this.Path, this.EnumType.Name);

		return new Dictionary<string, string[]>
		{
			{ path, [ValidationMessage.RequiredEnumValues(this.EnumType, property)] }
		};
	}
}

internal static class JsonPropertyPath
{
	// "$.parameters.algorithm" over FitPresetBinRequest gives "Parameters.Algorithm", "Algorithm". A segment
	// that resolves to no property is kept as the client wrote it, so the field is still named.
	public static (string Path, string Property) Resolve(Type requestType, string? jsonPath, string fallback)
	{
		var wirePath = jsonPath?.TrimStart('$', '.');
		if (string.IsNullOrEmpty(wirePath))
		{
			return (fallback, fallback);
		}

		var segments = wirePath.Split('.');
		var resolved = new string[segments.Length];
		var property = fallback;
		var declaringType = requestType;

		for (var i = 0; i < segments.Length; i++)
		{
			var (name, index) = SplitIndex(segments[i]);
			var match = FindProperty(declaringType, name);

			property = match?.Name ?? name;
			resolved[i] = property + index;
			declaringType = match is null ? null : NextType(match.PropertyType, index.Length > 0);
		}

		return (string.Join('.', resolved), property);
	}

	private static (string Name, string Index) SplitIndex(string segment)
	{
		var bracket = segment.IndexOf('[');
		return bracket < 0
			? (segment, string.Empty)
			: (segment[..bracket], segment[bracket..]);
	}

	// Matched on the CLR name rather than the naming policy, which is not in reach here. No request contract
	// renames a property, so the two are the same name in a different case.
	private static PropertyInfo? FindProperty(Type? declaringType, string name)
	{
		return declaringType?
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
	}

	private static Type? NextType(Type propertyType, bool indexed)
	{
		var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
		if (!indexed)
		{
			return type;
		}

		if (type.IsArray)
		{
			return type.GetElementType();
		}

		return type.GetInterfaces()
			.Append(type)
			.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			?.GetGenericArguments()[0];
	}
}
