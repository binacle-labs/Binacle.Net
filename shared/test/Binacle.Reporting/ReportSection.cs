using System.Text;
using ConsoleTables;

namespace Binacle.Reporting;

// One section of a file: a title, an optional line under it, and a table when there is one.
public class ReportSection
{
	public required string Title { get; init; }
	public string? Description { get; init; }
	public IResult? Table { get; init; }

	public string MarkdownPrint()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"## {this.Title}");
		if (!string.IsNullOrEmpty(this.Description))
		{
			sb.AppendLine(this.Description);
		}

		sb.AppendLine(string.Empty);
		if (this.Table is null)
		{
			return sb.ToString();
		}

		var table = ConsoleTable.From(this.Table.ToDataTable())
			.Configure(options => options.EnableCount = false);
		// Cells wrap at 40 characters by default, which breaks a markdown link across two rows.
		table.MaxWidth = int.MaxValue;
		sb.Append(table.ToMarkDownString());
		sb.AppendLine(string.Empty);
		return sb.ToString();
	}
}
