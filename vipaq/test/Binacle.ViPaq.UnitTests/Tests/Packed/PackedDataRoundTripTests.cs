using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Every real pack decodes back to itself: through the public serializer in each of its four modes, and through
// ProtocolEncoder forced to 16-bit widths, which the serializer never picks for these mostly-8-bit packs. This
// is the only place the real packs are round-tripped; the vectors and curated inputs elsewhere are small by design.
//
// The header bytes are checked against the header the serializer must produce, so a wrong width, layout or
// compressed bit fails even when the geometry still round-trips. Compressed bytes are never compared (§6.1).
[Trait("Result Tests", "Ensures results are as expected")]
public class PackedDataRoundTripTests
{
	private static readonly (bool Compress, Layout Layout)[] Modes =
	[
		(false, Layout.RowMajor),
		(false, Layout.Columnar),
		(true, Layout.RowMajor),
		(true, Layout.Columnar),
	];

	private static readonly Layout[] Layouts = [Layout.RowMajor, Layout.Columnar];

	[Theory]
	[MemberData(nameof(PackedScenarioProvider.Names), MemberType = typeof(PackedScenarioProvider))]
	public void Serializer_Round_Trips_In_Every_Mode(string name)
	{
		var scenario = PackedScenarioProvider.Get(name);
		var expected = new BinContents<ushort>(scenario.Bin, scenario.Items);

		foreach (var (compress, layout) in Modes)
		{
			var expectedHeader = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(scenario.Bin, scenario.Items)
				with { Compressed = compress, Layout = layout };

			var data = ViPaqSerializerTestingFixture.Serialize(expected, options =>
			{
				options.Compress = compress;
				options.Layout = layout;
			});
			var actual = ViPaqSerializerTestingFixture.Deserialize<ushort>(data);

			Header.FromBytes(data[0], data[1]).ShouldBe(expectedHeader);
			BinContents.AssertSame(expected, actual);
		}
	}

	[Theory]
	[MemberData(nameof(PackedScenarioProvider.NonEmptyNames), MemberType = typeof(PackedScenarioProvider))]
	public void Forced_Sixteen_Bit_Widths_Round_Trip(string name)
	{
		var scenario = PackedScenarioProvider.Get(name);
		var expected = new BinContents<ushort>(scenario.Bin, scenario.Items);

		foreach (var layout in Layouts)
		{
			var header = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(scenario.Bin, scenario.Items)
				with
				{
					Layout = layout,
					BinDimensionsWidth = Width.Sixteen,
					ItemDimensionsWidth = Width.Sixteen,
					ItemCoordinatesWidth = Width.Sixteen,
				};

			var data = ProtocolTestingFixture.Encode(header, scenario.Bin, scenario.Items);
			var actual = ProtocolTestingFixture.Deserialize<ushort>(data);

			Header.FromBytes(data[0], data[1]).ShouldBe(header);
			BinContents.AssertSame(expected, actual);
		}
	}
}
