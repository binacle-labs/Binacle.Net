#!/usr/bin/env bash
# Install the lychee binary into ~/.local/bin, checked against its published SHA-256.
#   install-lychee.sh
set -euo pipefail

# The only home for this version. The checksum is upstream's, published beside the asset as <asset>.sha256.
version="0.24.2"
sha256="73657a111819a30c47c08352896796f23d64e4eb2b3ed39b6d32149241566fc5"

# musl, not gnu: it links nothing from the system, so no distribution library can go missing.
asset="lychee-x86_64-unknown-linux-musl"
url="https://github.com/lycheeverse/lychee/releases/download/lychee-v${version}/${asset}.tar.gz"

tmp="${RUNNER_TEMP:-/tmp}"
mkdir -p "$HOME/.local/bin"

curl --proto '=https' --proto-redir '=https' -fsSL -o "$tmp/lychee.tar.gz" "$url"
echo "${sha256}  $tmp/lychee.tar.gz" | sha256sum -c -
tar xzf "$tmp/lychee.tar.gz" -C "$tmp"
install -m0755 "$tmp/${asset}/lychee" "$HOME/.local/bin/lychee"

# ~/.local/bin is on PATH for an interactive shell but not for a later workflow step, hence GITHUB_PATH.
# There is no such file on a laptop, where your own PATH already covers it.
echo "$HOME/.local/bin" >> "${GITHUB_PATH:-/dev/null}"

"$HOME/.local/bin/lychee" --version
