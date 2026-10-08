
namespace Binacle.Data;

public sealed class ScenarioItem : IWithID, IWithDimensions, IWithQuantity
{
	public ScenarioItem()
	{
		this.ID = string.Empty;
	}

	public ScenarioItem(string id, Binacle.Geometry.IWithDimensions<int> dimensions, int quantity)
	{
		this.ID = id;
		this.Length = dimensions.Length;
		this.Width = dimensions.Width;
		this.Height = dimensions.Height;
		this.Quantity = quantity;
	}

	// "LxWxH" or "LxWxH [Q]" (quantity default 1), parsed via the shared Binacle.CompactNotation notation.
	public static ScenarioItem FromCompactString(string compact)
	{
		var parsed = Binacle.CompactNotation.CompactNotationParser.ParseDimensionsAndQuantity<int>(compact);
		return new ScenarioItem(compact, parsed, parsed.Quantity);
	}

	public string ID { get; set; }
	public int Quantity { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
