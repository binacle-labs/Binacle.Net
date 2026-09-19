# Measures what the code does and writes the numbers into the repository.
#
#   just measure all       both slices
#   just measure lib       Binacle.Lib.PackingEfficiency -> lib/results/
#   just measure vipaq     Binacle.ViPaq.EncodedSize     -> vipaq/results/
#   just measure check     all, then fail if either results/ changed
#
# Every project here rewrites tracked files; a run that changes nothing leaves a clean tree. The numbers are
# deterministic - packing efficiency, encoded size - never timings, which stay in the benchmark projects.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The projects print why they failed, so just's own "Recipe failed" line adds nothing. The exit code still
# gets through.
set no-exit-message := true

# List the recipes
default:
    @just --list measure

# Both slices
all: lib vipaq

# Pack every scenario with every algorithm -> lib/results/
lib:
    dotnet run -c Release --project lib/measure/Binacle.Lib.PackingEfficiency

# Encode every pack every way -> vipaq/results/
vipaq:
    dotnet run -c Release --project vipaq/measure/Binacle.ViPaq.EncodedSize

# Every project is deterministic and its output is committed, so a clean tree after a full run is the whole
# check. Nothing in CI calls this.

# Measure everything, then fail if the committed results changed
check: all
    #!/usr/bin/env bash
    set -euo pipefail

    changed="$(git status --porcelain -- lib/results vipaq/results)"
    if [[ -n "$changed" ]]; then
        echo "Committed results do not match what the measure projects produce:" >&2
        echo "$changed" >&2
        exit 1
    fi

    echo "Committed results match the measure projects."
