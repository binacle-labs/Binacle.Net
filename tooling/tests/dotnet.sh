#!/usr/bin/env bash
# Runs one C# test project. Every C# test goes through here, so the flags sit in one place.
#   dotnet.sh <project folder> [label]
#
# With COVERAGE_FORMAT unset this is a plain `dotnet test`. With it set, the same run also writes a test
# report and a coverage file under artifacts/. `just coverage all` is what sets it.
#
# DOTNET_TEST_ARGS is appended to every run. CI builds the solution once and passes --no-build; on a laptop
# it is unset.
set -euo pipefail

project="$1"
label="${2:-}"
read -ra extra_args <<<"${DOTNET_TEST_ARGS:-}"

if [ -z "${COVERAGE_FORMAT:-}" ]; then
    exec dotnet test --project "$project" "${extra_args[@]}"
fi

# The format name says who reads the file, not what the file is.
case "$COVERAGE_FORMAT" in
    cobertura) output_format=cobertura ;;
    sonar)     output_format=xml ;;  # Visual Studio coverage, the only C# format Sonar reads
    *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
esac

# The label goes on both file names. Without it a project run twice overwrites its own first run.
name=$(basename "$project")
if [ -n "$label" ]; then
    name="$name.$label"
fi

results="$(pwd)/artifacts/tests"
coverage="$(pwd)/artifacts/coverage/$COVERAGE_FORMAT"
mkdir -p "$results" "$coverage"

# Written before the run, so `just coverage table` can tell "wrote no report" from "never ran".
echo "$name" >> "$results/expected.txt"

# Both paths are absolute. A relative coverage path lands inside the project's own bin folder.
#
# No `--` in front of these options. `dotnet test` hands anything it does not recognise to the test app
# itself, and a `--` makes it read the project path as junk.
dotnet test --project "$project" "${extra_args[@]}" \
    --results-directory "$results" \
    --report-ctrf --report-ctrf-filename "$name.ctrf.json" \
    --coverage --coverage-output-format "$output_format" \
    --coverage-output "$coverage/$name.xml"
