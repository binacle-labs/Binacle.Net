using System.Collections;

namespace Binacle.Lib.Data.ResultSelection;

internal class MultipleScenarioCollectionsReader : IEnumerable<CollectionScenario>
{
    private readonly List<CollectionScenario> scenarios;
    internal MultipleScenarioCollectionsReader(string[] collectionKeys)
    {
        this.scenarios = new List<CollectionScenario>();
        foreach (var collectionKey in collectionKeys)
        {
            var collectionScenarios = ScenarioCollectionsReader.GetScenarios(collectionKey)
                .Select(x => new CollectionScenario(collectionKey, x));
            this.scenarios.AddRange(collectionScenarios);
        }
    }
    public virtual IEnumerator<CollectionScenario> GetEnumerator()
    {
        foreach (var scenario in this.scenarios)
        {
            yield return scenario;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
