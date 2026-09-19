using Binacle.Lib.Testing.Providers;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Data;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_CubeBaseline_Packing : FastValidationBenchmarkBase
{
    protected override Scenario? GetScenario() =>
        CubeScalingProblemsProvider.GetBaseline();
	
    protected override AlgorithmOperation AlgorithmOperation 
        => AlgorithmOperation.Packing;
}
