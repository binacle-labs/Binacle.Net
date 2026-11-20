using System.Threading.Channels;

namespace Binacle.Net.DiagnosticsModule.Telemetry;

// Counts each write the inner channel refuses. With DropWrite or no bound, it refuses only once completed.
internal sealed class RefusalCountingChannel<T> : Channel<T>
{
	public RefusalCountingChannel(Channel<T> inner, Action onRefused)
	{
		this.Reader = inner.Reader;
		this.Writer = new RefusalCountingWriter(inner.Writer, onRefused);
	}

	private sealed class RefusalCountingWriter : ChannelWriter<T>
	{
		private readonly ChannelWriter<T> inner;
		private readonly Action onRefused;

		public RefusalCountingWriter(ChannelWriter<T> inner, Action onRefused)
		{
			this.inner = inner;
			this.onRefused = onRefused;
		}

		public override bool TryWrite(T item)
		{
			if (this.inner.TryWrite(item))
			{
				return true;
			}

			this.onRefused();
			return false;
		}

		public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
			=> this.inner.WaitToWriteAsync(cancellationToken);

		public override bool TryComplete(Exception? error = null) => this.inner.TryComplete(error);
	}
}
