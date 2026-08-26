using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.Lib.Benchmarks.Providers;

public static class CubeScalingProblemsProvider
{
    public static Scenario GetBaseline()
    {
        return Scenario.Create(
            name: "CubeBaseline",
            bin: "60x40x10",
            items: ["5x5x5 [192]"],
            metrics: "24000 24000 192 100.00",
            results: new Dictionary<string, string>
            {
                ["FFD"] = "FullyPacked FullyPacked",
                ["WFD"] = "FullyPacked FullyPacked",
                ["BFD"] = "FullyPacked FullyPacked"
            }
        );
    }
}
