# Rewrites the manifests under .agents/ - the five _index.md files.
#
#   just agents all                      rewrite all five
#   just agents generate-index plans     rewrite one
#
# Run it after adding, renaming, moving or re-describing any file under those directories.
#
# The generator is tooling/agents/generate-index.py and what it writes is tooling/agents/indexes.toml. Python
# because nothing in CI calls this - it is a laptop command, so it can use a real parser instead of awk.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The generator already printed why it failed, so just's own "Recipe failed" line adds nothing.
set no-exit-message := true

# List the recipes
default:
    @just --list agents

# Rewrite every manifest
all:
    @tooling/agents/generate-index.py

# Rewrite one manifest [rules|docs|design|plans|memory]
generate-index name:
    @tooling/agents/generate-index.py {{ name }}
