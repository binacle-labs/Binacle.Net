# Regenerates the data that is committed to the repository.
#
#   just regen all                     every generator, in order
#   just regen or-lib-scenarios        OR-Library text -> shared/data/bischoff-suite
#   just regen vipaq-packed-data       those + custom-problems, packed -> vipaq/data/packed
#   just regen vipaq-interop-vectors   the interop pair -> vipaq/test-vectors
#   just regen demo-samples            shared/data/demo-samples -> the demo's sample set
#   just regen check                   regenerate, then fail if any of it changed
#
# Every tool here rewrites committed files. No recipe takes an argument, so a run does its whole list and
# cannot leave the data half written.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The tools print why they failed, so just's own "Recipe failed" line adds nothing. The exit code still
# gets through.
set no-exit-message := true

ts_package := "vipaq/packages/binacle-vipaq"
ui_package := "packages/binacle-net-ui"

# List the recipes
default:
    @just --list regen

# or-lib-scenarios writes what vipaq-packed-data reads, so the order is the recipe.

# Run every generator
all: or-lib-scenarios vipaq-packed-data vipaq-interop-vectors demo-samples

# OR-Library text -> the tests-kernel scenarios in shared/data/bischoff-suite
or-lib-scenarios:
    dotnet run --project shared/tools/Binacle.OrLibrary.Converter

# Pack those + custom-problems -> the placed results in vipaq/data/packed
vipaq-packed-data:
    dotnet run --project vipaq/tools/Binacle.ViPaq.PackedDataGenerator

# The two halves write cs/ and ts/ of the same matrix from one input.json. Running one alone is the drift
# InteropIntegrityTests exists to catch, so they are one recipe.

# Both interop halves and the header bytes -> vipaq/test-vectors
vipaq-interop-vectors:
    dotnet run --project vipaq/tools/Binacle.ViPaq.VectorGenerators
    npx ts-node --project {{ ts_package }}/tsconfig.json {{ ts_package }}/tools/generateVectors.ts

# shared/data/demo-samples -> the demo's sample set in packages/binacle-net-ui
demo-samples:
    npx ts-node --project {{ ui_package }}/tsconfig.json {{ ui_package }}/tools/generateSamples.ts

# Every generator is deterministic and its output is committed, so a clean tree after a full run is the whole
# check. Nothing in CI calls this.

# Regenerate everything, then fail if the committed data changed
check: all
    #!/usr/bin/env bash
    set -euo pipefail

    # Only what the generators write. These folders also hold their own README, vipaq/test-vectors holds the
    # hand-written vectors no generator touches, and the demo's package is otherwise hand-written.
    generated=(
        ':(glob)shared/data/bischoff-suite/**/*.json'
        ':(glob)vipaq/data/packed/**/*.json'
        ':(glob)vipaq/test-vectors/header/**/*.json'
        ':(glob)vipaq/test-vectors/interop/cs/**/*.json'
        ':(glob)vipaq/test-vectors/interop/ts/**/*.json'
        'packages/binacle-net-ui/src/utils/sampleData.ts'
    )

    changed="$(git status --porcelain -- "${generated[@]}")"
    if [[ -n "$changed" ]]; then
        echo "Committed data does not match what the generators produce:" >&2
        echo "$changed" >&2
        exit 1
    fi

    echo "Committed data matches the generators."
