#!/usr/bin/env bash
# The label a deploy is filed under on the host. Prints `DEPLOY_MESSAGE=<short sha> - <subject>`.
#   deploy-message.sh <commit>
set -euo pipefail

commit="$1"

# Read from git, because a workflow_dispatch run carries no commit payload. The four characters stripped are
# the ones that would break out of the quoted argument the host is handed.
#
# $'...' not '...': tr needs the backslash escaped, and plain quotes trip shellcheck SC1003.
subject=$(git log -1 --format=%s "$commit" | tr -d $'"`$\\\\' | cut -c1-90)

echo "DEPLOY_MESSAGE=${commit:0:8} - ${subject}"
