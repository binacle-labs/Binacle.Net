# Times the code with BenchmarkDotNet. One project per question; one recipe per project and tier.
#
#   just bench                                the list, each recipe with its cost
#   just bench lib-algorithms-smoke           one recipe
#   just bench <recipe> ffd packing           words narrow the run: ffd, bfd, wfd, packing, fitting
#   just bench <recipe> --iterationTime 100   anything from the first -word on goes to BenchmarkDotNet as is
#
# Reports land in the project's BenchmarkDotNet.Artifacts/results/, gitignored. A run worth keeping is copied
# into <slice>/results/benchmarks/<family>/ by hand, dated.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# BenchmarkDotNet already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through.
set no-exit-message := true

algorithms := 'lib/bench/Binacle.Lib.Benchmarks.Algorithms'

# List the recipes
default:
    @just --list bench

# The sample tier
lib-algorithms *words: (lib-algorithms-sample words)

# FFD, WFD, BFD x v1, v2 on four scenarios: 48 cases, about 4 minutes
lib-algorithms-smoke *words: (_run algorithms 'short' 'smoke' words)

# v1 against v2 per algorithm on 30 picked Bischoff problems: 360 cases, about 30 minutes
lib-algorithms-sample *words: (_run algorithms 'short' 'sample' words)

# v1 against v2 per algorithm on all 700 Bischoff problems: 8,400 cases, about 30 hours; job="short" is about 12
lib-algorithms-full job="default" *words: && (_run algorithms job 'full' words)
    @echo '8,400 cases: about 30 hours at the default job, about 12 at short. Ctrl-C stops it.'

# BestAlgorithm, BestBin, SmallestBin - v1 against v2: 22 cases, about 2 minutes
lib-result-selection *words: (_run 'lib/bench/Binacle.Lib.Benchmarks.ResultSelection' 'short' words)

# One project at one job; the tier and the words are categories the classes carry
[private]
_run project job *categories:
    @tooling/bench.run.sh {{ quote(project) }} {{ quote(job) }} {{ categories }}
