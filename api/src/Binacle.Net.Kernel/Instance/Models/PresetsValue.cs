namespace Binacle.Net.Kernel.Instance.Models;

// Plain data, on purpose - Kernel references neither Binacle.Packing nor Binacle.Geometry, and a bin type
// here would be the single largest cost in this slice. BinPresetOptionsExtensions in the entry project projects
// BinPresetOptions into this shape; nothing in Kernel knows BinOption exists.
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
