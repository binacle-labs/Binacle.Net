using System.Globalization;
using System.Text;
using Binacle.Net.Kernel.Instance.Models;

namespace Binacle.Net.UIModule.Services;

// Draws a preset's boxes as cartons packed shut, seen from the corner. The instance loads its own presets, so
// they are drawn on the server when the page renders. All of one preset's boxes share a viewBox, so they share
// a scale and a floor.
internal static class CartonDrawing
{
	private const double Margin = 3;

	public static IReadOnlyList<string> ForBins(IReadOnlyList<InstancePresetBin> bins)
	{
		var view = new View();
		var layers = bins.Select(bin => Carton(view, bin.Length, bin.Width, bin.Height)).ToList();
		var viewBox = view.ViewBox(Margin);
		return layers
			.Select(layer => $"<svg class=\"drawing__art drawing__art--cartons\" viewBox=\"{viewBox}\" aria-hidden=\"true\">{layer}</svg>")
			.ToList();
	}

	private static string Carton(View v, double l, double d, double h)
	{
		var shadow = 0.3 * h + 3;
		var front = Poly(v.P(0, 0, 0), v.P(l, 0, 0), v.P(l, 0, h), v.P(0, 0, h));
		var side = Poly(v.P(l, 0, 0), v.P(l, d, 0), v.P(l, d, h), v.P(l, 0, h));
		var top = Poly(v.P(0, 0, h), v.P(l, 0, h), v.P(l, d, h), v.P(0, d, h));
		var outline = Poly(v.P(0, 0, 0), v.P(l, 0, 0), v.P(l, d, 0), v.P(l, d, h), v.P(0, d, h), v.P(0, 0, h));

		var t = Math.Min(3.2, d / 6);
		var drop = Math.Min(7, h * 0.35);
		var tapeTop = Poly(v.P(0, d / 2 - t, h), v.P(l, d / 2 - t, h), v.P(l, d / 2 + t, h), v.P(0, d / 2 + t, h));
		var tapeSide = Poly(v.P(l, d / 2 - t, h), v.P(l, d / 2 + t, h), v.P(l, d / 2 + t, h - drop), v.P(l, d / 2 - t, h - drop));

		var svg = new StringBuilder();
		svg.Append(Shape("shadow", Poly(v.P(l, 0, 0), v.P(l + shadow, 4, 0), v.P(l + shadow, d + 4, 0), v.P(l, d, 0))));
		svg.Append(Shape("bin--front", front)).Append(Shape("bin--side", side)).Append(Shape("bin--top", top));
		svg.Append(Shape("bin", Line(v.P(0, d / 2, h), v.P(l, d / 2, h))));

		// Pallets, containers and envelopes are not taped: they keep the seam alone.
		var taped = Math.Max(l, Math.Max(d, h)) <= 100 && h >= 3;
		if (taped)
		{
			svg.Append(Shape("bin--tape", tapeTop)).Append(Shape("bin--tape", tapeSide));
		}

		// A label only on a front that is neither flat nor tall, and big enough to hold one.
		var k = l * 0.3 / 20;
		var lx = l * 0.1;
		var lz = h * 0.82;
		if (taped && h / l is >= 0.35 and <= 0.9 && lz - 9 * k >= 2 * k)
		{
			svg.Append("<g class=\"drawing__detail\">");
			svg.Append(Shape("bin--detail", Poly(v.P(lx, 0, lz - 9 * k), v.P(lx + 20 * k, 0, lz - 9 * k), v.P(lx + 20 * k, 0, lz), v.P(lx, 0, lz))));
			for (var i = 0; i < 3; i++)
			{
				var z = lz - (3 + i * 2.4) * k;
				svg.Append(Shape("bin--detail", Line(v.P(lx + 3 * k, 0, z), v.P(lx + (17 - i * 4) * k, 0, z))));
			}
			svg.Append("</g>");
		}

		svg.Append(Shape("bin--outline", outline));
		return svg.ToString();
	}

	private static string Shape(string name, string d)
	{
		var parts = name.Split("--");
		var cls = $"drawing__{parts[0]}-shape";
		return parts.Length > 1
			? $"<path class=\"{cls} {cls}--{parts[1]}\" d=\"{d}\"/>"
			: $"<path class=\"{cls}\" d=\"{d}\"/>";
	}

	private static string Poly(params (double X, double Y)[] points)
		=> "M" + string.Join(" L", points.Select(p => $"{N(p.X)} {N(p.Y)}")) + " Z";

	private static string Line((double X, double Y) a, (double X, double Y) b)
		=> $"M{N(a.X)} {N(a.Y)} L{N(b.X)} {N(b.Y)}";

	private static string N(double value)
	{
		var s = value.ToString("F1", CultureInfo.InvariantCulture);
		if (s is "-0.0" or "0.0")
		{
			return "0";
		}
		return s.EndsWith(".0") ? s[..^2] : s;
	}

	// From the corner: turned 30 degrees, looked down on at 24. x along the front, y into the box, z up.
	private sealed class View
	{
		private static readonly double Turn = 30 * Math.PI / 180;
		private static readonly double Tilt = 24 * Math.PI / 180;

		private double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

		public (double X, double Y) P(double x, double y, double z)
		{
			var across = x * Math.Cos(Turn) + y * Math.Sin(Turn);
			var depth = -x * Math.Sin(Turn) + y * Math.Cos(Turn);
			var point = (X: across, Y: -(z * Math.Cos(Tilt) + depth * Math.Sin(Tilt)));
			this.minX = Math.Min(this.minX, point.X);
			this.minY = Math.Min(this.minY, point.Y);
			this.maxX = Math.Max(this.maxX, point.X);
			this.maxY = Math.Max(this.maxY, point.Y);
			return point;
		}

		public string ViewBox(double margin)
		{
			var x0 = this.minX - margin;
			var y0 = this.minY - margin;
			return $"{N(x0)} {N(y0)} {N(this.maxX + margin - x0)} {N(this.maxY + margin - y0)}";
		}
	}
}
