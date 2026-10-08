using Binacle.Net.Kernel.Instance;
using Binacle.Net.Kernel.Instance.Models;

namespace Binacle.Net.Kernel.UnitTests.Instance;

// One bag holds both halves of what an instance reports about itself: switches and the bin presets it
// loaded. The presets bag must never be mistaken for a switched-on feature - that would make the health
// check report "Presets" as a feature and IsFeatureEnabled("Presets") return true for nothing.
[Trait("Behavioral Tests", "Ensures InstanceOptions tells a feature from the presets bag it also carries")]
public class InstanceOptionsTests
{
	[Fact]
	public void A_Feature_Reports_Enabled_Once_Added()
	{
		var options = new InstanceOptions();

		options.AddFeature("SwaggerUI", "/swagger");

		options.IsFeatureEnabled("SwaggerUI").ShouldBeTrue();
	}

	[Fact]
	public void A_Feature_Never_Added_Reports_Disabled()
	{
		var options = new InstanceOptions();

		var enabled = options.IsFeatureEnabled("SwaggerUI");

		enabled.ShouldBeFalse();
	}

	[Fact]
	public void A_Feature_Added_With_No_Path_Answers_On_No_Url()
	{
		var options = new InstanceOptions();

		options.AddFeature("DebugEndpoint");

		options.IsFeatureEnabled("DebugEndpoint").ShouldBeTrue();
		options.PathFor("DebugEndpoint").ShouldBeNull();
	}

	[Fact]
	public void A_Feature_Added_With_A_Path_Reports_It()
	{
		var options = new InstanceOptions();

		options.AddFeature("ScalarUI", "/scalar");

		options.PathFor("ScalarUI").ShouldBe("/scalar");
	}

	[Fact]
	public void A_Removed_Feature_Reports_Disabled()
	{
		var options = new InstanceOptions();
		options.AddFeature("SwaggerUI", "/swagger");

		options.RemoveFeature("SwaggerUI");

		options.IsFeatureEnabled("SwaggerUI").ShouldBeFalse();
	}

	[Fact]
	public void Adding_The_Same_Feature_Twice_Replaces_Rather_Than_Duplicates()
	{
		var options = new InstanceOptions();
		options.AddFeature("HealthChecks", "/_health");

		options.AddFeature("HealthChecks", "/health");

		options.PathFor("HealthChecks").ShouldBe("/health");
		options.EnabledFeatures.Count(name => name == "HealthChecks").ShouldBe(1);
	}

	// The trap this whole slice exists to avoid: a preset bag landing in EnabledFeatures would make the health
	// check report "Presets" as a switched-on feature and IsFeatureEnabled("Presets") mean nothing.
	[Fact]
	public void A_Bag_Holding_Presets_Plus_Two_Switched_On_Features_Reports_Exactly_Two_Enabled_Features()
	{
		var options = new InstanceOptions();
		options.AddFeature("SwaggerUI", "/swagger");
		options.AddFeature("ScalarUI", "/scalar");
		var presets = new PresetsValue([new InstancePreset("Default", [new InstancePresetBin("small", 10, 10, 10)])]);

		options.SetPresets(presets);

		var enabled = options.EnabledFeatures;
		enabled.Count.ShouldBe(2);
		enabled.ShouldContain("SwaggerUI");
		enabled.ShouldContain("ScalarUI");
		enabled.ShouldNotContain("Presets");
	}

	[Fact]
	public void The_Presets_Bag_Never_Reports_As_A_Feature_Named_Presets()
	{
		var options = new InstanceOptions();
		var presets = new PresetsValue([]);

		options.SetPresets(presets);

		options.IsFeatureEnabled("Presets").ShouldBeFalse();
	}

	[Fact]
	public void Presets_Defaults_To_Empty_When_Nothing_Was_Set()
	{
		var options = new InstanceOptions();

		var presets = options.Presets;

		presets.Presets.ShouldBeEmpty();
	}

	[Fact]
	public void Presets_Round_Trips_What_Was_Set()
	{
		var options = new InstanceOptions();
		var bin = new InstancePresetBin("small", 10, 20, 30);
		var preset = new InstancePreset("Default", [bin]);
		var presetsValue = new PresetsValue([preset]);

		options.SetPresets(presetsValue);

		var presets = options.Presets;
		presets.Presets.ShouldBe([preset]);
	}
}
