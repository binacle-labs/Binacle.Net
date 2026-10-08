
namespace Binacle.Lib.Testing;

public delegate TAlgorithm TestAlgorithmFactory<out TAlgorithm>(ScenarioBin bin, List<ScenarioItem> items);
