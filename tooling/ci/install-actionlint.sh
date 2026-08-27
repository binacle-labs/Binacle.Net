#!/usr/bin/env bash
# Install the actionlint binary into ~/.local/bin, checked against its published SHA-256.
#   install-actionlint.sh
set -euo pipefail

# The only home for this version. The checksum is upstream's, in actionlint_<version>_checksums.txt.
version="1.7.12"
sha256="8aca8db96f1b94770f1b0d72b6dddcb1ebb8123cb3712530b08cc387b349a3d8"

asset="actionlint_${version}_linux_amd64"
url="https://github.com/rhysd/actionlint/releases/download/v${version}/${asset}.tar.gz"

tmp="${RUNNER_TEMP:-/tmp}"
mkdir -p "$HOME/.local/bin"

curl --proto '=https' --proto-redir '=https' -fsSL -o "$tmp/actionlint.tar.gz" "$url"
echo "${sha256}  $tmp/actionlint.tar.gz" | sha256sum -c -
tar xzf "$tmp/actionlint.tar.gz" -C "$tmp" actionlint
install -m0755 "$tmp/actionlint" "$HOME/.local/bin/actionlint"

# ~/.local/bin is on PATH for an interactive shell but not for a later workflow step, hence GITHUB_PATH.
# There is no such file on a laptop, where your own PATH already covers it.
echo "$HOME/.local/bin" >> "${GITHUB_PATH:-/dev/null}"

"$HOME/.local/bin/actionlint" --version
