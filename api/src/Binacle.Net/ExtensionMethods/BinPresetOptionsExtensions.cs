using Binacle.Net;
using Binacle.Net.Kernel.Instance.Models;

namespace Binacle.Net.ExtensionMethods;

internal static class BinPresetOptionsExtensions
{
	// The one place BinPresetOptions (which carries BinOption, a Bin type) meets PresetsValue (which does not).
	// Kernel has no reference to Binacle.Packing or Binacle.Geometry, so this projection cannot live there.
	// Extends the nullable type on purpose: the configuration section is optional and reads back null.
	internal static PresetsValue ToInstancePresets(this BinPresetOptions? binPresetOptions)
	{
		if (binPresetOptions is null || binPresetOptions.Presets.Count == 0)
		{
			return PresetsValue.Empty;
		}

		var presets = new List<InstancePreset>();

		foreach (var (name, preset) in binPresetOptions.Presets)
		{
			var bins = new List<InstancePresetBin>();

			foreach (var bin in preset.Bins)
			{
				var instanceBin = new InstancePresetBin(bin.ID, bin.Length, bin.Width, bin.Height);
				bins.Add(instanceBin);
			}

			var instancePreset = new InstancePreset(name, bins);
			presets.Add(instancePreset);
		}

		return new PresetsValue(presets);
	}
}
