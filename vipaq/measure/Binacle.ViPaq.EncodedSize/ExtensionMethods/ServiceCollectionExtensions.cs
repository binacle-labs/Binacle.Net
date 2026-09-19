using Binacle.Reporting;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Data.Packed;
using Binacle.ViPaq.EncodedSize.Tests;
using Binacle.ViPaq.Testing.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.EncodedSize.ExtensionMethods;

public static class ServiceCollectionExtensions
{
	// NoOp is the raw-size baseline; deflate and gzip are the two candidates being raced.
	private static (string Name, ICompressionCodec Codec, string Blurb)[] Codecs =
	[
		("NoOp", new NoOpCodec(), "Uncompressed baseline. The codec passes the body through, so these are raw sizes."),
		("Deflate", new DeflateCodec(), "Bodies compressed with raw DEFLATE, both sides."),
		("Gzip", new GzipCodec(), "Bodies compressed with gzip, both sides."),
	];
	
	// Real sets only. Synthetic random data is never size-measured - gzip can't grip it.
	private static (string Label, IReadOnlyCollection<Scenario> Scenarios)[] ScenarioSets =
	[
		("custom packs", CustomProblems.All),
		("Bischoff suite", BischoffSuite.All),
	];

	private static EncoderInfo[] Layouts = [EncoderInfo.RowMajor, EncoderInfo.Columnar];

	// One file per codec. Protobuf runs the same codec as ViPaq, so every table compares format, not compression.
	public static IServiceCollection AddVipaqProtobufSizeComparisonTests(this IServiceCollection services)
	{
		foreach (var (codecName, codec, blurb) in Codecs)
		{
			var sizeFile = new ResultFile
			{
				Filename = $"VipaqProtobufSizeComparison.{codecName}",
				Title = $"ViPaq vs Protobuf - stored size ({codecName})",
				Description = blurb + " Real placed data (offline FFD, WFD and BFD), each set shown per layout."
			};

			foreach (var (setLabel, scenarios) in ScenarioSets)
			{
				foreach (var layout in Layouts)
				{
					services.AddTransient<IReporter, VipaqProtobufSizeComparisonTest>(serviceProvider =>
						new VipaqProtobufSizeComparisonTest(
							scenarios,
							codec,
							layout,
							$"Real scenarios - {setLabel} - {layout.LayoutName}",
							$"Real placed data, {codecName} codec, {layout.LayoutName} layout.",
							sizeFile,
							serviceProvider.GetRequiredService<ILogger<VipaqProtobufSizeComparisonTest>>()
						)
					);
				}
			}
		}

		return services;
	}

	// One file per layout, ViPaq only, all three codecs side by side. Layout is held still so the raw size reads
	// against deflate and gzip per scenario.
	public static IServiceCollection AddCodecCompressionCrossoverTests(this IServiceCollection services)
	{
		foreach (var layout in Layouts)
		{
			var crossoverFile = new ResultFile
			{
				Filename = $"CodecCompressionCrossover.{layout.LayoutName}",
				Title = $"ViPaq codec compression crossover - {layout.LayoutName}",
				Description = "ViPaq stored size under each codec: raw (NoOp) next to deflate and gzip, so you can "
					+ "see where compressing a pack starts to pay and which codec wins. Ordered by item count."
			};

			foreach (var (setLabel, scenarios) in ScenarioSets)
			{
				services.AddTransient<IReporter, CodecCompressionCrossoverTest>(serviceProvider =>
					new CodecCompressionCrossoverTest(
						scenarios,
						layout,
						$"Real scenarios - {setLabel} - {layout.LayoutName}",
						$"Real placed data, {layout.LayoutName} layout, all three codecs.",
						crossoverFile,
						serviceProvider.GetRequiredService<ILogger<CodecCompressionCrossoverTest>>()
					)
				);
			}
		}

		return services;
	}
}
