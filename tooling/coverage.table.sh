#!/usr/bin/env bash
#
# The verdict on the last coverage run: one row per suite, then coverage per project.
#
#   tooling/coverage.table.sh [cobertura|sonar]
#
# Run from the repo root. Reads artifacts/tests and artifacts/coverage, both written by tooling/tests.just.
# The exit code is the run's - non-zero if any suite failed or wrote nothing.

# No -e. A grep that finds nothing must leave a blank cell, not kill the table.
set -uo pipefail
shopt -s nullglob

format="${1:-cobertura}"

total_failed=0
seen=""

# The numbers sit in a small summary object, so one grep each is enough - no json parser needed.
count() { grep -o "\"$2\":[0-9]*" "$1" | head -1 | cut -d: -f2; }

row() {
	printf '%-44s Passed:%-6s Failed:%-4s Skipped:%s\n' "$1" "$2" "$3" "$4"
	total_failed=$((total_failed + $3))
	seen="$seen $1"
}

echo ""

# C# writes CTRF, jest writes its own json and rspec writes a third. The same three numbers under three
# sets of field names.
for report in artifacts/tests/*.ctrf.json; do
	name=$(basename "$report" .ctrf.json)
	row "$name" "$(count "$report" passed)" "$(count "$report" failed)" "$(count "$report" skipped)"
done

for report in artifacts/tests/*.jest.json; do
	name=$(basename "$report" .jest.json)
	row "$name" "$(count "$report" numPassedTests)" "$(count "$report" numFailedTests)" \
		"$(count "$report" numPendingTests)"
done

for report in artifacts/tests/*.rspec.json; do
	name=$(basename "$report" .rspec.json)
	row "$name" "$(count "$report" example_count)" "$(count "$report" failure_count)" \
		"$(count "$report" pending_count)"
done

# A suite that ran and wrote nothing would be missing from the table, which reads as "it does not exist".
# Every test writes its name into expected.txt before it runs, so a missing report is a red row instead.
while IFS= read -r name; do
	[ -n "$name" ] || continue
	case " $seen " in *" $name "*) continue ;; esac
	printf '%-44s no report\n' "$name"
	total_failed=$((total_failed + 1))
done < <(sort -u artifacts/tests/expected.txt 2>/dev/null)

echo ""

# A suite is not a project. Several suites load the same code, and each one's file holds only what that run
# reached - Binacle.CompactNotation reads 34% in the Lib suite's file and 100% once the two are put together.
# The union across every file is the only honest number, and reportgenerator is what computes it. Only
# cobertura carries the per-line data, so a sonar run gets no coverage block.
#
# Not comparable with SonarCloud. Sonar counts coverable lines its own way and drops whole trees that are in
# scope here. These rows say which project to work on, not what the gate will read.
if [ "$format" = cobertura ] && [ -d artifacts/coverage/cobertura ]; then
	dotnet tool restore >/dev/null

	# Inputs and filters come from .netconfig at the repo root, which is reportgenerator's own config file.
	dotnet reportgenerator \
		-targetdir:'artifacts/coverage/summary' \
		-reporttypes:'TextSummary' \
		>/dev/null

	summary=artifacts/coverage/summary/Summary.txt
	if [ -f "$summary" ]; then
		# A project sits at column 0 and its classes are indented under it, so the leading space is what
		# tells them apart.
		awk '/^[^ ]/ && $0 != "Summary" { printf "%-44s %s\n", $1, $NF }' "$summary"
		echo ""
		grep 'Line coverage:' "$summary" | sed 's/^  //'
		echo ""
	fi
fi

if [ "$total_failed" -gt 0 ]; then
	echo "$total_failed failed - scroll up for the failures, each suite printed its own."
	exit 1
fi

echo "All suites passed."
