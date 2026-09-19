namespace Binacle.Lib.PackingEfficiency;

// The files under lib/results/. Every one opens with the same sentence: tool, count, data set - never a date.
internal static class ResultFiles
{
	private const string Header =
		"Written by `just measure lib` from `Binacle.Lib.PackingEfficiency` over the 700 Bischoff suite scenarios (thpack1..7). Do not edit.";

	public static readonly ResultFile Readme = new()
	{
		Filename = "README",
		Title = "Packing efficiency results",
		Description = Header
	};

	public static readonly ResultFile PackingEfficiency = new()
	{
		Filename = "packing-efficiency",
		Title = "Packing efficiency",
		Description = Header
	};

	public static readonly ResultFile VersionParity = new()
	{
		Filename = "version-parity",
		Title = "Version parity",
		Description = Header
	};
}
