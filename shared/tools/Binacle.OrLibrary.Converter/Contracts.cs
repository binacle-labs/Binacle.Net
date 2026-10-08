namespace Binacle.OrLibrary.Converter;

// The compact scenario, in the exact shape and property order Binacle.Data reads. One array per thpack file.
//
//   Name    - "OrLibrary_thpack{file}_{problem index}", e.g. "OrLibrary_thpack1_1".
//   Bin      - the container as "LxWxH".
//   Metrics  - "ItemsVolume BinVolume ItemsCount Percentage": totals over all box types, and their volume ratio.
//   Result   - the expected outcome the tests assert against, keyed by algorithm: "{PackingStatus} {FittingStatus}".
//   Items    - the box types as "LxWxH [Quantity]" (types with a count, not placed items, no coordinates).
internal sealed class Scenario
{
	public required string Name { get; init; }
	public required string Bin { get; init; }
	public required string Metrics { get; init; }
	public required IReadOnlyDictionary<string, string> Result { get; init; }
	public required string[] Items { get; init; }
}
