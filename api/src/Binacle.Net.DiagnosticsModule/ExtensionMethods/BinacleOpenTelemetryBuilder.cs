using OpenTelemetry;
using Serilog;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal sealed class BinacleOpenTelemetryBuilder
{
	public BinacleOpenTelemetryBuilder(OpenTelemetryBuilder builder, ILogger logger)
	{
		this.Builder = builder;
		this.Logger = logger;
	}

	public OpenTelemetryBuilder Builder { get; }
	public ILogger Logger { get; }
}
