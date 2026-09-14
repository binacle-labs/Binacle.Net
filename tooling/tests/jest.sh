#!/usr/bin/env bash
# Runs one TypeScript package. The jest half of dotnet.sh, with the same two modes.
#   jest.sh <package's jest displayName>
#
# --selectProjects and not --projects <path>, because the root jest.config.js keeps the workspace folder in
# the recorded paths, and its collectCoverageFrom is ignored when a package's own config drives the run.
set -euo pipefail

package="$1"

if [ -z "${COVERAGE_FORMAT:-}" ]; then
    exec npx jest --selectProjects "$package"
fi

# The same two format names as dotnet.sh, in what jest calls them. Sonar reads javascript coverage as lcov.
case "$COVERAGE_FORMAT" in
    cobertura) reporter=cobertura ;;
    sonar)     reporter=lcovonly ;;
    *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
esac

results="$(pwd)/artifacts/tests"
coverage="$(pwd)/artifacts/coverage/$COVERAGE_FORMAT"
mkdir -p "$results" "$coverage"
echo "$package" >> "$results/expected.txt"

# Paths are absolute. jest resolves a relative one against the package, not against artifacts/.
npx jest --selectProjects "$package" \
    --coverage --coverageReporters="$reporter" --coverageDirectory="$coverage/$package" \
    --json --outputFile="$results/$package.jest.json"

# jest names the file after its reporter and buries it in a folder. Lift it out, named after the package, so
# the folder holds one flat file per project.
for produced in "$coverage/$package"/*; do
    [ -f "$produced" ] || continue
    mv "$produced" "$coverage/$package.${produced##*.}"
done
rmdir "$coverage/$package"
