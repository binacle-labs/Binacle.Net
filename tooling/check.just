# Checks. Each one reads a real file and says what is wrong with it.
#
#   just check links                   the internal links in all three built sites
#   just check links <site>            one site
#   just check links-external <site>   the same site, other people's servers included
#   just check workflows               actionlint over .github/workflows
#   just check actions                 the .github/actions files, which actionlint cannot read
#   just check scripts                 shellcheck over the scripts in tooling/ci
#
# `links` reads the built site and never the source, because a href in the source is Liquid and not a URL
# yet. Build first.
#
# `workflows`, `actions` and `scripts` print the files they were handed and count them at the end. All three
# say nothing at all when they pass, so the count is what tells a clean run from one that never started.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# lychee prints every broken link with its file and line, so just's own "Recipe failed" line adds nothing.
# The exit code still gets through, so CI still fails.
set no-exit-message := true

# lychee only finds a config by itself when it sits in the working directory, and this one does not, so it
# is named. Absolute, because the recipes run from the repo root and this file does not move with them.
lychee_config := justfile_directory() / "tooling" / "check.lychee.toml"

# List the recipes
default:
    @just --list check

# Internal links in the built sites - all three with no argument, or one [site]
links site="":
    #!/usr/bin/env bash
    set -euo pipefail

    sites="{{ site }}"
    [ -n "$sites" ] || sites="docs demo www"

    for s in $sites; do
        just check _lychee "$s" --offline
    done

# Every link in one site, other people's servers included - slower, and it can fail on somebody else's outage
links-external site: (_lychee site)

# The workflow files: expressions, shell quoting, runner labels, action inputs
workflows:
    #!/usr/bin/env bash
    set -euo pipefail

    files=(.github/workflows/*.yml)

    echo "checking ${#files[@]} workflows in .github/workflows"
    printf '  %s\n' "${files[@]##*/}"

    # actionlint prints its own errors and exits non-zero, so the line below is only reached on a clean run.
    actionlint -color "${files[@]}"

    echo "${#files[@]} workflows, no errors"

# The .github/actions files, which actionlint cannot read
actions:
    #!/usr/bin/env bash
    set -euo pipefail

    files=(.github/actions/*/action.yml)
    names=("${files[@]%/action.yml}")

    echo "checking ${#files[@]} actions in .github/actions"
    printf '  %s\n' "${names[@]##*/}"

    # By hand, because actionlint reads every file as a workflow. A vars or secrets expression anywhere in
    # one of these files, descriptions included, makes the action fail to load on the runner.
    #
    # grep prints the file, the line and the text. The line below says what to do about it.
    if grep -nE --color=auto '\$\{\{[^}]*\b(vars|secrets)\.' "${files[@]}"; then
        echo "take it as an input instead - neither context exists inside an action" >&2
        exit 1
    fi

    echo "${#files[@]} actions, no errors"

# The shell a workflow runs - the scripts in tooling/ci
scripts:
    #!/usr/bin/env bash
    set -euo pipefail

    files=(tooling/ci/*.sh)

    echo "checking ${#files[@]} scripts in tooling/ci"
    printf '  %s\n' "${files[@]##*/}"

    # shellcheck prints its own errors and exits non-zero, so the line below is only reached on a clean run.
    shellcheck "${files[@]}"

    echo "${#files[@]} scripts, no errors"

[private]
_lychee site *flags:
    #!/usr/bin/env bash
    set -euo pipefail

    dir="artifacts/{{ site }}"
    if [ ! -d "$dir" ]; then
        echo "No $dir. Run 'just build {{ site }}' first - this reads the built site." >&2
        exit 1
    fi

    # --root-dir must be absolute, or a href like /core-concepts/ resolves against the filesystem root
    # instead of the site.
    lychee --config '{{ lychee_config }}' {{ flags }} --root-dir "$PWD/$dir" --no-progress "$dir/**/*.html"
