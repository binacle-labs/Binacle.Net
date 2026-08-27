#!/usr/bin/env bash
# The run summary for a smoke: the image, the digest its tag resolved to, and each check.
#   smoke-summary.sh <image> <digest> '<check:result, one per line>'
set -euo pipefail

image="$1"
digest="$2"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

{
    echo "## Smoked \`${image}\`"
    echo
    if [ -n "$digest" ]; then
        echo "**Digest** \`${digest}\` - what the tag pointed at when this ran."
    else
        echo '**The pull failed.** Nothing was smoked.'
    fi
    echo
    echo '| Check | Result |'
    echo '|---|---|'
    while read -r pair; do
        [ -n "$pair" ] || continue

        case "${pair##*:}" in
            success) mark='✅' ;;
            skipped) mark='-' ;;
            *)       mark='❌' ;;
        esac

        echo "| ${pair%%:*} | ${mark} |"
    done <<<"$3"
} >>"$summary"
