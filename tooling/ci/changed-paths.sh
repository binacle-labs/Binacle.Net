#!/usr/bin/env bash
# Which halves of the repo a diff touched. Prints `code=yes|no` and `site=yes|no`.
#   changed-paths.sh <base> <head>
set -euo pipefail

# Not code: a doc, an agent file, a site page, or a Jekyll plugin. None of them reach the image.
not_code='^(\.agents/|sites/|ruby/)|\.md$'

# Everything a site is built from. No .md carve-out - a markdown file under sites/ is a page.
site_input='^(sites/|ruby/|packages/|shared/|vipaq/|assets/|\.github/)'

# Three dots: with two, a commit landing on main reads as a change in every open pull request.
changed=$(git diff --name-only "$1...$2")

touched()              { local pattern="$1"; grep -qE  "$pattern" <<<"$changed"; }
touched_anything_but() { local pattern="$1"; grep -qvE "$pattern" <<<"$changed"; }

if touched_anything_but "$not_code";   then echo "code=yes"; else echo "code=no"; fi
if touched              "$site_input"; then echo "site=yes"; else echo "site=no"; fi
