using System.Text.Json;
using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Files;

namespace Binacle.TestsKernel.Algorithms;

internal static class ScenarioReader
{ 
	private class ReadScenario
	{
		public string? Name { get; set; }
		public string? Bin{ get; set; }
		public string? Metrics { get; set; }
		// An object keyed by algorithm - read as a JsonElement so a wrong shape fails here, naming the scenario.
		public JsonElement? Result { get; set; }
		public string[]? Items { get; set; }
	}

	public static List<Scenario> ReadScenarios(IFile file)
	{
		var resultScenarios = new List<Scenario>();
		using (var sr = new StreamReader(file.OpenRead()))
		{
			var readScenarios = JsonSerializer.Deserialize<List<ReadScenario>>(sr.ReadToEnd());
			if(readScenarios is null)
			{
				return resultScenarios;
			}

			foreach (var readScenario in readScenarios)
			{
				if (string.IsNullOrWhiteSpace(readScenario.Name))
				{
					throw new ArgumentNullException("No name found in scenario");
				}
				if (string.IsNullOrWhiteSpace(readScenario.Bin))
				{
					throw new ArgumentNullException("No bin found in scenario");
				}
				if (string.IsNullOrWhiteSpace(readScenario.Metrics))
				{
					throw new ArgumentNullException("No metrics found in scenario");
				}
				if (readScenario.Result is not { } result || result.ValueKind == JsonValueKind.Null)
				{
					throw new ArgumentNullException("No result found in scenario");
				}
				if (readScenario.Items is null || readScenario.Items.Length < 1)
				{
					throw new ArgumentNullException("No items found in scenario");
				}

				if (result.ValueKind != JsonValueKind.Object)
				{
					throw new ArgumentException(
						$"Invalid result in scenario {readScenario.Name}. Expected an object keyed by algorithm, "
						+ "for example { \"FFD\": \"FullyPacked FullyPacked\" }.");
				}

				resultScenarios.Add(Scenario.Create(
					readScenario.Name,
					readScenario.Bin,
					readScenario.Items,
					readScenario.Metrics,
					result.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString()!)
				));
			}
		}

		return resultScenarios;
	}
}
