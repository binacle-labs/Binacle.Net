using Binacle.Lib;
using Binacle.TestReporting;

namespace Binacle.ViPaq.PackedDataGenerator;

// Packs the Bischoff suite, the custom problems and the demo samples with Binacle.Lib and freezes the placed
// results as committed data files under vipaq/data/packed. Takes no arguments on purpose: a run always
// regenerates every algorithm, so it cannot half-run and leave the data mixed. Output is deterministic, so a
// no-change re-run is byte-identical.
//
// The algorithm rides on the file name as a ".<algo>" suffix (orlib_thpack1.ffd.json), so the sets sit side by
// side without mixing.
internal class Program
{
	// Where to read the problems, the subfolder to write placed results into, and the files.
	private static readonly SourceFamily[] Families =
	[
		new(
			InputDir: ["shared", "data", "custom-problems"],
			DestinationFolder: "custom-problems",
			Files: ["baseline.json", "complex.json", "simple.json"]),
		new(
			InputDir: ["shared", "data", "bischoff-suite"],
			DestinationFolder: "bischoff-suite",
			Files:
			[
				"orlib_thpack1.json", "orlib_thpack2.json", "orlib_thpack3.json", "orlib_thpack4.json",
				"orlib_thpack5.json", "orlib_thpack6.json", "orlib_thpack7.json",
			]),
		new(
			InputDir: ["shared", "data", "demo-samples"],
			DestinationFolder: "demo-samples",
			Files:
			[
				"01-opening-set.json", "02-packs-nowhere.json", "03-three-answers.json", "04-bfd-loses.json",
				"05-one-of-each.json", "06-long-items.json", "07-tall-items.json", "08-cube-bin.json",
				"09-bfd-fits-more.json", "10-six-types.json", "11-seven-types.json", "12-middle-bin-wins.json",
				"13-twenty-four-cubes.json", "14-flat-items.json", "15-same-volume-different-shape.json",
				"16-only-bfd-fully-packs.json", "17-four-bins-bfd-ahead.json", "18-four-bins-bfd-fully-packs.json",
				"19-five-bins.json", "20-wfd-wins.json",
			]),
	];

	// One generator per Algorithm value, so a new algorithm in the enum is generated with no edit here.
	// AlgorithmFactory maps each value to its v2 implementation - the version the API runs - so the suffix
	// names the family, not the version.
	private static readonly IReadOnlyList<PackedDataGenerator> Generators =
		[.. Enum.GetValues<Algorithm>().Select(algorithm => new PackedDataGenerator(algorithm))];

	static async Task Main(string[] args)
	{
		var locator = RepositoryRoot.Bind();

		foreach (var generator in Generators)
		{
			var algorithm = generator.Algorithm;
			var suffix = algorithm.ToString().ToLowerInvariant();
			Console.WriteLine($"[{algorithm}] packing (.{suffix}.json)");

			var totalSamples = 0;
			var totalItems = 0;
			foreach (var family in Families)
			{
				var result = await generator.GenerateAsync(family, locator);
				totalSamples += result.Samples;
				totalItems += result.Items;
			}

			Console.WriteLine($"[{algorithm}] {totalSamples} samples, {totalItems} placed items.");
		}
	}
}
