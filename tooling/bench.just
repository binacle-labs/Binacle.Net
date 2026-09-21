# Times the code with BenchmarkDotNet. One project per question; one recipe per project and tier.
#
#   just bench                                the list, each recipe with its cost
#   just bench lib-result-selection           one recipe
#   just bench <recipe> ffd packing           words narrow the run: ffd, bfd, wfd, packing, fitting
#   just bench <recipe> --iterationTime 100   anything else goes to BenchmarkDotNet as is
#
# Reports land in the project's BenchmarkDotNet.Artifacts/results/, gitignored. A run worth keeping is copied
# into <slice>/results/benchmarks/<family>/ by hand, dated.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# BenchmarkDotNet already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through.
set no-exit-message := true

# List the recipes
default:
    @just --list bench

# BestAlgorithm, BestBin, SmallestBin - v1 against v2: 22 cases, about 2 minutes
lib-result-selection *words: (_run 'lib/bench/Binacle.Lib.Benchmarks.ResultSelection' 'short' words)

# One project at one job; the words are the script's to sort
[private]
_run project job *words:
    @tooling/bench.run.sh {{ quote(project) }} {{ quote(job) }} {{ words }}
