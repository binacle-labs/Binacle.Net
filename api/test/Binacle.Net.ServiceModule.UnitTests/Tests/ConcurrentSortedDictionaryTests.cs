using System.Collections;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;

namespace Binacle.Net.ServiceModule.UnitTests.Tests;

// The in-memory account and subscription repositories keep everything in one of these, in a static field, so
// every request in the process shares it. A lock that does not hold returns a wrong answer rather than throwing,
// which is why the two concurrency cases at the bottom exist.
public class ConcurrentSortedDictionaryTests
{
	private static ConcurrentSortedDictionary<int, string> Populated()
	{
		var dictionary = new ConcurrentSortedDictionary<int, string>();
		dictionary.Add(3, "three");
		dictionary.Add(1, "one");
		dictionary.Add(2, "two");
		return dictionary;
	}

	[Fact]
	public void An_added_entry_is_found_by_key()
	{
		var dictionary = Populated();

		dictionary.Count.ShouldBe(3);
		dictionary.ContainsKey(2).ShouldBeTrue();
		dictionary[2].ShouldBe("two");

		dictionary.TryGetValue(2, out var found).ShouldBeTrue();
		found.ShouldBe("two");
	}

	[Fact]
	public void A_key_that_is_not_there_is_reported_missing()
	{
		var dictionary = Populated();

		dictionary.ContainsKey(9).ShouldBeFalse();
		dictionary.TryGetValue(9, out var found).ShouldBeFalse();
		found.ShouldBeNull();
		Should.Throw<KeyNotFoundException>(() => dictionary[9]);
	}

	// The two ways of writing differ, and the repositories use both: Add is for a create that must not clobber,
	// the indexer is for an update.
	[Fact]
	public void Add_refuses_a_duplicate_key_where_the_indexer_overwrites()
	{
		var dictionary = Populated();

		Should.Throw<ArgumentException>(() => dictionary.Add(1, "again"));
		dictionary[1].ShouldBe("one");

		dictionary[1] = "again";
		dictionary[1].ShouldBe("again");
		dictionary.Count.ShouldBe(3);
	}

	[Fact]
	public void Remove_says_whether_it_removed_anything()
	{
		var dictionary = Populated();

		dictionary.Remove(2).ShouldBeTrue();
		dictionary.Remove(2).ShouldBeFalse();
		dictionary.Count.ShouldBe(2);
	}

	[Fact]
	public void Clear_empties_it()
	{
		var dictionary = Populated();

		dictionary.Clear();

		dictionary.Count.ShouldBe(0);
		dictionary.GetKeys().ShouldBeEmpty();
	}

	// Sorted, not insertion-ordered - the whole reason this wraps a SortedDictionary rather than a
	// ConcurrentDictionary. A listing endpoint reads its page order straight off this.
	[Fact]
	public void Keys_values_and_entries_all_come_back_in_key_order()
	{
		var dictionary = Populated();

		dictionary.GetKeys().ShouldBe([1, 2, 3]);
		dictionary.GetValues().ShouldBe(["one", "two", "three"]);
		dictionary.Select(entry => entry.Key).ShouldBe([1, 2, 3]);

		// The untyped enumerator is its own implementation, and a serializer reaching this through IEnumerable
		// is what would find it broken.
		var untyped = new List<int>();
		foreach (KeyValuePair<int, string> entry in (IEnumerable)dictionary)
		{
			untyped.Add(entry.Key);
		}

		untyped.ShouldBe([1, 2, 3]);
	}

	// Every read hands back a copy taken under the lock. Without that, a caller iterating while another request
	// writes gets "Collection was modified" out of a repository that looks read-only.
	[Fact]
	public void A_returned_collection_is_a_snapshot_and_not_a_live_view()
	{
		var dictionary = Populated();

		var keys = dictionary.GetKeys();
		var values = dictionary.GetValues();
		using var entries = dictionary.GetEnumerator();

		dictionary.Add(4, "four");
		dictionary.Remove(1);

		keys.ShouldBe([1, 2, 3]);
		values.ShouldBe(["one", "two", "three"]);

		var enumerated = new List<int>();
		while (entries.MoveNext())
		{
			enumerated.Add(entries.Current.Key);
		}

		enumerated.ShouldBe([1, 2, 3]);
	}

	[Fact]
	public async Task Concurrent_writers_all_land()
	{
		const int writers = 8;
		const int perWriter = 500;
		var dictionary = new ConcurrentSortedDictionary<int, string>();

		await Task.WhenAll(Enumerable.Range(0, writers).Select(writer => Task.Run(() =>
		{
			for (var i = 0; i < perWriter; i++)
			{
				var key = (writer * perWriter) + i;
				dictionary.Add(key, key.ToString());
			}
		})));

		dictionary.Count.ShouldBe(writers * perWriter);
		dictionary.GetKeys().ShouldBe(Enumerable.Range(0, writers * perWriter));
	}

	[Fact]
	public async Task Reading_while_another_thread_writes_does_not_throw()
	{
		var dictionary = Populated();
		using var writing = new CancellationTokenSource();

		var writer = Task.Run(() =>
		{
			for (var i = 100; !writing.IsCancellationRequested; i++)
			{
				dictionary[i] = i.ToString();
				dictionary.Remove(i - 1);
			}
		});

		var reader = Task.Run(() =>
		{
			for (var i = 0; i < 2000; i++)
			{
				foreach (var _ in dictionary)
				{
				}

				_ = dictionary.GetKeys().ToList();
				_ = dictionary.Count;
			}
		});

		await reader;
		await writing.CancelAsync();
		await writer;

		dictionary.ShouldNotBeEmpty();
	}
}
