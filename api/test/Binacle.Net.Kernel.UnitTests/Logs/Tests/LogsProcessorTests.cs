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
	// OnConvert runs on the processor's thread, so a test can hold a write in flight.
	private sealed record Request(string Name, bool Throws = false, Action? OnConvert = null)
		: ILogEntryConvertible<Entry>
	{
		public Entry ToLogEntry(DateTimeOffset timestamp)
		{
			this.OnConvert?.Invoke();
			return this.Throws
				? throw new InvalidOperationException("this request cannot become a log entry")
				: new Entry(this.Name, timestamp);
		}
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

	[Fact]
	public async Task Each_entry_calls_written_or_failed_once()
	{
		var written = 0;
		var failed = new List<Exception>();
		var options = new LogsProcessorOptions<Request>
		{
			Path = LogsHost.LogsFolder,
			FileNameFormat = "packing-{0}.ndjson",
			DateFormat = "yyyy-MM-dd",
			OnWritten = () => written++,
			OnWriteFailed = failed.Add,
		};

		await this.Drain(
			options,
			At(Now),
			true,
			new Request("first"),
			new Request("bad", Throws: true),
			new Request("second")
		);

		written.ShouldBe(2);
		failed.ShouldHaveSingleItem().ShouldBeOfType<InvalidOperationException>();
	}

	// The channel is left open, so only the processor can complete it.
	[Fact]
	public async Task A_stopped_processor_completes_the_channel_and_discards_what_is_left()
	{
		var failed = 0;
		var discarded = 0;
		var options = new LogsProcessorOptions<Request>
		{
			Path = LogsHost.LogsFolder,
			FileNameFormat = "packing-{0}.ndjson",
			DateFormat = "yyyy-MM-dd",
			MaxConsecutiveAllowedExceptions = 2,
			OnWriteFailed = _ => failed++,
			OnDiscarded = () => discarded++,
		};
		var channel = Channel.CreateUnbounded<Request>();
		channel.Writer.TryWrite(new Request("bad", Throws: true));
		channel.Writer.TryWrite(new Request("worse", Throws: true));
		channel.Writer.TryWrite(new Request("left"));
		channel.Writer.TryWrite(new Request("also left"));

		var processor = new LogsProcessor<Request, Entry>(
			channel,
			this.host,
			At(Now),
			options,
			NullLogger<LogsProcessor<Request, Entry>>.Instance
		);
		await processor.StartAsync(CancellationToken.None);
		await processor.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(10));

		failed.ShouldBe(2);
		discarded.ShouldBe(2);
		channel.Writer.TryWrite(new Request("too late")).ShouldBeFalse();
		channel.Reader.Completion.IsCompleted.ShouldBeTrue();
	}

	private sealed class Counts
	{
		public int Written;
		public int Failed;
		public int Discarded;
	}

	private static LogsProcessorOptions<Request> Counting(Counts counts) => new()
	{
		Path = LogsHost.LogsFolder,
		FileNameFormat = "packing-{0}.ndjson",
		DateFormat = "yyyy-MM-dd",
		OnWritten = () => Interlocked.Increment(ref counts.Written),
		OnWriteFailed = _ => Interlocked.Increment(ref counts.Failed),
		OnDiscarded = () => Interlocked.Increment(ref counts.Discarded),
	};

	// Starts the processor on an open channel, holds the first request in flight, and queues the rest behind it.
	// Returns once the first request is being written.
	private async Task<(LogsProcessor<Request, Entry> Processor, Channel<Request> Channel, SemaphoreSlim Release)>
		HoldInFlight(LogsProcessorOptions<Request> options, TimeProvider timeProvider, params string[] queued)
	{
		var started = new SemaphoreSlim(0);
		var release = new SemaphoreSlim(0);
		var channel = Channel.CreateUnbounded<Request>();
		var processor = new LogsProcessor<Request, Entry>(
			channel,
			this.host,
			timeProvider,
			options,
			NullLogger<LogsProcessor<Request, Entry>>.Instance
		);
		await processor.StartAsync(CancellationToken.None);

		channel.Writer.TryWrite(new Request("in flight", OnConvert: () =>
		{
			started.Release();
			release.Wait();
		}));
		foreach (var name in queued)
		{
			channel.Writer.TryWrite(new Request(name));
		}

		(await started.WaitAsync(TimeSpan.FromSeconds(10))).ShouldBeTrue();
		return (processor, channel, release);
	}

	[Fact]
	public async Task A_normal_stop_finishes_the_write_in_flight_and_writes_what_is_queued()
	{
		var counts = new Counts();
		var (processor, channel, release) = await this.HoldInFlight(
			Counting(counts), At(Now), "queued", "also queued");

		var stop = processor.StopAsync(CancellationToken.None);
		release.Release();
		await stop.WaitAsync(TimeSpan.FromSeconds(10));

		this.LinesIn("packing-2026-08-31.ndjson")
			.Select(line => JsonSerializer.Deserialize<Entry>(line)!.Name)
			.ShouldBe(["in flight", "queued", "also queued"]);
		counts.Written.ShouldBe(3);
		counts.Failed.ShouldBe(0);
		counts.Discarded.ShouldBe(0);
		channel.Writer.TryWrite(new Request("too late")).ShouldBeFalse();
	}

	// The deadline runs on the processor's TimeProvider, so advancing the fake clock passes it.
	[Fact]
	public async Task What_the_stop_deadline_leaves_is_discarded_not_failed()
	{
		var counts = new Counts();
		var timeProvider = At(Now);
		var (processor, _, release) = await this.HoldInFlight(
			Counting(counts), timeProvider, "queued", "also queued");

		var stop = processor.StopAsync(CancellationToken.None);
		timeProvider.Advance(TimeSpan.FromSeconds(5));
		release.Release();
		await stop.WaitAsync(TimeSpan.FromSeconds(10));

		counts.Written.ShouldBe(0);
		counts.Failed.ShouldBe(0);
		counts.Discarded.ShouldBe(3);
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
