#!/usr/bin/env bash
#
# Every test with the coverage collector attached, then the one merge Sonar needs.
#
#   tooling/coverage.run.sh <cobertura|sonar>
#
# Run from the repo root. Writes artifacts/tests and artifacts/coverage; tooling/coverage.table.sh reads them.
# Needs jq for the sonar format, and nothing extra for cobertura.

set -euo pipefail

format="${1:-}"

case "$format" in
cobertura | sonar) ;;
*)
	echo "Unknown format '$format'. Use cobertura or sonar." >&2
	exit 1
	;;
esac

# The whole folder, not only this format's. A leftover cobertura run would still be sitting there after a
# sonar run, and `report` would merge it and look normal. artifacts/coverage holds exactly one run.
rm -rf artifacts/tests artifacts/coverage

# `|| true` because a failing suite still wrote its report, and the table is what says pass or fail. Without
# it a failed run prints no table at all - the one time you most want to see it.
COVERAGE_FORMAT="$format" just test all || true

# sonar.ruby.coverage.reportPaths takes no wildcard, so the ten gem reports have to arrive as one file. It is
# the only coverage setting Sonar has that does not glob; the C# and javascript ones beside it do.
#
# Every .json in the sonar folder is a simplecov report - C# writes .xml there and jest writes .info. A path
# appears in exactly one gem's report, so merging the coverage maps is a plain union.
if [ "$format" = sonar ]; then
	shopt -s nullglob
	reports=(artifacts/coverage/sonar/*.json)

	if [ ${#reports[@]} -gt 0 ]; then
		# Written beside them under a name the glob above does not match, so a half-written merge can never
		# be read back as an eleventh report.
		jq -s '{meta: .[0].meta, coverage: (map(.coverage) | add), groups: {}}' "${reports[@]}" \
			>artifacts/coverage/sonar/ruby.json.tmp

		rm -f "${reports[@]}"
		mv artifacts/coverage/sonar/ruby.json.tmp artifacts/coverage/sonar/ruby.json
	fi
fi
