namespace Binacle.Net.Kernel.Instance.Models;

// Plain data, on purpose - Kernel does not reference Binacle.Packing. BinPresetOptionsExtensions in the entry
// project projects BinPresetOptions into this shape.
public sealed class PresetsValue : InstanceValue
{
	public static PresetsValue Empty { get; } = new([]);

	public IReadOnlyList<InstancePreset> Presets { get; }

	public PresetsValue(IReadOnlyList<InstancePreset> presets)
	{
		this.Presets = presets;
	}
}

public sealed record InstancePreset(string Name, IReadOnlyList<InstancePresetBin> Bins);

public sealed record InstancePresetBin(string Id, int Length, int Width, int Height);
