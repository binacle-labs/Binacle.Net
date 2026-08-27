#!/usr/bin/env bash
# Tag a commit and push the tag.
#   push-tag.sh <tag> <commit>
set -euo pipefail

tag="$1"
commit="$2"

git config user.name "github-actions[bot]"
git config user.email "github-actions[bot]@users.noreply.github.com"
git tag "$tag" "$commit"

git push origin "$tag"
