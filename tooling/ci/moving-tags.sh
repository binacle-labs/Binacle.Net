#!/usr/bin/env bash
# Which of the public tags move, given the one that never does. Prints moving=<space separated>.
#   moving-tags.sh <immutable tag> '<one tag per line>'
#
# The filter is here so the version tag is written exactly once. Copying the whole list twice would be
# simpler and works today, but a frozen version tag would reject the second write.
set -euo pipefail

immutable="$1"

moving=()
found=0
while IFS= read -r tag; do
    [[ -n "$tag" ]] || continue
    if [[ "$tag" == "$immutable" ]]; then
        found=1
        continue
    fi
    moving+=("$tag")
done <<<"$2"

# Not a signing check - the signature is on the digest, so every tag points at something signed whatever
# this does. It catches build's version and metadata-action's disagreeing, which would publish two version
# tags instead of one.
if [[ "$found" -ne 1 ]]; then
    echo "'${immutable}' is not in the tag list:" >&2
    printf '  %s\n' "$2" >&2
    exit 1
fi

echo "moving=${moving[*]:-}"
