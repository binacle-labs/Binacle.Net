using System.Runtime.InteropServices;
using Binacle.Net.Kernel.Instance;
using Binacle.Net.Kernel.Instance.Models;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.Pages;

internal class InstanceModel : AppletPageModel
{
	private readonly IOptions<InstanceOptions> instanceOptions;

	public InstanceModel(
		AppletsService appletsService,
		IOptions<InstanceOptions> instanceOptions,
		IWebHostEnvironment environment
	)
		: base(appletsService, "/Instance")
	{
		this.instanceOptions = instanceOptions;
		this.Environment = environment.EnvironmentName;
	}

	public string Version => Metadata.Version;

	// The page reports what the container runs, so this comes from the container. It used to be typed into
	// the view.
	public string Runtime => RuntimeInformation.FrameworkDescription;

	public string Environment { get; }

	public IReadOnlyList<FeatureSwitch> Switches => FeatureSwitch.All;

	public bool IsOn(FeatureSwitch featureSwitch)
		=> this.instanceOptions.Value.IsFeatureEnabled(featureSwitch.Feature);

	// Whoever switched it on recorded where it answers. The health path is configurable, so this is the only
	// way to link it correctly.
	public string? PathFor(FeatureSwitch featureSwitch)
		=> this.instanceOptions.Value.PathFor(featureSwitch.Feature);

	// Sorted here, once, so the view stays a plain loop. This is the same instance the health check and
	// /_debug read - filled once at startup, so it can go stale against /api/v4/presets. See Program.cs.
	public IReadOnlyList<InstancePreset> Presets
		=> this.instanceOptions.Value.Presets.Presets
			.OrderBy(preset => preset.Name, StringComparer.Ordinal)
			.ToList();
}
