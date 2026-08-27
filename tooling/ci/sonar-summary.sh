#!/usr/bin/env bash
# Wait for SonarCloud to finish processing, then write its quality gate to the run summary.
#   sonar-summary.sh <path to report-task.txt> <commit> <branch>        needs SONAR_TOKEN
set -euo pipefail

: "${SONAR_TOKEN:?SONAR_TOKEN is not set}"
report="$1"
commit="$2"
branch="$3"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

# `sonar end` writes this file, and it is the only place the task and dashboard URLs appear.
ce_task_url=$(grep -m1 '^ceTaskUrl=' "$report" | cut -d= -f2-)
dashboard_url=$(grep -m1 '^dashboardUrl=' "$report" | cut -d= -f2-)

status=unknown

# 5 minutes. `end` returns as soon as the report is uploaded and SonarCloud processes it after, so reading the
# gate without this wait returns the PREVIOUS analysis. A task still queued after 5 minutes is an outage.
for _ in $(seq 1 60); do
    task=$(curl -sS -u "${SONAR_TOKEN}:" "$ce_task_url")
    status=$(printf '%s' "$task" | jq -r '.task.status')

    case "$status" in
        SUCCESS|FAILED|CANCELED) break ;;
    esac

    sleep 5
done

if [ "$status" != "SUCCESS" ]; then
    {
        echo '## Sonar'
        echo
        echo "The analysis task reported \`${status}\`. No gate to read."
        echo
        echo "[Dashboard](${dashboard_url})"
    } >>"$summary"
    exit 0
fi

analysis_id=$(printf '%s' "$task" | jq -r '.task.analysisId')
gate=$(curl -sS -u "${SONAR_TOKEN}:" \
    "https://sonarcloud.io/api/qualitygates/project_status?analysisId=${analysis_id}")

case "$(printf '%s' "$gate" | jq -r '.projectStatus.status')" in
    OK)    verdict='✅ Passed' ;;
    ERROR) verdict='❌ Failed' ;;
    *)     verdict="$(printf '%s' "$gate" | jq -r '.projectStatus.status')" ;;
esac

{
    echo '## Sonar'
    echo
    echo "**Quality gate:** ${verdict} - commit \`${commit:0:8}\` on \`${branch}\`"
    echo
    echo '| Condition | Value | Required | |'
    echo '|---|---|---|---|'
    # `comparator` is the FAILING direction, so it is inverted here to read as the requirement.
    printf '%s' "$gate" | jq -r '
      .projectStatus.conditions[]
      | (if .comparator == "LT" then ">=" elif .comparator == "GT" then "<=" else .comparator end) as $req
      | "| `\(.metricKey)` | \(.actualValue) | \($req) \(.errorThreshold) | \(if .status == "OK" then "✅" else "❌" end) |"'
    echo
    echo "[Open the dashboard](${dashboard_url})"
} >>"$summary"
