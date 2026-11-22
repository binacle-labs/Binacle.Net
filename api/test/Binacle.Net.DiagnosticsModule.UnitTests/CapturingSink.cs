using Serilog.Core;
using Serilog.Events;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// The Serilog counterpart of CapturingLogger, for code that logs before the DI logger exists.
internal sealed class CapturingSink : ILogEventSink
{
	private readonly List<LogEvent> events = [];

	public IReadOnlyList<string> Warnings =>
		this.events
			.Where(logEvent => logEvent.Level == LogEventLevel.Warning)
			.Select(logEvent => logEvent.RenderMessage())
			.ToArray();

	public void Emit(LogEvent logEvent)
	{
		this.events.Add(logEvent);
	}
}
