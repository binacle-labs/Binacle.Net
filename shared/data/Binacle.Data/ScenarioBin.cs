
namespace Binacle.Data;

public sealed class ScenarioBin : IWithID, IWithDimensions
{
	public ScenarioBin()
	{
		this.ID = string.Empty;
	}

	public ScenarioBin(string id, Binacle.Geometry.IWithDimensions<int> dimensions)
	{
		this.ID = id;
		this.Length = dimensions.Length;
		this.Width = dimensions.Width;
		this.Height = dimensions.Height;
	}

	// "LxWxH": a bin carries no quantity. Parsed via the shared Binacle.CompactNotation notation.
	public static ScenarioBin FromCompactString(string compact)
		=> new(compact, Binacle.CompactNotation.CompactNotationParser.ParseDimensions<int>(compact));

	public string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
