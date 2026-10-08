namespace Binacle.Lib.PackingEfficiency.Reporters;

// Which shipped algorithms reached the top fill on a scenario, and the gap to the next fill. A three-way tie
// has no next fill, so its margin is zero.
internal sealed record Wins(string[] Best, decimal Margin)
{
	public static Wins Of(PackedScenario scenario)
	{
		var fills = Algorithms.Families
			.Select(family => (Family: family, Fill: scenario.Fill(family, Algorithms.Shipped)))
			.ToArray();
		var top = fills.Max(x => x.Fill);
		var best = fills.Where(x => x.Fill == top).Select(x => x.Family).ToArray();
		var runnersUp = fills.Where(x => x.Fill < top).Select(x => x.Fill).ToArray();
		var margin = runnersUp.Length == 0 ? 0m : top - runnersUp.Max();
		return new Wins(best, margin);
	}
}
