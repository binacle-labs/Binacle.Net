#!/usr/bin/env bash
# Tag a commit and push the tag.
#   push-tag.sh <tag> <commit>
set -euo pipefail

tag="$1"
commit="$2"

# No user.name or user.email needed: with no -a or -m this is a lightweight tag, which is a ref and not an
# object, so git never asks who you are.
git tag "$tag" "$commit"

git push origin "$tag"
