#!/usr/bin/env bash
# Runs one benchmark project at one BenchmarkDotNet job. The door is tooling/bench.just.
#
#   tooling/bench.run.sh <project> <job> [categories...] [-- flags for BenchmarkDotNet]
#
# A category is a word the classes carry in [BenchmarkCategory]: the tier (smoke, sample, full) and the
# narrowing words (ffd, bfd, wfd, packing, fitting). Every category given must match, so "smoke ffd packing"
# is the AND. Words from the first one starting with - go to BenchmarkDotNet as they are.

set -euo pipefail

project="$1"
job="$2"
shift 2

categories=()
while (( $# > 0 )) && [[ "$1" != -* ]]; do
    categories+=("$1")
    shift
done

filter=()
if (( ${#categories[@]} > 0 )); then filter=(--allCategories "${categories[@]}"); fi

exec dotnet run -c Release --project "$project" -- --job "$job" "${filter[@]}" "$@"
