namespace Binacle.Reporting;

// Names one output file. Title is the `#` line; Description is the one sentence under it.
public class ResultFile
{
	public required string Filename { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
}
