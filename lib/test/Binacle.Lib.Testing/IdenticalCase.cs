namespace Binacle.Lib.Testing;

// The bin and item of the Identical benchmarks: every bin and every item the same, and every piece fits.
public static class IdenticalCase
{
	// Eight by eight by eight items; 256 fills half of it.
	public const string BinSize = "160x120x80";
	public const string ItemSize = "20x15x10";

	public static ScenarioBin Bin()
		=> ScenarioBin.FromCompactString(BinSize);

	// The pieces are split evenly across the lines.
	public static List<ScenarioItem> Items(int pieces, int lines = 1)
	{
		if (pieces % lines != 0)
			throw new ArgumentException($"{pieces} pieces do not split evenly over {lines} lines.");

		return Enumerable.Range(1, lines)
			.Select(_ => ScenarioItem.FromCompactString($"{ItemSize} [{pieces / lines}]"))
			.ToList();
	}
}
