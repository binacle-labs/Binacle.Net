using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.Lib.ResultSelection;
using Binacle.Net.Services;
using Binacle.Net.Telemetry;

namespace Binacle.Net.ExtensionMethods;

internal static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBinacleServices(
		this IServiceCollection services
		)
	{
		services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
		services.AddSingleton<IBinProcessorFactory, BinProcessorFactory>();
		services.AddSingleton<IAlgorithmProcessorFactory, AlgorithmProcessorFactory>();

		services.AddSingleton<IResultSelector>(sp =>
		{
			return new ResultSelector(
				bestBin: new BestBin_v2(),
				smallestBin: new SmallestBin_v2(),
				bestAlgorithm: new BestAlgorithm_v2()
			);
		});
		services.AddSingleton<PackingMetrics>();
		services.AddSingleton<IBinacleService, BinacleService>();
		services.Decorate<IBinacleService, InstrumentedBinacleService>();
		// Outermost, so the log write is not timed as packing.
		services.Decorate<IBinacleService, PackingLogBinacleService>();

		return services;
	}
	
}
