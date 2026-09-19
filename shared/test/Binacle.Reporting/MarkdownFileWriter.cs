namespace Binacle.Reporting;

// Writes <outputDirectory>/<Filename>.md: the title, the one-sentence header, then the sections. Overwritten
// every run; the committed copy is what the next run is diffed against.
public class MarkdownFileWriter : IFileWriter
{
	private readonly string outputDirectory;

	public MarkdownFileWriter(string outputDirectory)
	{
		this.outputDirectory = outputDirectory;
	}

	public async Task WriteAsync(ResultFile file, ReportSection[] sections)
	{
		Directory.CreateDirectory(this.outputDirectory);

		var filepath = Path.Combine(this.outputDirectory, $"{file.Filename}.md");
		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}

		await using var writer = new StreamWriter(filepath);

		await writer.WriteLineAsync($"# {file.Title}");
		await writer.WriteLineAsync(string.Empty);

		if (!string.IsNullOrWhiteSpace(file.Description))
		{
			await writer.WriteLineAsync(file.Description);
			await writer.WriteLineAsync(string.Empty);
		}

		foreach (var section in sections)
		{
			await writer.WriteAsync(section.MarkdownPrint());
		}
	}
}
