using Binacle.CompactNotation;

namespace Binacle.ViPaq.Testing.Compact;

// The pack in compact notation: the bin, then every placed item as "LxWxH (X,Y,Z)", joined by ';'. The notation
// defines the bin and the item; the ';' joiner is this harness's, not the notation's, so nothing parses a whole
// pack back today. Text, not base64.
public static class CompactEncoder
{
	private const char Separator = ';';

	public static string Encode(Scenario scenario)
	{
		var parts = new List<string>(scenario.Items.Length + 1)
		{
			CompactNotationFormatter.FormatDimensions<ushort>(scenario.Bin)
		};

		foreach (var item in scenario.Items)
		{
			var dimensions = CompactNotationFormatter.FormatDimensions<ushort>(item);
			var coordinates = CompactNotationFormatter.FormatCoordinates<ushort>(item);
			parts.Add($"{dimensions} {coordinates}");
		}

		return string.Join(Separator, parts);
	}
}
