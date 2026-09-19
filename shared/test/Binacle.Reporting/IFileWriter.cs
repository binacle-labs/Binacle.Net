namespace Binacle.Reporting;

// Writes one file. Markdown is the only writer today; the interface leaves room for others.
public interface IFileWriter
{
	Task WriteAsync(ResultFile file, ReportSection[] sections);
}
