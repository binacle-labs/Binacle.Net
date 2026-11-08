using System.Text.RegularExpressions;
using Binacle.Net.Kernel.Instance.Models;
using Binacle.Net.UIModule.Services;

namespace Binacle.Net.UIModule.UnitTests;

// The instance page draws whatever presets the instance loaded, so the drawing has to hold for any box size.
[Trait("Behavioral Tests", "Ensures preset boxes are drawn on one scale, with tape and a label only where they suit")]
public class CartonDrawingTests
{
	private static IReadOnlyList<string> Draw(params (int L, int W, int H)[] boxes)
		=> CartonDrawing.ForBins(boxes.Select((b, i) => new InstancePresetBin($"B{i}", b.L, b.W, b.H)).ToList());

	private static string ViewBox(string svg)
		=> Regex.Match(svg, "viewBox=\"([^\"]+)\"").Groups[1].Value;

	[Fact]
	public void Every_Box_Gets_One_Drawing()
	{
		var drawings = Draw((10, 10, 10), (20, 20, 20), (30, 30, 30));

		drawings.Count.ShouldBe(3);
	}

	[Fact]
	public void The_Boxes_Of_One_Preset_Share_A_Scale()
	{
		var drawings = Draw((10, 10, 10), (60, 40, 30));

		ViewBox(drawings[0]).ShouldBe(ViewBox(drawings[1]));
	}

	// Pinned, so a change to the picture is a choice and not an accident.
	[Fact]
	public void The_Smallest_Default_Cube_Draws_As_Designed()
	{
		var drawings = Draw((10, 10, 10), (20, 20, 20), (30, 30, 30));

		ViewBox(drawings[0]).ShouldBe("-3 -41 59.4 51.1");
		drawings[0].ShouldContain("<path class=\"drawing__bin-shape drawing__bin-shape--front\" d=\"M0 0 L8.7 2 L8.7 -7.1 L0 -9.1 Z\"/>");
	}

	[Fact]
	public void A_Carton_Is_Taped()
	{
		var drawing = Draw((60, 40, 20))[0];

		drawing.ShouldContain("drawing__bin-shape--tape");
	}

	[Fact]
	public void A_Pallet_Keeps_The_Seam_Alone()
	{
		var drawing = Draw((120, 80, 15))[0];

		drawing.ShouldNotContain("drawing__bin-shape--tape");
		drawing.ShouldNotContain("drawing__detail");
	}

	[Fact]
	public void An_Envelope_Is_Not_Taped()
	{
		var drawing = Draw((32, 25, 1))[0];

		drawing.ShouldNotContain("drawing__bin-shape--tape");
	}

	[Fact]
	public void A_Label_Goes_Only_On_A_Front_Of_Good_Proportion()
	{
		var drawings = Draw((60, 40, 30), (60, 40, 10), (20, 20, 40));

		drawings[0].ShouldContain("drawing__detail");
		drawings[1].ShouldNotContain("drawing__detail");
		drawings[2].ShouldNotContain("drawing__detail");
	}
}
