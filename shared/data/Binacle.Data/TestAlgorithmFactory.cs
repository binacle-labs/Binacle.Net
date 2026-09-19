
namespace Binacle.Data;

public delegate TAlgorithm TestAlgorithmFactory<out TAlgorithm>(ScenarioBin bin, List<ScenarioItem> items);
