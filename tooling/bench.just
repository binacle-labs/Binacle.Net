# Times the code with BenchmarkDotNet. One project per question; one recipe per project and tier.
#
#   just bench                               the list, in tier order, each recipe with its cost
#   just bench lib-algorithms-smoke          smoke: minutes, takes nothing
#   just bench lib-algorithms-sample quick   sample: the default job; `quick` runs the short one
#   just bench lib-algorithms-full precise   full: hours, asks first; the short job unless `precise`
#
# The tier is the class name's first word: Smoke_, Sample_, Full_, and Cores_ for racing. Any other word fails
# before a run starts.
# Reports land in the project's BenchmarkDotNet.Artifacts/results/, gitignored, and the next run of the same
# class overwrites them. A run worth keeping is copied by hand into <slice>/results/benchmarks/<date>/<family>/
# - baseline/ instead when it is that class's first kept run. That folder's README says what counts as worth
# keeping.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# BenchmarkDotNet already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through.
set no-exit-message := true

# List the recipes
default:
    @just --unsorted --list bench

# FFD, WFD, BFD x v1, v2 on four scenarios: 48 cases, about 6 minutes
lib-algorithms-smoke: (lib-algorithms-run 'short' 'Smoke')

# v1 against v2 per algorithm on 30 picked Bischoff problems: 360 cases, about 30 minutes with `quick`
[arg('mode', pattern='|quick')]
lib-algorithms-sample mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=default
    if [ "{{ mode }}" = quick ]; then job=short; fi

    just bench lib-algorithms-run "$job" Sample

# v1 against v2 per algorithm on all 700 Bischoff problems: 8,400 cases, about 12 hours; `precise` about 30
[arg('mode', pattern='|precise')]
[confirm('lib-algorithms-full: 8,400 cases, about 12 hours (precise: about 30). Run?')]
lib-algorithms-full mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = precise ]; then job=default; fi

    just bench lib-algorithms-run "$job" Full

[private]
lib-algorithms-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Algorithms -- --job {{ job }} --filter '*.{{ tier }}_*'

# Loop against Parallel for both sets and each algorithm alone, v2, 30 problems x 2, 4, 8, 12 cores: 840 cases, about 70 minutes; `precise` about 3.5 hours
[arg('mode', pattern='|precise')]
[confirm('lib-racing-cores: 840 cases, about 70 minutes (precise: about 3.5 hours). Run?')]
lib-racing-cores mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = precise ]; then job=default; fi

    just bench lib-racing-run "$job" Cores

[private]
lib-racing-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Racing -- --job {{ job }} --filter '*.{{ tier }}_*'

# Loop against Parallel at four item counts, both sets; and over bins 2, 3, 7 for FFD and BFD: 64 cases, about 8 minutes
lib-threshold-smoke: (lib-threshold-run 'short' 'Smoke')

# The whole item ladder for the sets, and bins 1-7 at three item counts: 128 cases, about 11 minutes with `quick`
[arg('mode', pattern='|quick')]
lib-threshold-sample mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=default
    if [ "{{ mode }}" = quick ]; then job=short; fi

    just bench lib-threshold-run "$job" Sample

# v1 and v2 over the whole ladder and bins 1-7: 704 cases, about 1 hour; `precise` about 3
[arg('mode', pattern='|precise')]
[confirm('lib-threshold-full: 704 cases, about 1 hour (precise: about 3). Run?')]
lib-threshold-full mode='':
    #!/usr/bin/env bash
    set -euo pipefail

    job=short
    if [ "{{ mode }}" = precise ]; then job=default; fi

    just bench lib-threshold-run "$job" Full

[private]
lib-threshold-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Threshold -- --job {{ job }} --filter '*.{{ tier }}_*'

# BestAlgorithm, BestBin, SmallestBin - v1 against v2: 22 cases, about 3 minutes. Its one tier
lib-result-selection: (lib-result-selection-run 'short')

[private]
lib-result-selection-run job:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ResultSelection -- --job {{ job }} --filter '*'

# FFD, WFD, BFD x v1, v2 over the 11-step item ladder: 66 cases, about 20 minutes. Its one tier
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

# Encode and Decode on three packs - protobuf against ViPaq's two layouts: 18 cases, about 3 minutes
vipaq-smoke: (vipaq-run 'short' 'Smoke')

# Encode, Decode on twelve packs, and CompressionCost: 84 cases, about 20 minutes; about 7 with `quick`
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
