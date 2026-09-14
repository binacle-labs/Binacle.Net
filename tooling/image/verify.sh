#!/usr/bin/env bash
# Verify a published image: tags, signature, attestations, metadata. Each check is its own script beside
# this one; this runs the ones asked for and OR-s their exit codes.
#   verify.sh <version> [all|tags|signature|attestations|metadata] [signed_from] [repo]
#
# No `set -e`: every check runs and the codes are OR-ed at the end, so the first failure cannot hide the
# answers that explain it.
set -uo pipefail

version="${1:-}"
check="${2:-all}"
signed_from="${3:-refs/heads/main}"
repo="${4:-binacle/binacle-net}"
here="$(dirname "$0")"

if [ -z "$version" ]; then
    echo "A version is required: just image verify 3.0.0" >&2
    exit 1
fi
case "$check" in
    all) checks="tags signature attestations metadata" ;;
    tags|signature|attestations|metadata) checks="$check" ;;
    *)
        echo "Unknown check '${check}'." >&2
        echo "Use all, tags, signature, attestations or metadata." >&2
        exit 1
        ;;
esac

echo "Verifying binacle-net ${version}"
rc=0
failed=""
for c in $checks; do
    case "$c" in
        signature) "$here/verify-signature.sh" "$version" "$repo" "$signed_from" ;;
        *)         "$here/verify-$c.sh" "$version" "$repo" ;;
    esac || { rc=1; failed="$failed $c"; }
    echo
done
if [ "$rc" -eq 0 ]; then
    echo "PASS - everything checked came back clean."
else
    echo "FAIL -$failed" >&2
fi
exit $rc
