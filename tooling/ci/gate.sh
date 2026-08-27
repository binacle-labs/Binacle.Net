#!/usr/bin/env bash
# Pass only if every job passed or was skipped.
#   gate.sh '<json of every job and its result>'
set -euo pipefail

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

# Turn the json into one plain line per job: "name result"
jobs=$(jq -r 'to_entries[] | "\(.key) \(.value.result)"' <<<"$1")

failed=0

echo '| Job | Result |' >>"$summary"
echo '|---|---|' >>"$summary"

while read -r name result; do
    echo "| $name | $result |" >>"$summary"

    if [ "$result" != "success" ] && [ "$result" != "skipped" ]; then
        echo "$name failed." >&2
        failed=1
    fi
done <<<"$jobs"

if [ "$failed" = 1 ]; then
    echo "" >>"$summary"
    echo "The gate failed. A job above did not pass." >>"$summary"
    exit 1
fi

echo "Every job passed or was skipped."
