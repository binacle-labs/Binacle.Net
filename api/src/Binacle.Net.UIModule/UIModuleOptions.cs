namespace Binacle.Net.UIModule;

internal class UIModuleOptions
{
	// Empty means the demo fetches relative, from the API it ships in. That is the only shipped value.
	public string ApiBaseUrl { get; set; } = string.Empty;

	// "light", "dark" or "system". _Layout.cshtml renders it and the browser reads it back off the html
	// element - one value, or the server paints one theme and the switcher changes to another.
	public string DefaultTheme { get; set; } = "system";
}
