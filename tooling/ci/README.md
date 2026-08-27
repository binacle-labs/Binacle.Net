# CI

What the GitHub Actions workflows run and read. One script per operation, so every step in a workflow is a
named command you can also run in a terminal.

Nothing in here is written inside a `.just` file or inside a workflow, for two reasons: shellcheck cannot read
either, and neither can be run on its own. A `.sh` file is both.

## 📂 What is in it

| File | What it does |
|---|---|
| `changed-paths.sh` | Which halves of the repo a diff touched. Prints `code=yes\|no` and `site=yes\|no` |
| `gate.sh` | Passes only if every job in the json passed or was skipped. Writes the job table |
| `deploy-message.sh` | The label a deploy is filed under on the host. Prints `DEPLOY_MESSAGE=...` |
| `push-tag.sh` | Tags a commit and pushes the tag |
| `deploy-summary.sh` | The deploy's run summary: commit, marker tag, site |
| `sonar-summary.sh` | Waits for SonarCloud to finish, then writes its quality gate |
| `check-release-ref.sh` | Passes only if the release was dispatched on `main` |
| `check-version.sh` | Passes only if the version is semver shaped, with no leading `v` |
| `check-release-tag.sh` | Passes only if the tag is free, or already points at this commit |
| `changelog-section.sh` | Which `CHANGELOG.md` section a version publishes. Prints `name=...` |
| `github-release.sh` | Creates the release for a tag, or replaces the body of one that exists |
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
```

## ⚠️ What will bite you

**Every script takes its inputs as arguments and reads no GitHub context of its own**, so all of them run on a
laptop. Keep it that way: a value from a workflow goes through `env:` and then into the command line.

**A script that prints `key=value` is meant to be teed**, and its own chatter goes to stderr so it cannot end
up in `$GITHUB_OUTPUT`. The workflow step reads
`just ci changed-paths "$BASE" "$HEAD" | tee -a "$GITHUB_OUTPUT"`.

**A script that writes a run summary appends to `$GITHUB_STEP_SUMMARY`, falling back to the screen.** That
fallback is what makes it runnable here; do not drop it.

**They must pass `shellcheck` clean.** That is the whole reason they are files rather than recipe bodies.

`push-tag.sh` sets `user.name` and `user.email` on the repository it runs in, which is what a runner needs and
what you probably do not want on a laptop.

**`check-release-tag.sh` reaches origin.** It asks `git ls-remote` as well as the local clone, because the tag
it is guarding against is usually one only origin has.
