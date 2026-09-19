using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binacle.ViPaq.Testing.Json;

// The JSON a user's token replaces: the bin and the placed items, nothing else. Same field names as the API,
// camelCase, no whitespace. No IDs, algorithm or percentages - the token does not carry those, so counting
// them would overstate what it saves. Text, not base64: JSON is already its stored form.
public static class JsonEncoder
{
	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = false
	};

	public static string Encode(Scenario scenario)
	{
		var pack = new Pack(
			new Box(scenario.Bin.Length, scenario.Bin.Width, scenario.Bin.Height),
			scenario.Items.Select(item => new PlacedBox(item.Length, item.Width, item.Height, item.X, item.Y, item.Z)).ToArray()
		);
		return JsonSerializer.Serialize(pack, Options);
	}

	private sealed record Pack(Box Bin, PlacedBox[] Items);

	private sealed record Box(ushort Length, ushort Width, ushort Height);

	private sealed record PlacedBox(ushort Length, ushort Width, ushort Height, ushort X, ushort Y, ushort Z);
}
