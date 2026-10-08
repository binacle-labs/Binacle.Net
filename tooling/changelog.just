# Reads one version's section out of CHANGELOG.md.
#
#   just changelog extract 3.0.0      print that version's section
#   just changelog extract Unreleased print the in-progress section
#   just changelog check 3.0.0        exit 1 if the section is missing or empty
#
# The release workflow calls both: `check` as its first gate, `extract` to build the release body. Each
# recipe is a door onto the script of the same name beside this file.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The scripts print why they failed, so just's own "Recipe failed" line adds nothing. The exit code still
# gets through, so CI still fails.
set no-exit-message := true

changelog := "CHANGELOG.md"

# List the recipes
default:
    @just --list changelog

# Print one version's section, headings promoted to h2 [version|Unreleased]
extract version:
    @tooling/changelog.extract.sh {{ quote(version) }} {{ quote(changelog) }}

# Fail if a version's section is missing or empty [version|Unreleased]
check version:
    @tooling/changelog.check.sh {{ quote(version) }} {{ quote(changelog) }}
