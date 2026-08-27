#!/usr/bin/env bash
# Create the GitHub release for a tag, or replace the body of one that already exists.
#   github-release.sh <tag> <changelog section> <commit>        needs GH_TOKEN
set -euo pipefail

tag="$1"
section="$2"
commit="$3"
notes="${RUNNER_TEMP:-/tmp}/notes.md"

# The flag is set in both directions, so a release marked prerelease by hand is corrected for a real tag.
case "$tag" in
    *-*) pre=true  ;;
    *)   pre=false ;;
esac

# `extract` promotes the section's headings back to h2 - in the file they sit at h3 under the version heading,
# and a release body has no such parent.
just changelog extract "$section" >"$notes"

# --target makes the tag too: `gh release create` creates a missing tag itself, on the commit named here, so
# the image and the tag cannot end up on different commits. An existing tag is used as it stands.
#
# Create OR edit: publishing a release from GitHub's web UI creates the tag and the release at once, so it may
# already be there and a plain `gh release create` would fail on it.
if gh release view "$tag" >/dev/null 2>&1; then
    echo "Release ${tag} already exists - replacing its body with the ${section} section."
    gh release edit "$tag" --title "$tag" --notes-file "$notes" --prerelease="$pre"
elif [[ "$pre" = true ]]; then
    echo "Creating release ${tag} from the ${section} section."
    gh release create "$tag" --target "$commit" --title "$tag" --notes-file "$notes" --prerelease
else
    echo "Creating release ${tag} from the ${section} section."
    gh release create "$tag" --target "$commit" --title "$tag" --notes-file "$notes"
fi
