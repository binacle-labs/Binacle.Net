using System.Collections.ObjectModel;

namespace Binacle.Net.ServiceModule.IntegrationTests;

// The exporters add from the request and export threads while a test reads.
public sealed class ExportedItems<T> : Collection<T>
{
	protected override void InsertItem(int index, T item)
	{
		lock (this.Items)
		{
			base.InsertItem(index, item);
		}
	}

	public T[] Snapshot()
	{
		lock (this.Items)
		{
			return [.. this.Items];
		}
	}
}
