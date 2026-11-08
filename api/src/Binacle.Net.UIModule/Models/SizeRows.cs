namespace Binacle.Net.UIModule.Models;

// What _SizeRows needs to draw one list of sizes. Rows and Remove are Alpine expressions on the page's model.
internal record SizeRows(string Rows, string Name, IReadOnlyList<string> Fields, string Remove);
