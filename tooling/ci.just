# The shell a workflow runs, loaded by the root justfile as the `ci` module.
#
# Every recipe is a door onto one file in tooling/ci/. Nothing is written in here, because a `.just` body is
# invisible to shellcheck and cannot be run on its own - a real script is both.
#
# Each script takes its inputs as arguments and reads no GitHub context of its own, so every one of them runs
# on a laptop. The ones that print a `key=value` line are meant to be teed into $GITHUB_OUTPUT or $GITHUB_ENV;
# the ones that write a run summary fall back to the screen.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The scripts print why they failed, so just's own "Recipe failed" line adds nothing.
set no-exit-message := true

# quote() hands a value to the shell as one argument whatever is in it - these carry json, newlines and quotes.

# List the recipes
default:
    @just --list ci

# Which halves of the repo a diff touched - prints code=yes|no and site=yes|no
changed-paths base head:
    @tooling/ci/changed-paths.sh {{ quote(base) }} {{ quote(head) }}

# Pass only if every job in the json passed or was skipped
gate results:
    @tooling/ci/gate.sh {{ quote(results) }}

# The label a deploy is filed under - prints DEPLOY_MESSAGE=<short sha> - <subject>
deploy-message commit:
    @tooling/ci/deploy-message.sh {{ quote(commit) }}

# Tag a commit and push the tag
push-tag tag commit:
    @tooling/ci/push-tag.sh {{ quote(tag) }} {{ quote(commit) }}

# Write the deploy's run summary: commit, marker tag, site
deploy-summary commit tag url:
    @tooling/ci/deploy-summary.sh {{ quote(commit) }} {{ quote(tag) }} {{ quote(url) }}

# Wait for SonarCloud, then write its quality gate to the run summary
sonar-summary report commit branch:
    @tooling/ci/sonar-summary.sh {{ quote(report) }} {{ quote(commit) }} {{ quote(branch) }}

# Only main may release
check-release-ref ref:
    @tooling/ci/check-release-ref.sh {{ quote(ref) }}

# The version is semver shaped, with no leading v
check-version version:
    @tooling/ci/check-version.sh {{ quote(version) }}

# The release tag is free, or already on this commit
check-release-tag tag commit:
    @tooling/ci/check-release-tag.sh {{ quote(tag) }} {{ quote(commit) }}

# Which CHANGELOG section a version publishes - prints name=<section>
changelog-section version:
    @tooling/ci/changelog-section.sh {{ quote(version) }}

# Which public tags move, given the one that never does - prints moving=<space separated>
moving-tags immutable tags:
    @tooling/ci/moving-tags.sh {{ quote(immutable) }} {{ quote(tags) }}

# Copy one image to one or more tags by digest, then prove every tag reads back as it
copy-tags source tags:
    @tooling/ci/copy-tags.sh {{ quote(source) }} {{ quote(tags) }}

# Create the release for a tag, making the tag on the commit, or replace the body of one that exists
github-release tag section commit:
    @tooling/ci/github-release.sh {{ quote(tag) }} {{ quote(section) }} {{ quote(commit) }}

# Write the release's run summary: version, digest, public tags
release-summary version digest tag url tags:
    @tooling/ci/release-summary.sh {{ quote(version) }} {{ quote(digest) }} {{ quote(tag) }} {{ quote(url) }} {{ quote(tags) }}

# Pull a published image - prints digest=sha256:...
pull-image image:
    @tooling/ci/pull-image.sh {{ quote(image) }}

# Write the smoke's run summary: image, digest, each check
smoke-summary image digest results:
    @tooling/ci/smoke-summary.sh {{ quote(image) }} {{ quote(digest) }} {{ quote(results) }}

# Count the open code scanning alerts and write them to the run summary
codeql-summary repo ref branch analysis:
    @tooling/ci/codeql-summary.sh {{ quote(repo) }} {{ quote(ref) }} {{ quote(branch) }} {{ quote(analysis) }}

# Which version the Docker Hub page describes - prints name=<version>
dockerhub-version version="":
    @tooling/ci/dockerhub-version.sh {{ quote(version) }}
