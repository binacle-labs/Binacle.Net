#!/usr/bin/env bash
# Signed by this repo's release workflow, on the given ref.
#   verify-signature.sh <version> <repo> <signed_from>
set -uo pipefail

version="$1"
repo="$2"
signed_from="$3"

echo "== signature =="
if ! command -v cosign >/dev/null 2>&1; then
    echo "  cosign is not installed - DEVELOPMENT.md has the pinned install" >&2
    exit 1
fi
ref="${repo}:${version}"

# Anchored on purpose: without the $ this takes a signature from any branch, and pushing a branch is not a
# release.
from=$(printf '%s' "$signed_from" | sed 's/[].[^$*\\]/\\&/g')

# Never drop either flag. Without the identity this only asks whether anyone signed it, and anyone can.
if out=$(cosign verify "$ref" \
    --certificate-identity-regexp "^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@${from}\$" \
    --certificate-oidc-issuer https://token.actions.githubusercontent.com 2>&1); then
    printf '  %-52s signed by the release workflow, on %s\n' "$ref" "$signed_from"
else
    printf '  %-52s NOT VERIFIED\n' "$ref" >&2
    while IFS= read -r line; do printf '      %s\n' "$line"; done <<<"$out" >&2
    exit 1
fi
