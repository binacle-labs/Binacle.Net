using System.Text.Json;
using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Logs.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace Binacle.Net.Kernel.UnitTests.Logs.Tests;

// Drains a channel onto disk, one JSON line per request, on a background thread nobody is waiting for. A request
// that fails to convert must not take the processor down with it, and a processor that keeps failing must stop
// rather than spin.
public class LogsProcessorTests : IDisposable
{
	private readonly LogsHost host = new();

	// The channel is typed, so good and failing requests have to be one type.
	private sealed record Request(string Name, bool Throws = false) : ILogEntryConvertible<Entry>
	{
		public Entry ToLogEntry(DateTimeOffset timestamp)
			=> this.Throws
				? throw new InvalidOperationException("this request cannot become a log entry")
				: new Entry(this.Name, timestamp);
	}

	private sealed record Entry(string Name, DateTimeOffset Timestamp);

	private static readonly DateTimeOffset Now = new(2026, 8, 31, 23, 30, 0, TimeSpan.Zero);

	private static LogsProcessorOptions<Request> Options(
		string path = LogsHost.LogsFolder,
		int maxConsecutiveAllowedExceptions = 10
	) => new()
	{
		Path = path,
		FileNameFormat = "packing-{0}.ndjson",
		DateFormat = "yyyy-MM-dd",
		MaxConsecutiveAllowedExceptions = maxConsecutiveAllowedExceptions,
	};

	// Completing the writer is what ends the drain loop, so a test that enqueues everything up front can await
	// the processor instead of polling for a file.
	private async Task<LogsProcessor<Request, Entry>> Drain(
		LogsProcessorOptions<Request> options,
		FakeTimeProvider timeProvider,
		bool complete = true,
		params Request[] requests
	)
	{
		var channel = Channel.CreateUnbounded<Request>();
		foreach (var request in requests)
		{
			await channel.Writer.WriteAsync(request);
		}

		if (complete)
		{
			channel.Writer.Complete();
		}

		var processor = new LogsProcessor<Request, Entry>(
			channel,
			this.host,
			timeProvider,
			options,
			NullLogger<LogsProcessor<Request, Entry>>.Instance
		);

		await processor.StartAsync(CancellationToken.None);
		await processor.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(10));
		return processor;
	}

	private static FakeTimeProvider At(DateTimeOffset now, TimeSpan? localOffset = null)
	{
		var timeProvider = new FakeTimeProvider(now);
		if (localOffset is not null)
		{
			timeProvider.SetLocalTimeZone(
				TimeZoneInfo.CreateCustomTimeZone("Test", localOffset.Value, "Test", "Test")
			);
		}

		return timeProvider;
	}

	private string[] LinesIn(string fileName)
		=> File.ReadAllLines(Path.Combine(this.host.LogsPath, fileName));

	[Fact]
	public async Task Each_request_becomes_one_json_line()
	{
		await this.Drain(Options(), At(Now), true, new Request("first"), new Request("second"));

		this.host.LogFiles().ShouldBe(["packing-2026-08-31.ndjson"]);

		var lines = this.LinesIn("packing-2026-08-31.ndjson");
		lines.Length.ShouldBe(2);
		lines.Select(line => JsonSerializer.Deserialize<Entry>(line)!.Name).ShouldBe(["first", "second"]);
	}

	[Fact]
	public async Task The_entry_is_stamped_with_the_time_it_was_processed()
	{
		await this.Drain(Options(), At(Now), true, new Request("first"));

		var entry = JsonSerializer.Deserialize<Entry>(this.LinesIn("packing-2026-08-31.ndjson")[0])!;
		entry.Timestamp.ShouldBe(Now);
	}

	// The file rolls on local date and the entry is stamped UTC. With the host two hours ahead, 23:30 UTC is
	// already the next day locally, so the two disagree - which is the point.
	[Fact]
	public async Task The_file_rolls_on_local_date_while_the_entry_stays_utc()
	{
		await this.Drain(Options(), At(Now, TimeSpan.FromHours(2)), true, new Request("first"));

		this.host.LogFiles().ShouldBe(["packing-2026-09-01.ndjson"]);

		var entry = JsonSerializer.Deserialize<Entry>(this.LinesIn("packing-2026-09-01.ndjson")[0])!;
		entry.Timestamp.ShouldBe(Now);
	}

	[Fact]
	public async Task The_log_directory_is_created_if_it_is_not_there()
	{
		Directory.Exists(this.host.LogsPath).ShouldBeFalse();

		await this.Drain(Options(), At(Now), true, new Request("first"));

		Directory.Exists(this.host.LogsPath).ShouldBeTrue();
	}

	// One bad request is not a reason to stop logging every request after it.
	[Fact]
	public async Task A_request_that_cannot_convert_is_skipped_and_the_rest_are_written()
	{
		await this.Drain(
			Options(maxConsecutiveAllowedExceptions: 2),
			At(Now),
			true,
			new Request("before"),
			new Request("bad", Throws: true),
			new Request("after")
		);

		this.LinesIn("packing-2026-08-31.ndjson")
			.Select(line => JsonSerializer.Deserialize<Entry>(line)!.Name)
			.ShouldBe(["before", "after"]);
	}

	// The counter resets on every success, so failures have to be consecutive to reach the limit.
	[Fact]
	public async Task A_success_resets_the_failure_count()
	{
		await this.Drain(
			Options(maxConsecutiveAllowedExceptions: 2),
			At(Now),
			true,
			new Request("bad", Throws: true),
			new Request("good"),
			new Request("bad", Throws: true),
			new Request("also good")
		);

		this.LinesIn("packing-2026-08-31.ndjson")
			.Select(line => JsonSerializer.Deserialize<Entry>(line)!.Name)
			.ShouldBe(["good", "also good"]);
	}

	// The channel is left open here. Only the limit can end the loop, so a processor that did not stop would
	// hang instead of failing - hence the timeout inside Drain.
	[Fact]
	public async Task Enough_consecutive_failures_stop_the_processor()
	{
		await this.Drain(
			Options(maxConsecutiveAllowedExceptions: 2),
			At(Now),
			false,
			new Request("bad", Throws: true),
			new Request("worse", Throws: true),
			new Request("never read", Throws: true)
		);

		this.host.LogFiles().ShouldBeEmpty();
	}

	// A file where the directory should be. Nothing can be written after that, but the process must come up.
	[Fact]
	public async Task A_log_directory_that_cannot_be_created_does_not_bring_the_host_down()
	{
		File.WriteAllText(this.host.LogsPath, string.Empty);

		await this.Drain(
			Options(maxConsecutiveAllowedExceptions: 1),
			At(Now),
			false,
			new Request("first")
		);

		File.Exists(this.host.LogsPath).ShouldBeTrue();
	}

	public void Dispose() => this.host.Dispose();
}
