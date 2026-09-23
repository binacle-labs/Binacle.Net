using System.Text.RegularExpressions;

namespace Binacle.ViPaq.EncodedSize;

// One output file holds one group in one layout. The Bischoff suite is split by its seven problem sets, because
// 2,100 packs in one table is more than GitHub will render.
internal static partial class Groups
{
	public const string BischoffFamily = "bischoff-suite";

	public static readonly Group[] All =
	[
		new("thpack1", "Bischoff thpack1"),
		new("thpack2", "Bischoff thpack2"),
		new("thpack3", "Bischoff thpack3"),
		new("thpack4", "Bischoff thpack4"),
		new("thpack5", "Bischoff thpack5"),
		new("thpack6", "Bischoff thpack6"),
		new("thpack7", "Bischoff thpack7"),
		new("custom-problems", "Custom problems"),
		new("demo-samples", "Demo samples")
	];

	// "OrLibrary_thpack1_1" -> "thpack1"; any other family is one group.
	public static string Of(string family, string packName)
	{
		if (family != BischoffFamily)
		{
			return family;
		}

		var match = ThpackPattern().Match(packName);
		if (!match.Success)
		{
			throw new InvalidOperationException(
				$"Bischoff pack '{packName}' names no thpack set, so no file would hold it.");
		}

		return match.Value;
	}

	[GeneratedRegex("thpack[0-9]+")]
	private static partial Regex ThpackPattern();
}

internal sealed record Group(string Slug, string Label);

// One file holds one algorithm, so the column would repeat one value on every row.
internal static class Algorithms
{
	public static readonly string[] All = ["BFD", "FFD", "WFD"];
}
