using System.Reflection;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule;

namespace Binacle.Net.ServiceModule.UnitTests;

// Every nullable enum a client can send has to carry JsonStringNullableEnumConverter. Without it the default
// reader takes an ordinal and refuses an unknown name with a plain JsonException, which the binding path
// answers 400 rather than 422 - so a new field is wrong the day it is added, and only on the error path.
[Trait("Behavioral Tests", "Ensures every request enum is read by the converter that reports it as a validation error")]
public class RequestEnumConverterTests
{
	public static TheoryData<Type, string> NullableEnumProperties()
	{
		var data = new TheoryData<Type, string>();

		// Interfaces are skipped: the contracts declare Algorithm on IWithAlgorithm and the attribute belongs
		// on the class that carries the property.
		var properties = typeof(IModuleMarker).Assembly
			.GetTypes()
			.Where(x => x is { IsClass: true })
			.SelectMany(x => x.GetProperties(
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
			))
			.Where(x => Nullable.GetUnderlyingType(x.PropertyType)?.IsEnum == true);

		foreach (var property in properties)
		{
			data.Add(property.DeclaringType!, property.Name);
		}

		return data;
	}

	[Theory]
	[MemberData(nameof(NullableEnumProperties))]
	public void A_Nullable_Enum_Carries_The_Converter(Type declaringType, string propertyName)
	{
		var property = declaringType.GetProperty(propertyName)!;

		var converter = property.GetCustomAttribute<JsonConverterAttribute>();

		converter.ShouldNotBeNull();
		converter.ConverterType.ShouldBe(typeof(JsonStringNullableEnumConverter));
	}

	// A pass with nothing to check would read the same as a pass, and the set is discovered rather than
	// written down.
	[Fact]
	public void The_Contracts_Declare_At_Least_One()
	{
		NullableEnumProperties().ShouldNotBeEmpty();
	}
}
