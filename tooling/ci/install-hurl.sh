#!/usr/bin/env bash
# Install the hurl binary into ~/.local/bin, checked against its published SHA-256.
#   install-hurl.sh
set -euo pipefail

# The only home for this version. The checksum is upstream's, published beside the asset as <asset>.sha256.
version="8.0.1"
sha256="cac7c4670d69444db120edb21fe06c97ba8c80dcc52279957c8dd18f05fb0c06"

# gnu, because upstream publishes no musl build. This is why the smoke workflow pins ubuntu-24.04: the binary
# links libxml2.so.2, which Ubuntu 26.04 does not ship.
asset="hurl-${version}-x86_64-unknown-linux-gnu"
url="https://github.com/Orange-OpenSource/hurl/releases/download/${version}/${asset}.tar.gz"

tmp="${RUNNER_TEMP:-/tmp}"
mkdir -p "$HOME/.local/bin"

curl --proto '=https' --proto-redir '=https' -fsSL -o "$tmp/hurl.tar.gz" "$url"
echo "${sha256}  $tmp/hurl.tar.gz" | sha256sum -c -
tar xzf "$tmp/hurl.tar.gz" -C "$tmp"
install -m0755 "$tmp/${asset}/bin/hurl" "$HOME/.local/bin/hurl"

# ~/.local/bin is on PATH for an interactive shell but not for a later workflow step, hence GITHUB_PATH.
# There is no such file on a laptop, where your own PATH already covers it.
echo "$HOME/.local/bin" >> "${GITHUB_PATH:-/dev/null}"

"$HOME/.local/bin/hurl" --version
