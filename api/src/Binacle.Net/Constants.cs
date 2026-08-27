using Binacle.Net.Kernel.Validation;

namespace Binacle.Net;


internal static class ErrorMessage
{
	public static string RequiredEnumValues<TEnum>(string propertyName)
		where TEnum : struct, Enum
	{
		return ValidationMessage.RequiredEnumValues(typeof(TEnum), propertyName);
	}
}

