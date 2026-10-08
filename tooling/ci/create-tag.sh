#!/usr/bin/env bash
# Tag a commit through the API. No git push, so no job needs the checkout's credential.
#   create-tag.sh <tag> <commit>        needs GH_TOKEN and GITHUB_REPOSITORY
set -euo pipefail

tag="$1"
commit="$2"

gh api --method POST "repos/${GITHUB_REPOSITORY:?}/git/refs" -f ref="refs/tags/${tag}" -f sha="${commit}"
