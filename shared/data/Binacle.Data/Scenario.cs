using Binacle.Data.Helpers;

namespace Binacle.Data;

public record CollectionScenario(
	string CollectionKey,
	Scenario Scenario
);


public class Scenario
{
	public required string Name { get; init; }
	public required ScenarioBin Bin { get; init; }
	public required ScenarioMetrics Metrics { get; init; }
	public required List<ScenarioItem> Items { get; init; }
	
	public required ScenarioResult Result { get; init; }
	public override string ToString() => Name;


	public static Scenario Create(
		string name,
		string bin,
		string[] items,
		string metrics,
		IReadOnlyDictionary<string, string> results)
	{
		var parsedMetrics = ScenarioMetricsHelper.ParseFromCompactString(metrics);
		var parsedResult = ScenarioResultHelper.ParseFromMap(results);
		return new Scenario
		{
			Name = name,
			Bin = ScenarioBin.FromCompactString(bin),
			Metrics = parsedMetrics,
			Items = items.Select(ScenarioItem.FromCompactString).ToList(),
			Result = parsedResult
		};
	}
}
