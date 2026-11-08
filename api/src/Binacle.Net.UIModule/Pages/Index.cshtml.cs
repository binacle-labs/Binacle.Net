using Binacle.Net.Kernel.Instance;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.Pages;

internal class IndexModel : PageModel
{
	private readonly AppletsService appletsService;
	private readonly IOptions<InstanceOptions> instanceOptions;

	public IndexModel(AppletsService appletsService, IOptions<InstanceOptions> instanceOptions)
	{
		this.appletsService = appletsService;
		this.instanceOptions = instanceOptions;
	}

	public Applet Packing => this.AppletFor("/Packing");
	public Applet Vipaq => this.AppletFor("/Vipaq");
	public Applet Instance => this.AppletFor("/Instance");

	public string Version => Metadata.Version;

	// The same list This Instance shows, so the two pages cannot disagree.
	public IReadOnlyList<FeatureSwitch> Switches => FeatureSwitch.All;

	public bool IsOn(FeatureSwitch featureSwitch)
		=> this.instanceOptions.Value.IsFeatureEnabled(featureSwitch.Feature);

	public string? PathFor(FeatureSwitch featureSwitch)
		=> this.instanceOptions.Value.PathFor(featureSwitch.Feature);

	public int PresetCount => this.instanceOptions.Value.Presets.Presets.Count;

	// A reference gets a door only when it is on: a door to a page that is not there is a dead end.
	public IReadOnlyList<(string Title, string Path)> ReferenceDoors
		=> FeatureSwitch.All
			.Where(x => x.Feature is "SwaggerUI" or "ScalarUI")
			.Select(x => (Title: x.Label, Path: this.PathFor(x)))
			.Where(x => x.Path is not null)
			.Select(x => (x.Title, x.Path!))
			.ToList();

	private Applet AppletFor(string page)
		=> this.appletsService.Applets.First(x => x.Page == page);
}
