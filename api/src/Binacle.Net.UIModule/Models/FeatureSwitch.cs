namespace Binacle.Net.UIModule.Models;

// The other things this server can expose. InstanceOptions only records what is switched on, so the off half
// has to be listed somewhere - this is that list, and a new switch needs a row here to appear on the page.
//
// The service module is left out on purpose: it is not advertised, and the documentation site has no page for
// it. The demo UI is left out because you are looking at it.
// Label is the short name the home page's band shows.
internal record FeatureSwitch(string Feature, string Name, string Label, string Setting)
{
	public static IReadOnlyList<FeatureSwitch> All { get; } =
	[
		new("SwaggerUI", "Swagger UI", "Swagger", "SWAGGER_UI=True"),
		new("ScalarUI", "Scalar UI", "Scalar", "SCALAR_UI=True"),
		new("HealthChecks", "Health check", "Health", "HealthChecks__Enabled=True"),
		new("DebugEndpoint", "Debug endpoint", "Debug", "DEBUG_ENDPOINT=True"),
	];
}
