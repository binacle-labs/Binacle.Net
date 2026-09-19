namespace Binacle.Reporting;

// The five numbers every summary table carries. StdDev is over the whole set, not a sample.
public readonly record struct Statistics(double Min, double Mean, double Median, double Max, double StdDev)
{
	public static Statistics Of(IEnumerable<double> values)
	{
		var ordered = values.Order().ToArray();
		if (ordered.Length == 0)
		{
			throw new ArgumentException("Statistics need at least one value.", nameof(values));
		}

		var mean = ordered.Average();
		var sumSquaredDiffs = ordered.Sum(value => Math.Pow(value - mean, 2));
		var stdDev = Math.Sqrt(sumSquaredDiffs / ordered.Length);

		return new Statistics(ordered[0], mean, MedianOf(ordered), ordered[^1], stdDev);
	}

	private static double MedianOf(double[] ordered)
	{
		var middle = ordered.Length / 2;
		if (ordered.Length % 2 == 0)
		{
			return (ordered[middle - 1] + ordered[middle]) / 2.0;
		}

		return ordered[middle];
	}
}
