using Binacle.ViPaq.EncodedSize.PreReportChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.ViPaq.EncodedSize.ExtensionMethods;

public static class PreReportCheckExtensions
{
	// Pair with RunPreReportChecks: register on the builder, run from the built provider before the reporters,
	// so a broken premise stops the run rather than skewing a table.
	public static IServiceCollection AddPreReportChecks(this IServiceCollection services)
	{
		services.AddTransient<IPreReportCheck, CuratedPicksCheck>();
		services.AddTransient<IPreReportCheck, GroupCoverageCheck>();
		return services;
	}

	// Runs every registered gate, in registration order. Throws on the first failure.
	public static void RunPreReportChecks(this IServiceProvider serviceProvider)
	{
		foreach (var check in serviceProvider.GetServices<IPreReportCheck>())
		{
			check.Run();
		}
	}
}
