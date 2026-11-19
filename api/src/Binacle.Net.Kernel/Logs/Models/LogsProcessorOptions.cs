namespace Binacle.Net.Kernel.Logs.Models;

// TChannelRequest is unused in the body: it keys the DI registration, so each log processor gets its own
// options singleton.
public class LogsProcessorOptions<TChannelRequest>
{
	public required string Path { get; init; }
	public required string FileNameFormat { get; init; }
	public required string DateFormat { get; init; }
	// After this many consecutive failures the processor stops itself.
	public int MaxConsecutiveAllowedExceptions { get; init; } = 10;

	// Null means never delete: these logs are an archive, so pruning is opt-in. Set it (7, say) to auto-delete
	// local files older than that many days.
	public int? RetentionDays { get; init; }

	// Each is called once per entry: written to disk, lost because writing it threw, or not written when the
	// processor stopped. A normal stop writes the queue first; only what the drain deadline leaves is discarded.
	public Action? OnWritten { get; init; }
	public Action<Exception>? OnWriteFailed { get; init; }
	public Action? OnDiscarded { get; init; }
}
