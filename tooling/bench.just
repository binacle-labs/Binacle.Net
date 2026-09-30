# Times the code with BenchmarkDotNet. One project per question; one recipe per project and tier.
#
#   just bench                               the list, in tier order, each recipe with its cost
#   just bench lib-algorithms-smoke          smoke: minutes, takes nothing
#   just bench lib-algorithms-sample quick   sample: the default job; `quick` runs the short one
#   just bench lib-algorithms-full precise   full: hours, asks first; the short job unless `precise`
#
# The tier is the class name's first word: Smoke_, Sample_, Full_. Result selection and scaling have one tier
# and run every class. The parallel projects have no tiers: one recipe per project runs its Identical_ classes.
# Reports land in the project's BenchmarkDotNet.Artifacts/results/, gitignored, and the next run of the same
# class overwrites them. A run worth keeping is copied by hand into <slice>/results/benchmarks/<date>/<family>/
# - baseline/ instead when it is that class's first kept run. <slice>/results/benchmarks/README.md says what
# counts as worth keeping.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# BenchmarkDotNet already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through.
set no-exit-message := true

# List the recipes
default:
    @just --unsorted --list bench

# FFD, WFD, BFD x v1, v2 on the smoke scenarios: about 6 minutes
lib-algorithms-smoke: (lib-algorithms-run 'short' 'Smoke')

# v1 against v2 per algorithm on the picked Bischoff problems: about 30 minutes with `quick`
[arg('mode', pattern='|quick')]
lib-algorithms-sample mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=default
    if [ "{{ mode }}" = quick ]; then job=short; fi

    just bench lib-algorithms-run "$job" Sample

# v1 against v2 per algorithm on every Bischoff problem: about 16 hours; `precise` about 30
[arg('mode', pattern='|precise')]
[confirm('lib-algorithms-full: about 16 hours (precise: about 30). Run?')]
lib-algorithms-full mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = precise ]; then job=default; fi

    just bench lib-algorithms-run "$job" Full

[private]
lib-algorithms-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Algorithms -- --job {{ job }} --filter '*.{{ tier }}_*'

# Loop against Parallel for both sets and each algorithm alone, v2, identical bin and items, 1 to 256 pieces x 2, 4, 8, 12 cores: about 20 to 30 minutes; `dry` runs each case once
[arg('mode', pattern='|dry')]
lib-parallel-algorithms-identical mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = dry ]; then job=dry; fi

    just bench lib-parallel-algorithms-run "$job" Identical

[private]
lib-parallel-algorithms-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms -- --job {{ job }} --filter '*.{{ tier }}_*'

# Loop against Parallel, one algorithm, v2, identical bins and items, every item fits: 1 to 32 bins x 1 to 256 pieces, then 64 and 256 pieces over 1 to 64 lines on FFD, x 2, 4, 8, 12 cores: about 1 hour for FFD, not measured for WFD and BFD; `dry` runs each case once
[arg('algorithm', pattern='FFD|WFD|BFD')]
[arg('mode', pattern='|dry')]
lib-parallel-bins-identical algorithm mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = dry ]; then job=dry; fi

    just bench lib-parallel-bins-run "$job" "Identical_{{ algorithm }}"

[private]
lib-parallel-bins-run job prefix:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ParallelBins -- --job {{ job }} --filter '*.{{ prefix }}_*'

# BestAlgorithm, BestBin, SmallestBin - v1 against v2: about 3 minutes. Its one tier
lib-result-selection: (lib-result-selection-run 'short')

[private]
lib-result-selection-run job:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ResultSelection -- --job {{ job }} --filter '*'

# FFD, WFD, BFD x v1, v2 over the item ladder: about 20 minutes. Its one tier
[arg('mode', pattern='|quick')]
lib-scaling mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=default
    if [ "{{ mode }}" = quick ]; then job=short; fi

    just bench lib-scaling-run "$job"

[private]
lib-scaling-run job:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Scaling -- --job {{ job }} --filter '*'

# Encode and Decode on the smoke packs - protobuf against ViPaq's two layouts, and JSON on encode: about 3 minutes
vipaq-smoke: (vipaq-run 'short' 'Smoke')

# Encode, Decode on the sample packs, and CompressionCost: about 30 minutes; about 7 with `quick`
[arg('mode', pattern='|quick')]
vipaq-sample mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=default
    if [ "{{ mode }}" = quick ]; then job=short; fi

    just bench vipaq-run "$job" Sample

[private]
vipaq-run job tier:
    dotnet run -c Release --project vipaq/bench/Binacle.ViPaq.Benchmarks -- --job {{ job }} --filter '*.{{ tier }}_*'
