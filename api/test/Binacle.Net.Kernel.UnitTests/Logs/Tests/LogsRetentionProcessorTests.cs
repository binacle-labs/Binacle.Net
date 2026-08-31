using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Logs.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace Binacle.Net.Kernel.UnitTests.Logs.Tests;

// Deletes old log files on its own loop. It is the only thing in the repo that deletes anything on a timer, so
// what it will not touch matters more than what it will.
public class LogsRetentionProcessorTests : IDisposable
{
	private readonly LogsHost host = new();

	private sealed record Request;

	private static readonly DateTimeOffset Now = new(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

	// Every sweeping test seeds this one, and waits for it to go. That is the only signal there is that the
	// sweep has happened.
	private const string Expired = "packing-2026-08-21.ndjson";

	private static LogsProcessorOptions<Request> Options(int? retentionDays)
		=> new()
		{
			Path = LogsHost.LogsFolder,
			FileNameFormat = "packing-{0}.ndjson",
			DateFormat = "yyyy-MM-dd",
			RetentionDays = retentionDays,
		};

	private LogsRetentionProcessor<Request> Processor(int? retentionDays)
		=> new(
			this.host,
			new FakeTimeProvider(Now),
			Options(retentionDays),
			NullLogger<LogsRetentionProcessor<Request>>.Instance
		);

	private void GivenLogFile(string fileName, int daysOld)
	{
		Directory.CreateDirectory(this.host.LogsPath);
		var path = Path.Combine(this.host.LogsPath, fileName);
		File.WriteAllText(path, "{}");
		File.SetLastWriteTimeUtc(path, Now.UtcDateTime.AddDays(-daysOld));
	}

	// ExecuteAsync does not start on the calling thread, so StartAsync returning says nothing about the first
	// sweep. Waiting for the expired file to go is what does.
	private async Task<LogsRetentionProcessor<Request>> Swept(int retentionDays)
	{
		var processor = this.Processor(retentionDays);
		await processor.StartAsync(CancellationToken.None);

		var deadline = DateTime.UtcNow.AddSeconds(10);
		while (File.Exists(Path.Combine(this.host.LogsPath, Expired)))
		{
			if (DateTime.UtcNow > deadline)
			{
				throw new TimeoutException($"{Expired} was still there ten seconds after the processor started");
			}

			await Task.Delay(20);
		}

		await processor.StopAsync(CancellationToken.None);
		return processor;
	}

	[Fact]
	public async Task A_file_past_the_retention_age_is_deleted()
	{
		this.GivenLogFile(Expired, daysOld: 10);

		await this.Swept(retentionDays: 7);

		this.host.LogFiles().ShouldBeEmpty();
	}

	[Fact]
	public async Task A_file_inside_the_retention_age_is_kept()
	{
		this.GivenLogFile(Expired, daysOld: 10);
		this.GivenLogFile("packing-2026-08-30.ndjson", daysOld: 1);

		await this.Swept(retentionDays: 7);

		this.host.LogFiles().ShouldBe(["packing-2026-08-30.ndjson"]);
	}

	// The file name format becomes the glob, so anything else sharing the folder is not ours to delete.
	[Fact]
	public async Task A_file_that_is_not_ours_is_left_alone_however_old_it_is()
	{
		this.GivenLogFile(Expired, daysOld: 10);
		this.GivenLogFile("something-else.log", daysOld: 2000);

		await this.Swept(retentionDays: 7);

		this.host.LogFiles().ShouldBe(["something-else.log"]);
	}

	// Null is the default and means keep everything. Getting this wrong deletes an archive nobody asked to
	// prune. No waiting here: with retention off the processor returns instead of starting a loop.
	[Fact]
	public async Task No_retention_setting_deletes_nothing()
	{
		this.GivenLogFile(Expired, daysOld: 10);

		var processor = this.Processor(retentionDays: null);
		await processor.StartAsync(CancellationToken.None);
		await processor.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
		await processor.StopAsync(CancellationToken.None);

		this.host.LogFiles().ShouldBe([Expired]);
	}

	[Fact]
	public async Task A_log_directory_that_is_not_there_yet_is_not_an_error()
	{
		Directory.Exists(this.host.LogsPath).ShouldBeFalse();

		var processor = this.Processor(retentionDays: 7);
		await processor.StartAsync(CancellationToken.None);
		// There is nothing to wait for when the sweep finds no directory. The assertion does not depend on the
		// delay - it gives the sweep a chance to run and fail, which is the thing being ruled out.
		await Task.Delay(250, TestContext.Current.CancellationToken);
		await processor.StopAsync(CancellationToken.None);

		processor.ExecuteTask!.Exception.ShouldBeNull();
		this.host.LogFiles().ShouldBeEmpty();
	}

	// Shutdown cancels the wait between sweeps. That is not a fault, and a processor that reported it as one
	// would fail every host shutdown.
	[Fact]
	public async Task Shutdown_while_waiting_for_the_next_sweep_is_not_a_fault()
	{
		this.GivenLogFile(Expired, daysOld: 10);

		var processor = await this.Swept(retentionDays: 7);

		processor.ExecuteTask!.IsCompletedSuccessfully.ShouldBeTrue();
	}

	public void Dispose() => this.host.Dispose();
}
