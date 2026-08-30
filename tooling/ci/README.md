# CI

What the GitHub Actions workflows run and read. One script per operation, so every step in a workflow is a
named command you can also run in a terminal.

Nothing in here is written inside a `.just` file or inside a composite action, because neither can be run on
its own and neither can be handed to shellcheck. A `.sh` file is both.

## 📂 What is in it

| File | What it does |
|---|---|
| `changed-paths.sh` | Which halves of the repo a diff touched. Prints `code=yes\|no` and `site=yes\|no` |
| `gate.sh` | Passes only if every job in the json passed or was skipped. Writes the job table |
| `deploy-message.sh` | The label a deploy is filed under on the host. Prints `DEPLOY_MESSAGE=...` |
| `install-actionlint.sh` | Installs actionlint, pinned by version and SHA-256 |
| `install-container-structure-test.sh` | Installs container-structure-test, the same way |
| `install-hurl.sh` | Installs hurl, the same way |
| `install-lychee.sh` | Installs lychee, the same way |
| `push-tag.sh` | Tags a commit and pushes the tag - the three deploy markers; the release makes its own |
| `deploy-summary.sh` | The deploy's run summary: commit, marker tag, site |
| `sonar-summary.sh` | Waits for SonarCloud to finish, then writes its quality gate |
| `check-release-ref.sh` | Passes only if the release was dispatched on `main` |
| `check-version.sh` | Passes only if the version is semver shaped, with no leading `v` |
| `check-release-tag.sh` | Passes only if the tag is free, or already points at this commit |
| `changelog-section.sh` | Which `CHANGELOG.md` section a version publishes. Prints `name=...` |
| `moving-tags.sh` | Which public tags move, given the one that never does. Prints `moving=...` |
| `copy-tags.sh` | Copies one image to one or more tags by digest, then proves each reads back as it |
| `github-release.sh` | Creates the release, making the tag on the commit, or replaces the body of one that exists |
| `release-summary.sh` | The release's run summary: version, digest, public tags |
| `pull-image.sh` | Pulls a published image. Prints `digest=sha256:...` |
| `smoke-summary.sh` | The smoke's run summary: image, digest, each check |
| `codeql-summary.sh` | Counts the open code scanning alerts by severity |
| `dockerhub-version.sh` | Which version the Docker Hub page describes. Prints `name=...` |
| `sonar-analysis.xml` | Not a script - the SonarScanner settings. Scope, coverage paths and the exclusion lists, read by `sonar-analysis.yml` and by nothing else |

## 🚀 How you run one

From the repo root, through the `ci` module. `just --list ci` prints them all with their arguments.

```bash
just ci changed-paths HEAD~1 HEAD
just ci gate '{"changes":{"result":"success"},"image":{"result":"skipped"}}'
just ci deploy-summary "$(git rev-parse HEAD)" docs-42 https://docs.binacle.net
just ci check-release-tag v3.0.0 "$(git rev-parse HEAD)"
just ci moving-tags binacle/binacle-net:3.0.0 'binacle/binacle-net:3.0.0
binacle/binacle-net:latest'
```

**The four `install-*.sh` are the exception: no recipe, called by path.**

```bash
tooling/ci/install-lychee.sh
```

A composite action calls each one directly, because an action that installs a tool must not need another tool
installed first. They take no arguments: **the version and the checksum live in the script and nowhere else**,
and `DEVELOPMENT.md` tells a maintainer to run these rather than repeating the download.

## ⚠️ What will bite you

**Every script takes its inputs as arguments and reads no GitHub context of its own**, so all of them run on a
laptop. Keep it that way: a value from a workflow goes through `env:` and then into the command line.

**An `install-*.sh` writes to `$GITHUB_PATH`, falling back to `/dev/null`.** That is what puts the binary on
`PATH` for later steps; on a laptop there is no such file and your own `PATH` already covers `~/.local/bin`.

**A script that prints `key=value` is meant to be teed**, and its own chatter goes to stderr so it cannot end
up in `$GITHUB_OUTPUT`. The workflow step reads
`just ci changed-paths "$BASE" "$HEAD" | tee -a "$GITHUB_OUTPUT"`.

**A script that writes a run summary appends to `$GITHUB_STEP_SUMMARY`, falling back to the screen.** That
fallback is what makes it runnable here; do not drop it.

**They must pass `shellcheck` clean.** That is the whole reason they are files rather than recipe bodies, and
`just check scripts` is what enforces it - on the pull request and on a laptop.

`push-tag.sh` sets no git identity. With no `-a` or `-m` the tag is lightweight, which is a ref and not an
object, so git never asks who you are.

**`check-release-tag.sh` reaches origin.** It asks `git ls-remote` as well as the local clone, because the tag
it is guarding against is usually one only origin has.
