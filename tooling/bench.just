# Times the code with BenchmarkDotNet. One project per question; one recipe per project and tier.
#
#   just bench                               the list, in tier order, each recipe with its cost
#   just bench lib-algorithms-smoke          the recipe's own default job
#   just bench lib-algorithms-sample quick   the short job
#   just bench lib-algorithms-full precise   the default job; full asks first
#   just bench lib-scaling dry               each case once: checks the recipe runs, times nothing worth keeping
#   just bench lib-scaling dry 3             three processes per case, so a run-to-run spread shows
#
# Every recipe takes the same two optional arguments, a word and then a launch count. The word means the same
# job on every recipe - `dry` each case once, `quick` the short job, `precise` the default job - and with no
# word a recipe runs the job on its own line, which its comment also names. The launch count overrides the
# job's own and is what makes a run-to-run spread visible. `just --list` shows only the last line of a
# comment, which is why the recipe comments are one line.
#
# The tier is the class name's first word: Smoke_, Sample_, Full_. Result selection and scaling have one tier
# and run every class. The parallel projects have no tiers: one recipe per project runs its Identical_ classes.
# Reports land in the project's BenchmarkDotNet.Artifacts/results/, gitignored, and the next run of the same
# class overwrites them. A run worth keeping is copied by hand into <slice>/results/benchmarks/baseline/<family>/.
# <slice>/results/benchmarks/README.md says what counts as worth keeping.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# BenchmarkDotNet already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through.
set no-exit-message := true

# List the recipes
default:
    @just --unsorted --list bench

# FFD, WFD, BFD x v1, v2 on the smoke scenarios: about 6 minutes; default: quick
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-algorithms-smoke mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.Algorithms' '*.Smoke_*' short "{{ mode }}" "{{ launches }}"

# v1 against v2 per algorithm on the picked Bischoff problems: about 30 minutes with `quick`; default: precise
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-algorithms-sample mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.Algorithms' '*.Sample_*' default "{{ mode }}" "{{ launches }}"

# v1 against v2 per algorithm on every Bischoff problem: about 16 hours; `precise` about 30; default: quick; asks first, except for `dry`
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-algorithms-full mode='' launches='':
    #!/usr/bin/env bash
    set -euo pipefail

    if [ "{{ mode }}" != dry ]; then
        read -r -p "lib-algorithms-full: about 16 hours (precise: about 30). Run? [y/N] " answer
        if [ "$answer" != y ] && [ "$answer" != Y ]; then exit 1; fi
    fi

    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.Algorithms' '*.Full_*' short "{{ mode }}" "{{ launches }}"

# Loop against Parallel for both races and for each algorithm alone, v2, identical bin and items, 1 to 256 pieces x 2, 4, 8, 12 cores: about 40 minutes quick, measured 2026-10-03; default: quick. The filter takes every class on purpose - Loop against the sum of the alone classes only holds within one invocation
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-parallel-algorithms-identical mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms' '*.Identical_*' short "{{ mode }}" "{{ launches }}"

# Loop against Parallel, one algorithm, v2, identical bins and items, every item fits: 1 to 32 bins x 1 to 256 pieces, then 64 and 256 pieces over 1 to 64 lines, x 2, 4, 8, 12 cores: about 70 minutes for FFD quick, measured 2026-10-03; default: quick
[arg('algorithm', pattern='FFD|WFD|BFD')]
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-parallel-bins-identical algorithm mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.ParallelBins' '*.Identical_{{ algorithm }}_*' short "{{ mode }}" "{{ launches }}"

# What parallelising costs, with algorithms that do no packing: a two- and a three-algorithm race, and 1 to 8 bins, each walking 0 or 64 KB, x 1, 2, 4, 8, 12 cores: about 1 hour, `quick` about 12; default: precise
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-parallel-overhead mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.ParallelOverhead' '*' default "{{ mode }}" "{{ launches }}"

# BestAlgorithm, BestBin, SmallestBin - v1 against v2: about 3 minutes. Its one tier; default: quick
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-result-selection mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.ResultSelection' '*' short "{{ mode }}" "{{ launches }}"

# FFD, WFD, BFD x v1, v2 over the item ladder: about 20 minutes. Its one tier; default: precise
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
lib-scaling mode='' launches='':
    just bench _bench_run 'lib/bench/Binacle.Lib.Benchmarks.Scaling' '*' default "{{ mode }}" "{{ launches }}"

# Encode and Decode on the smoke packs - protobuf against ViPaq's two layouts, and JSON on encode: about 3 minutes; default: quick
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
vipaq-smoke mode='' launches='':
    just bench _bench_run 'vipaq/bench/Binacle.ViPaq.Benchmarks' '*.Smoke_*' short "{{ mode }}" "{{ launches }}"

# Encode, Decode on the sample packs, and CompressionCost: about 30 minutes; about 7 with `quick`; default: precise
[arg('mode', pattern='|dry|quick|precise')]
[arg('launches', pattern='|[1-9][0-9]*')]
vipaq-sample mode='' launches='':
    just bench _bench_run 'vipaq/bench/Binacle.ViPaq.Benchmarks' '*.Sample_*' default "{{ mode }}" "{{ launches }}"

# Every recipe above runs through this one, so the word and the launch count are read in a single place.
#   default   the job to run when no word was given
#   mode      the word the caller gave: nothing, dry, quick or precise
[private]
_bench_run project filter default mode launches:
    #!/usr/bin/env bash
    set -euo pipefail

    job={{ default }}
    if [ "{{ mode }}" = dry ]; then job=dry; fi
    if [ "{{ mode }}" = quick ]; then job=short; fi
    if [ "{{ mode }}" = precise ]; then job=default; fi

    # Unquoted below, so an empty value disappears instead of becoming an empty argument.
    launches=""
    if [ -n "{{ launches }}" ]; then launches="--launches {{ launches }}"; fi

    dotnet run -c Release --project {{ project }} -- --job "$job" $launches --filter '{{ filter }}'
