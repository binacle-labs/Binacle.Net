using System.Text.Json;
using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Kernel.Logs.Services;

// Generic log processor: drains a channel of requests and appends one JSON line per request to a dated file.
// Each request maps itself to its log entry (ILogEntryConvertible) in the background. The request thread only
// enqueued raw references. Knows nothing about any specific feature's request or entry types.
internal class LogsProcessor<TRequest, TLog> : BackgroundService
	where TRequest : ILogEntryConvertible<TLog>
{
	// Inside docker stop's default 10 seconds.
	private static readonly TimeSpan DrainDeadline = TimeSpan.FromSeconds(5);

	private readonly Channel<TRequest> channel;
	private readonly IHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly LogsProcessorOptions<TRequest> options;
	private readonly ILogger<LogsProcessor<TRequest, TLog>> logger;
	private int exceptionsCount;

	public LogsProcessor(
		Channel<TRequest> channel,
		IHostEnvironment environment,
		TimeProvider timeProvider,
		LogsProcessorOptions<TRequest> options,
		ILogger<LogsProcessor<TRequest, TLog>> logger)
	{
		this.channel = channel;
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.options = options;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var logDirectory = Path.Combine(this.environment.ContentRootPath, this.options.Path);
		this.EnsureDirectoryExists(logDirectory);
		this.logger.LogInformation("{Status} logs processor for {LogProcessorRequest}", "Starting",
			typeof(TRequest).Name);

		// Writes take the deadline, never stoppingToken, so a stop does not cut off the write in progress.
		using var deadline = new CancellationTokenSource(Timeout.InfiniteTimeSpan, this.timeProvider);
		using var startDeadline = stoppingToken.Register(() => deadline.CancelAfter(DrainDeadline));

		try
		{
			var canWrite = true;
			try
			{
				while (await this.channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
				{
					// Outside the try: only an entry already read can be lost.
					if (!this.channel.Reader.TryRead(out var request))
					{
						continue;
					}

					canWrite = await this.WriteAsync(request, logDirectory, deadline.Token).ConfigureAwait(false);
					if (!canWrite)
					{
						break;
					}
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
			}

			if (canWrite)
			{
				await this.DrainAsync(logDirectory, deadline.Token).ConfigureAwait(false);
			}
		}
		finally
		{
			this.Discard();
		}

		this.logger.LogInformation("{Status} logs processor for {LogProcessorRequest}", "Stopped",
			typeof(TRequest).Name);
	}

	// Takes no new entries, then writes what is queued until the deadline.
	private async Task DrainAsync(string logDirectory, CancellationToken deadline)
	{
		this.channel.Writer.TryComplete();
		while (!deadline.IsCancellationRequested && this.channel.Reader.TryRead(out var request))
		{
			if (!await this.WriteAsync(request, logDirectory, deadline).ConfigureAwait(false))
			{
				return;
			}
		}
	}

	// False when the processor must stop writing: too many failures in a row, or the stop deadline passed.
	private async Task<bool> WriteAsync(TRequest request, string logDirectory, CancellationToken deadline)
	{
		try
		{
			var log = request.ToLogEntry(this.timeProvider.GetUtcNow());

			var date = this.timeProvider.GetLocalNow();
			var fileName = string.Format(
				this.options.FileNameFormat,
				date.ToString(this.options.DateFormat)
			);
			var logFile = Path.Combine(logDirectory, fileName);
			var json = JsonSerializer.Serialize(log);

			await using (var writer = new StreamWriter(logFile, append: true))
			{
				await writer.WriteLineAsync(json.AsMemory(), deadline).ConfigureAwait(false);
				// Ensure data is written to disk
				await writer.FlushAsync(deadline).ConfigureAwait(false);
			}

			this.exceptionsCount = 0;
			this.options.OnWritten?.Invoke();
			return true;
		}
		catch (OperationCanceledException) when (deadline.IsCancellationRequested)
		{
			this.options.OnDiscarded?.Invoke();
			return false;
		}
		catch (Exception ex)
		{
			this.options.OnWriteFailed?.Invoke(ex);
			this.logger.LogError(ex, "An error occurred while processing logs for {LogProcessorRequest}",
				typeof(TRequest).Name);
			this.exceptionsCount++;

			if (this.exceptionsCount >= this.options.MaxConsecutiveAllowedExceptions)
			{
				this.logger.LogCritical(
					"Too many consecutive exceptions, stopping the processor for {LogProcessorRequest}",
					typeof(TRequest).Name);
				return false;
			}

			return true;
		}
	}

	// Once completed, the channel refuses new writes, so nothing queues behind a processor that is gone.
	private void Discard()
	{
		this.channel.Writer.TryComplete();
		while (this.channel.Reader.TryRead(out _))
		{
			this.options.OnDiscarded?.Invoke();
		}
	}

	private void EnsureDirectoryExists(string logDirectory)
	{
		try
		{
			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}
		}
		catch (Exception ex)
		{
			this.logger.LogCritical(ex, "Could not create log directory {LogDirectory}", logDirectory);
		}
	}
}
