namespace Binacle.Net.Kernel.Validation;

public static class ValidationMessage
{
	public static string RequiredEnumValues(Type enumType, string propertyName)
	{
		var values = Enum.GetNames(enumType);

		return
			$"'{propertyName}' is required and must be one of the following values: {string.Join(", ", values)}";
	}
}
