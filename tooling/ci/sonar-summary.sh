#!/usr/bin/env bash
# Write the SonarCloud quality gate to the run summary.
#   sonar-summary.sh <path to report-task.txt> <commit> <branch>        needs SONAR_TOKEN
set -euo pipefail

: "${SONAR_TOKEN:?SONAR_TOKEN is not set}"
report="$1"
commit="$2"
branch="$3"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

# `sonar end` writes this file. The step runs even when `end` failed, so a missing file means the scan never
# reached the upload.
if [[ ! -f "$report" ]]; then
    {
        echo '## Sonar'
        echo
        echo 'No analysis was uploaded. No gate to read.'
    } >>"$summary"
    exit 0
fi

ce_task_url=$(grep -m1 '^ceTaskUrl=' "$report" | cut -d= -f2-)
dashboard_url=$(grep -m1 '^dashboardUrl=' "$report" | cut -d= -f2-)

# One read, no wait: sonar.qualitygate.wait in sonar-analysis.xml makes `end` block until processing is done.
task=$(curl -sS -u "${SONAR_TOKEN}:" "$ce_task_url")
status=$(printf '%s' "$task" | jq -r '.task.status')

if [[ "$status" != "SUCCESS" ]]; then
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
