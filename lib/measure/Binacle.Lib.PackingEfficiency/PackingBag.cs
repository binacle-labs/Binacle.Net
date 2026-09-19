namespace Binacle.Lib.PackingEfficiency;

// Everything the reporters read. The runner fills it once; nothing packs after that.
internal sealed class PackingBag
{
	public List<PackedScenario> Scenarios { get; } = new();
}

// One scenario and its fill per algorithm version, as a percentage of the bin's volume.
// Set is the papers' name for the thpack file (BR1..BR7); Ceiling is the items' volume over the bin's.
internal sealed record PackedScenario(
	string Set,
	int Types,
	string Name,
	int Items,
	decimal Ceiling,
	IReadOnlyDictionary<string, decimal> Fills
)
{
	public decimal Fill(string family, int version) => this.Fills[Algorithms.Key(family, version)];
}

internal static class Algorithms
{
	public static readonly string[] Families = ["FFD", "WFD", "BFD"];
	public const int Shipped = 2;
	public const int Previous = 1;

	public static string Key(string family, int version) => $"{family}_v{version}";
}
