namespace Binacle.ViPaq.Testing.ViPaq;

// The header the harness hands `ProtocolEncoder` for a scenario in a forced mode, and the widths a report
// prints from it.
//
// The library's own `Header` works out the widths; this only prints them. `Header`,
// `Width` and `Layout` are internal, so they cannot appear on a public member here - which is why the library
// grants `InternalsVisibleTo` to this project.
public readonly record struct ViPaqHeader
{
	internal readonly Header Header;

	private ViPaqHeader(Header header)
	{
		this.Header = header;
	}

	// The header for a scenario in a forced mode, before any token exists. Widths come from `Header.Create`,
	// then the mode is stamped on. The race always compresses - NoOp is how it prices the raw size - so
	// `Compressed` is always set.
	public static ViPaqHeader Create(Scenario scenario, EncoderInfo encoderInfo)
	{
		var header = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(
			scenario.Bin,
			scenario.Items
		);

		var modifiedHeader = header
			with
			{
				Compressed = true,
				Layout = encoderInfo.Layout
			};

		return new ViPaqHeader(modifiedHeader);
	}

	public int BinDimensionsBits 
		=> Bits(this.Header.BinDimensionsWidth);

	public int ItemDimensionsBits 
		=> Bits(this.Header.ItemDimensionsWidth);

	public int ItemCoordinatesBits 
		=> Bits(this.Header.ItemCoordinatesWidth);

	// How a report names the three widths, in wire order.
	public string ToWidthsLabel()
		=> $"{this.BinDimensionsBits}/{this.ItemDimensionsBits}/{this.ItemCoordinatesBits}";

	private static int Bits(Width width) 
		=> Helpers.WidthHelper.ByteCount(width) * 8;
}
