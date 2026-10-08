using System.Globalization;

namespace Binacle.Reporting;

// Numbers in a written file use the invariant culture, so a run on any machine writes the same bytes.
public static class Format
{
	public static string Fixed(double value, int decimals = 2)
		=> value.ToString($"F{decimals}", CultureInfo.InvariantCulture);

	public static string Fixed(decimal value, int decimals = 2)
		=> value.ToString($"F{decimals}", CultureInfo.InvariantCulture);
}
