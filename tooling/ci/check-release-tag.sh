#!/usr/bin/env bash
# The release tag is free, or already points at this commit.
#   check-release-tag.sh <tag> <commit>
set -euo pipefail

tag="$1"
commit="$2"

local_sha="$(git rev-parse -q --verify "refs/tags/${tag}^{commit}" || true)"

# An annotated tag answers with the tag object and then the commit on the `^{}` line, so the last one wins.
remote_sha="$(git ls-remote origin "refs/tags/${tag}" "refs/tags/${tag}^{}" | awk '{ sha = $1 } END { print sha }')"

for sha in "$local_sha" "$remote_sha"; do
    [[ -n "$sha" ]] || continue
    if [[ "$sha" != "$commit" ]]; then
        echo "Tag ${tag} already exists, on ${sha}, and this run is on ${commit}." >&2
        echo "Release the next version instead. Do not delete and re-push a tag anyone has pulled." >&2
        exit 1
    fi
done

# Not an error: it is what lets a run that published and then failed be dispatched again.
if [[ -n "${local_sha}${remote_sha}" ]]; then
    echo "Tag ${tag} is already on ${commit} - this run is a retry."
else
    echo "Tag ${tag} is free."
fi
