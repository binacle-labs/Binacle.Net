#!/usr/bin/env bash
# Prints one version's section of the changelog, headings promoted so the shallowest is h2.
#   changelog.extract.sh <version|Unreleased> <changelog file>
#
# A section ends at the next version heading, not at the next `## ` - a promoted body carries its own. A
# version heading is a `## ` whose first word is `Unreleased` or a version number, brackets and a trailing
# ` - <date>` allowed. A heading inside a fenced code block is content and is left alone.
set -euo pipefail

want="$1"
file="$2"

awk -v want="$want" '
  function version_token(line,   h, a) {
    h = line; sub(/^## +/, "", h); gsub(/[][]/, "", h)
    split(h, a, / +/)
    if (a[1] == "Unreleased" || a[1] ~ /^[0-9]+\.[0-9]+\.[0-9]+/) return a[1]
    return ""
  }
  BEGIN { min = 99 }
  done { next }
  !f {
    if ($0 ~ /^## / && version_token($0) == want && want != "") f = 1
    next
  }
  {
    fence = ($0 ~ /^[ \t]*```/)
    if (!inside && !fence && $0 ~ /^## / && version_token($0) != "") { done = 1; next }
    if (fence) inside = !inside
    buf[++n] = $0
    if (!inside && !fence && match($0, /^#+ /)) {
      lev[n] = RLENGTH - 1
      if (lev[n] < min) min = lev[n]
    }
  }
  END {
    delta = (min == 99) ? 0 : 2 - min
    for (i = 1; i <= n; i++) {
      if (delta && lev[i]) {
        d = lev[i] + delta; if (d < 1) d = 1
        h = sprintf("%*s", d, ""); gsub(/ /, "#", h)
        print h substr(buf[i], lev[i] + 1)
      } else print buf[i]
    }
  }
' "$file"
