using Binacle.Net.Kernel.Instance.Models;

namespace Binacle.Net.Kernel.Instance;

// What this running instance reports about itself: which features are switched on and where, and the bin
// presets it loaded. One bag for both, because a self-hoster asks the same question of either - "what is
// this instance running with" - and the instance page answers it in one place.
public class InstanceOptions
{
	private const string PresetsKey = "Presets";

	private readonly Dictionary<string, InstanceValue> values = new(StringComparer.OrdinalIgnoreCase);

	public void AddFeature(string feature, string? path = null)
	{
		this.values[feature] = path is null ? new SwitchedOn() : new PathValue(path);
	}

	public void RemoveFeature(string feature)
	{
		this.values.Remove(feature);
	}

	public bool IsFeatureEnabled(string feature)
		=> this.values.TryGetValue(feature, out var value) && value is FeatureValue;

	public string? PathFor(string feature)
		=> (this.values.GetValueOrDefault(feature) as PathValue)?.Path;

	// Filtered by type, not by a name list, so the presets bag - or anything else that is not a FeatureValue -
	// never counts as a switched-on feature.
	public IReadOnlyCollection<string> EnabledFeatures
		=> this.values
			.Where(entry => entry.Value is FeatureValue)
			.Select(entry => entry.Key)
			.ToArray();

	public void SetPresets(PresetsValue presets)
	{
		this.values[PresetsKey] = presets;
	}

	public PresetsValue Presets
		=> this.values.GetValueOrDefault(PresetsKey) as PresetsValue ?? PresetsValue.Empty;
}
