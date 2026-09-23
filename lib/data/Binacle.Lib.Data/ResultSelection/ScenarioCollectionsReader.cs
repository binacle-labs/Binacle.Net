using System.Collections.ObjectModel;
using Binacle.Data.Files;

namespace Binacle.Lib.Data.ResultSelection;

internal static class ScenarioCollectionsReader
{
	private const string ResourcePrefix = "ResultSelection.";

	private static Dictionary<string, List<Scenario>> collections;

	public static ReadOnlyDictionary<string, List<Scenario>> Collections => collections.AsReadOnly();
	static ScenarioCollectionsReader()
	{
		collections = new Dictionary<string, List<Scenario>>();

		var files = EmbeddedResourceFileProvider.ByPrefix(typeof(ScenarioCollectionsReader).Assembly, ResourcePrefix);

		foreach (var file in files)
		{
			var collectionKey = GetCollectionKey(file);
			using var stream = file.OpenRead();
			var scenarios = ScenarioReader.ReadScenarios(stream);
			collections.Add(collectionKey, scenarios);
		}
	}

	// "<Set>.<name>.json" -> "<set>/<name>"
	private static string GetCollectionKey(EmbeddedResourceFile file)
	{
		var parts = file.RelativeName.Split('.');
		if (parts.Length != 3)
		{
			throw new ArgumentException($"Resource '{file.ResourceName}' is not the expected <set>.<name>.<extension> shape.");
		}

		return $"{parts[0]}/{parts[1]}".ToLower();
	}

	public static List<Scenario> GetScenarios(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!collections.TryGetValue(normalizedKey, out var scenarios))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return scenarios;
	}
}
