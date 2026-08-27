#!/usr/bin/env bash
# Install the container-structure-test binary into ~/.local/bin, checked against its published SHA-256.
#   install-container-structure-test.sh
set -euo pipefail

# The only home for this version. The checksum is upstream's, in checksums.txt.
version="v1.22.1"
sha256="fa35e89512a8978585f76cf41397956d2e3a30c62c2ad3fb857b1597074d14ca"

# A bare binary, not an archive - upstream ships one file per platform.
url="https://github.com/GoogleContainerTools/container-structure-test/releases/download/${version}/container-structure-test-linux-amd64"

tmp="${RUNNER_TEMP:-/tmp}"
mkdir -p "$HOME/.local/bin"

curl --proto '=https' --proto-redir '=https' -fsSL -o "$tmp/cst" "$url"
echo "${sha256}  $tmp/cst" | sha256sum -c -
install -m0755 "$tmp/cst" "$HOME/.local/bin/container-structure-test"

# ~/.local/bin is on PATH for an interactive shell but not for a later workflow step, hence GITHUB_PATH.
# There is no such file on a laptop, where your own PATH already covers it.
echo "$HOME/.local/bin" >> "${GITHUB_PATH:-/dev/null}"

"$HOME/.local/bin/container-structure-test" version
