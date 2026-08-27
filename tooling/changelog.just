# Reads one version's section out of CHANGELOG.md.
#
#   just changelog extract 3.0.0      print that version's section
#   just changelog extract Unreleased print the in-progress section
#   just changelog check 3.0.0        exit 1 if the section is missing or empty
#
# The release workflow calls both: `check` as its first gate, `extract` to build the release body.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# awk prints why it failed, so just's own "Recipe failed" line adds nothing. The exit code still gets
# through, so CI still fails.
set no-exit-message := true

changelog := "CHANGELOG.md"

# List the recipes
default:
    @just --list changelog

# Print one version's section, headings promoted to h2 [version|Unreleased]
extract version:
    #!/usr/bin/env bash
    set -euo pipefail

    # The awk below does two things.
    #
    # It finds the section. A section ends at the next VERSION heading, not at the next `## ` - a promoted
    # body carries its own `## `. So a terminator is a `## ` whose first word reads as `Unreleased` or as a
    # version number. `## 3.0.0`, `## [3.0.0]` and either with a trailing ` - <date>` all count.
    #
    # Then it promotes the headings. In the file a release sits at `##` under the one `# Changelog`; on its
    # own a release body starts at `##`. Every heading shifts by whatever brings the shallowest one back to
    # `##`, which keeps the relative depth inside the body.
    #
    # A heading inside a fenced code block is left alone - it is content, not structure.
    awk -v want='{{ version }}' '
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
    ' {{ changelog }}

# Fail if a version's section is missing or empty [version|Unreleased]
check version:
    #!/usr/bin/env bash
    set -euo pipefail

    notes="$(just changelog::extract '{{ version }}')"

    if [ -z "${notes//[[:space:]]/}" ]; then
        echo "No '## {{ version }}' section in {{ changelog }}." >&2
        echo "Rename [Unreleased] to {{ version }} before tagging." >&2
        exit 1
    fi

    lines=$(printf '%s\n' "$notes" | grep -c '' || true)
    echo "'## {{ version }}' found - ${lines} lines."
