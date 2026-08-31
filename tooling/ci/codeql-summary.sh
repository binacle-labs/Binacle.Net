#!/usr/bin/env bash
# Count the open code scanning alerts by severity and write them to the run summary.
#   codeql-summary.sh <owner/repo> <ref> <branch> <result of the analyse job>        needs GH_TOKEN
set -euo pipefail

repo="$1"
ref="$2"
branch="$3"
analysis="$4"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

# --paginate: the endpoint caps at 100 per page and a silent truncation reads as "fewer alerts now".
alerts=$(gh api --paginate \
    "repos/${repo}/code-scanning/alerts?state=open&ref=${ref}&per_page=100" \
    --jq '.[].rule.security_severity_level // "none"' | sort | uniq -c)

# `uniq -c` prints nothing for a level with no alerts, and a blank cell reads as unknown, not zero.
count() { local level="$1"; printf '%s\n' "$alerts" | awk -v k="$level" '$2 == k { n = $1 } END { print n + 0 }'; }

{
    echo '## CodeQL'
    echo
    # A failed matrix leg means these counts are missing that language, not that it has no findings.
    if [[ "$analysis" != "success" ]]; then
        echo "**The analysis reported \`${analysis}\`.** The counts below are incomplete."
        echo
    fi
    echo "**Open alerts on \`${branch}\`**"
    echo
    echo '| Severity | Open |'
    echo '|---|---|'
    for level in critical high medium low none; do
        echo "| ${level} | $(count "$level") |"
    done
    echo
    echo "[Security tab](${GITHUB_SERVER_URL:-https://github.com}/${repo}/security/code-scanning)"
} >>"$summary"
