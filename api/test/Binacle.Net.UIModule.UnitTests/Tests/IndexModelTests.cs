using Binacle.Net.Kernel.Instance;
using Binacle.Net.Kernel.Instance.Models;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Pages;
using Binacle.Net.UIModule.Services;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.UnitTests;

// The home page's band reports the instance from the server. It reads the same switch list as This Instance,
// so the two pages cannot disagree.
[Trait("Behavioral Tests", "Ensures the home page reports the instance without a browser call")]
public class IndexModelTests
{
	private static IndexModel IndexWith(params (string Feature, string? Path)[] enabled)
	{
		var instanceOptions = new InstanceOptions();
		foreach (var (feature, path) in enabled)
		{
			instanceOptions.AddFeature(feature, path);
		}

		return new IndexModel(new AppletsService(), Options.Create(instanceOptions));
	}

	[Fact]
	public void The_Band_Lists_The_Same_Switches_As_This_Instance()
	{
		var page = IndexWith();

		page.Switches.ShouldBe(FeatureSwitch.All);
	}

	[Fact]
	public void An_On_Switch_Links_To_Where_It_Answers()
	{
		var page = IndexWith(("HealthChecks", "/healthz"));

		var health = page.Switches.Single(x => x.Feature == "HealthChecks");

		page.IsOn(health).ShouldBeTrue();
		page.PathFor(health).ShouldBe("/healthz");
	}

	[Fact]
	public void The_Preset_Count_Is_What_The_Instance_Loaded()
	{
		var instanceOptions = new InstanceOptions();
		instanceOptions.SetPresets(new PresetsValue([
			new InstancePreset("a", [new InstancePresetBin("S", 1, 1, 1)]),
			new InstancePreset("b", [new InstancePresetBin("S", 1, 1, 1)]),
		]));
		var page = new IndexModel(new AppletsService(), Options.Create(instanceOptions));

		page.PresetCount.ShouldBe(2);
	}

	[Fact]
	public void No_Reference_Door_On_A_Bare_Instance()
	{
		var page = IndexWith();

		page.ReferenceDoors.ShouldBeEmpty();
	}

	[Fact]
	public void A_Reference_Door_Shows_Only_When_It_Is_On()
	{
		var page = IndexWith(("SwaggerUI", "/swagger"), ("HealthChecks", "/healthz"));

		page.ReferenceDoors.ShouldBe([("Swagger", "/swagger")]);
	}
}
