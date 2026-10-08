# Measures what the code does and writes the numbers into the repository.
#
#   just measure all       both slices
#   just measure lib       Binacle.Lib.PackingEfficiency -> lib/results/measurements/
#   just measure vipaq     Binacle.ViPaq.EncodedSize     -> vipaq/results/measurements/
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
    @just --unsorted --list measure

# Both slices
all: lib vipaq

# Pack every scenario with every algorithm -> lib/results/measurements/
lib:
    dotnet run -c Release --project lib/measure/Binacle.Lib.PackingEfficiency

# Encode every pack every way -> vipaq/results/measurements/
vipaq:
    dotnet run -c Release --project vipaq/measure/Binacle.ViPaq.EncodedSize
