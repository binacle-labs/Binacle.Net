using Binacle.Data;
using Binacle.Lib.Abstractions.Algorithms;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PackingEfficiency;

// Packs every Bischoff suite scenario with every algorithm version, once.
internal sealed class PackingRunner : IRunner
{
	private static readonly (string Family, int Version, TestAlgorithmFactory<IPackingAlgorithm> Create)[] algorithms =
	[
		("FFD", 1, AlgorithmFactories.FFD_v1),
		("FFD", 2, AlgorithmFactories.FFD_v2),
		("WFD", 1, AlgorithmFactories.WFD_v1),
		("WFD", 2, AlgorithmFactories.WFD_v2),
		("BFD", 1, AlgorithmFactories.BFD_v1),
		("BFD", 2, AlgorithmFactories.BFD_v2)
	];

	private readonly PackingBag bag;
	private readonly ILogger<PackingRunner> logger;

	public PackingRunner(PackingBag bag, ILogger<PackingRunner> logger)
	{
		this.bag = bag;
		this.logger = logger;
	}

	public void Run()
	{
		foreach (var collectionKey in BischoffSuite.DataProvider.Keys)
		{
			var set = SetLabel(collectionKey);
			foreach (var scenario in BischoffSuite.DataProvider.ByCollection(collectionKey))
			{
				var fills = new Dictionary<string, decimal>();
				foreach (var (family, version, create) in algorithms)
				{
					var algorithm = create(scenario.Bin, scenario.Items);
					var result = algorithm.Execute(new TestOperationParameters { Operation = AlgorithmOperation.Packing });
					fills[Algorithms.Key(family, version)] = result.PackedBinVolumePercentage;
				}

				var packed = new PackedScenario(
					Set: set,
					Types: scenario.Items.Count,
					Name: scenario.Name,
					Items: scenario.Metrics.ItemsCount,
					Ceiling: scenario.Metrics.Percentage,
					Fills: fills
				);
				this.bag.Scenarios.Add(packed);
			}

			this.logger.LogInformation("Packed {Set}", set);
		}
	}

	// "BischoffSuite/orlib_thpack3" -> "BR3"
	private static string SetLabel(string collectionKey) => $"BR{collectionKey[^1]}";
}
