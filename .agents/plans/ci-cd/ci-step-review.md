---
description: A read-only sweep of everything CI does, asking for each thing whether an official or first-party mechanism already does it - twelve findings, the biggest being Docker Hub's OIDC login, gh release create making its own tag, and the fact that nothing in CI runs shellcheck
state: blocked
waits-on: "the maintainer - findings 2, 3, 6, 9, 11 and the shellcheck gap are done; the rest are each a separate yes or no. State chosen by an agent, it was `in-progress` and that is not one of the five - strike it if wrong"
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
  - "tooling/ci/**"
---

# Is there a better way to do what CI does

Read on 2026-08-28: the eleven workflows in `.github/workflows/`, the nine composite actions in
`.github/actions/`, the sixteen scripts in `tooling/ci/` and `tooling/ci.just`, plus `tooling/check.just`,
`tooling/build.just` and `.github/dependabot.yml` where a workflow step reaches into them.

The question asked of every step was not "is there a different action for this" but "does the platform already
do this, and can the work be deleted rather than swapped". Twelve findings. Each names its benefit; where the
answer is that what we have is better, that is written down too, in **What to keep** near the end, so the same
files are not reviewed again.

## 1. Docker Hub can be logged into with the run's own OIDC token, so the stored registry token goes

**Now:** the release `publish` job and the Docker Hub page workflow both log in with `DOCKERHUB_USERNAME` and
`DOCKERHUB_TOKEN`, a long-lived personal access token stored as a repository secret.

**The supported way:** Docker Hub OIDC connections for GitHub Actions, announced by Docker on 2026-07-21 and
supported directly by `docker/login-action` - the action already pinned here, at a version new enough
(v4.6.0; the feature landed in v4.5.0). The login step keeps `username:` and **drops `password:` entirely**,
gaining one environment variable:

```yaml
- name: Login to Docker Hub
  uses: docker/login-action@<pin>
  env:
    DOCKERHUB_OIDC_CONNECTIONID: ${{ vars.DOCKERHUB_OIDC_CONNECTIONID }}
  with:
    username: ${{ vars.DOCKERHUB_ORGNAME }}
```

The job needs `id-token: write`, which `publish` already declares for cosign. Source: the
`docker/login-action` README's Docker Hub OIDC section, and Docker's blog post "Docker OIDC Connections for
GitHub Actions available for Docker Orgs".

**Benefit:** the credential that can push to the registry users pull from stops existing between runs. It is
minted per run and expires with it - the same property that made GHCR the staging registry in the first place,
applied to the registry that actually matters. Nothing to rotate, and nothing to leak from a repository
setting.

**Cost and risk.** Two real ones, both must be checked before this is worth doing:

- **Eligibility.** Docker states OIDC connections are for Docker Team, Business, Hardened Images, or an org
  enrolled in the Docker Sponsored Open Source Program. **I could not check which plan the `binacle` Docker
  Hub org is on** - that is a login-only fact. If it is a free org this finding is dead until the plan
  changes.
- **It does not close the secret completely.** `shared-dockerhub-overview.yml` writes the repository
  description through the Docker Hub **web API**, not the registry, using `peter-evans/dockerhub-description`,
  which takes a username and password. An OIDC registry login does nothing for it. So the secret survives for
  the page job; what goes away is the credential in the job that moves the artifact.

**Confidence:** high on the mechanism and the action version, unknown on eligibility.

## 2. `gh release create` makes the tag itself, so the release job's tag step can be deleted

**Now:** the `release` job runs `just ci push-tag "$TAG" "$COMMIT"` and then `just ci github-release`, in that
order, and the ordering is deliberate - the window in which the image is published and the tag does not exist
is kept one step wide.

**The supported way:** `gh release create` already creates the tag. Its manual says "If a matching git tag does
not yet exist, one will automatically get created from the latest state of the default branch", and "Use
`--target` to point to a different branch or commit for the automatic tag creation". `--target` takes a commit
sha, which is exactly what the job has in `github.sha`.

**What it becomes:** `github-release.sh` takes a third argument, the commit, and its two create branches gain
`--target "$commit"`. The "Push the release tag" step is deleted from the workflow.

**Benefit:** the window closes completely rather than narrowing. The tag and the release are made in one API
call, so the failure mode the release ordering was designed around - image published, tag missing - stops
being possible at all rather than being one step wide. It also deletes a step, an argument and a call into a
script that does something else entirely.

**What it does not break.** The retry path is preserved: if the tag already exists on this run's commit (the
gate allows that on purpose, so a run that published and then failed can be dispatched again), `gh release
create` uses the existing tag and ignores `--target`. The create-or-edit branch for a tag made by hand in
GitHub's web UI is untouched.

**Cost:** the tag is then created by the Releases API rather than by `git push`, so it is not visible in the
job's git output. The run log gets the release URL instead, which is the thing a reader actually follows.

**Confidence:** high.

## 3. Nothing in CI runs shellcheck over `tooling/ci/*.sh`

**Now:** 285 lines of shell were moved out of the workflows into `tooling/ci/` because a `.just` body and a
`run:` block cannot be handed to shellcheck and a `.sh` file can. That is the stated reason those files exist.
**No workflow runs shellcheck on them.** The `workflows` job installs actionlint and runs `just check
workflows` and `just check actions`; neither touches `tooling/ci/`. Checked by grep across `.github/` and
`tooling/`: the only mentions of shellcheck in the repository outside `.agents/` are comments and
`DEVELOPMENT.md`.

**The supported way:** shellcheck ships on the GitHub-hosted Ubuntu runners - `DEVELOPMENT.md` already says so
- and actionlint already invokes it for inline `run:` blocks. So this is one step in the `workflows` job with
nothing to install:

```yaml
- name: Check - scripts
  if: ${{ !cancelled() }}
  run: shellcheck tooling/ci/*.sh
```

It would sit better behind a `just check scripts` recipe, to match the two beside it.

**Benefit:** the reason those files are files becomes a fact rather than a habit. A quoting mistake in
`github-release.sh` or `check-release-tag.sh` is caught on the pull request instead of during a release, which
is the one run where a shell bug is most expensive. It costs one step and about two seconds.

**Confidence:** high. The gap is certain; only the exact recipe shape is a choice.

## 4. The identity lines in `push-tag.sh` do nothing, and the whole script has a first-party replacement

**Now:**

```bash
git config user.name "github-actions[bot]"
git config user.email "github-actions[bot]@users.noreply.github.com"
git tag "$tag" "$commit"
git push origin "$tag"
```

**The two `git config` lines are dead.** `git tag <name> <commit>` with no `-a`, `-m` or `-s` creates a
**lightweight** tag, which is a ref and not an object, so git never asks who you are. Proved locally on
2026-08-28 in a throwaway repository with `HOME`, `GIT_CONFIG_GLOBAL` and `GIT_CONFIG_SYSTEM` all pointed at
nothing: the lightweight tag was created, and `git tag -a` in the same shell failed with "Committer identity
unknown". So the two lines can be deleted today with no other change.

**And there is a supported way to skip git entirely.** A tag is a ref, and the REST API creates one:

```bash
gh api --method POST "repos/${repo}/git/refs" -f ref="refs/tags/${tag}" -f sha="${commit}"
```

With finding 2 taking the release's tag, the three deploy marker tags are the only callers left, and this
would replace the last `git push` in the repository.

**Benefit, and it is the one worth having:** no job would then need the git credential that
`actions/checkout` writes into `.git/config`. `persist-credentials: true` is checkout's default, and GitHub's
own hardening guidance is to turn it off where a job does not push. With no pushes left, every checkout in
every workflow can carry `persist-credentials: false` uniformly - so a compromised step in any job cannot read
a working token out of the checkout. Today two jobs genuinely need it and thirteen carry it for nothing.

**Cost:** fifteen `persist-credentials: false` lines, and a failure message from the API that reads worse than
git's ("Reference already exists" rather than "tag already exists"). The deploy `tag` job still checks out,
because `deploy-summary.sh` reads the commit subject with `git log`.

**Confidence:** high on the dead config lines - they are proved. High on the API working; medium on whether
fifteen added lines are worth the hardening, which is the maintainer's call.

## 5. The three deploy workflows are one workflow with three sets of five values

**Now:** `deploy-docs-site.yml`, `deploy-demo-site.yml` and `deploy-www-site.yml` are 80 lines each and
identical except for five values - the site name, its directory, the environment name, the URL and the
wrangler config path. 240 lines, three copies of the same three-job chain, three places to edit when the host
changes or a step is added.

**The supported way:** a reusable workflow, `shared-deploy-site.yml`, taking those five as inputs. A reusable
workflow brings its own job, so unlike a composite action it **can** set `runs-on`, `permissions`,
`timeout-minutes` and `environment:` - and `environment.name` and `environment.url` both accept expressions,
so the deployment URL still shows in the Actions UI. Each of the three files stays a real workflow with its own
`workflow_dispatch` and its own concurrency group, and shrinks to about fifteen lines.

**Benefit:** a change to how a site is deployed is one edit instead of three, and three files cannot drift from
each other. That drift is not hypothetical - two of the three environments have never been deployed, so two of
the three copies have never run.

**Cost, and it is a real argument against.** The current reason the deploy step is in the workflow rather than
inside an action is that the host is named where you would look for it, and `contents: write` sits next to the
tag push it allows. A reusable workflow moves both into a fourth file. The counter is that a reusable workflow
is a workflow - it appears in the Actions list, its jobs show in the run graph, and `permissions:` are still
written in it. That is not the same as burying a step inside something called "deploy site".

The site-deploys-are-manual rule is untouched: the three callers keep `workflow_dispatch` and nothing else.

**Confidence:** high that it works; medium on whether the maintainer wants it. This is a judgement about where
he wants to read the host name, not a technical question.

## 6. `container-structure-test` publishes checksums now, and the pin's comment says it does not

**Now:** `.github/actions/install-container-structure-test` carries

```
# Upstream publishes none. Taken from the binary in use, so it proves later downloads match that
# one, not that it was genuine.
SHA256: fa35e89512a8978585f76cf41397956d2e3a30c62c2ad3fb857b1597074d14ca
```

**That is out of date.** The v1.22.1 release publishes `checksums.txt`, and its line for
`container-structure-test-linux-amd64` is `fa35e895...d14ca` - the same value, byte for byte. Fetched from the
release on 2026-08-28.

**What it becomes:** the same pin, with the comment corrected to say where the value came from - "Published by
upstream in checksums.txt", matching the wording the actionlint and lychee actions already use.

**Benefit:** the checksum stops being self-confirming and starts being upstream's. It also removes the one
place in the four install actions where the reader is told the weaker guarantee applies, when it no longer
does. Two lines of comment; no behaviour change.

**Confidence:** high. Both halves were fetched and compared.

## 7. An environment with a branch policy would scope the Docker Hub credential to `main`

**Now:** `DOCKERHUB_TOKEN` is a repository secret. Any workflow run on any branch in this repository can read
it. The release is kept to `main` by `check-release-ref.sh`, which is a check inside the run.

**The supported way:** put the `publish` job (and the page workflow's job) in a GitHub **environment** whose
deployment branch policy allows `main` only, and move the two Docker Hub values to environment secrets. GitHub
refuses to start a job in that environment on any other ref, and the secrets are not readable outside it. The
three deploy workflows already use environments, so the shape is in the repo.

**Benefit:** it stops being possible to read the Docker Hub credential from a branch, rather than being
checked. That is a different class of guarantee from a step that runs inside the job.

**This does not replace `check-release-ref.sh`, and it should not.** The gate check fails in seconds with a
sentence a human wrote, before anything is built; the environment refusal happens much later and reads like a
configuration error. Keep both. The benefit here is the secret scoping alone.

**Cost:** where the credential lives moves into GitHub settings, which a reader of the repository cannot see -
the same objection that removed three repo variables. It is weaker here, because a secret is invisible either
way.

**Confidence:** medium-high on the mechanism, high that the benefit is real.

## 8. The `publish` job probably does not need `docker/setup-buildx-action`

**Now:** the `publish` job runs `docker/setup-buildx-action` before `docker buildx imagetools create`.

**Why it may be unnecessary:** the GitHub-hosted Ubuntu runner ships Docker Buildx as a plugin - version
0.36.1 on the 24.04 image, per the runner-images README. And `imagetools create` is a registry operation, not a
build: Docker's reference says it creates a manifest list from sources that "must already exist in the
registry". It needs no builder container.

**Benefit:** one fewer third-party action executing inside the one job that holds the Docker Hub credential,
and about fifteen seconds off every release. Fewer moving parts in the job where a compromise would be worst.

**The `build` job keeps its `setup-buildx-action`** - that one builds with `provenance: mode=max` and
`sbom: true`, which need the container driver the action sets up. This finding is about `publish` only.

**Cost and risk:** if `imagetools` turns out to want a configured builder for registry auth, the release fails
at the copy step - late, and on a real release. **I could not run docker to prove it.** The safe way to take
this is to remove it on the next prerelease dispatch, where a failure costs a re-dispatch.

**Confidence:** medium. The reading is clear; the proof is one run away and I did not have it.

## 9. A step that prints `just --version` and nothing reads it

**Now:** `shared-smoke-image.yml` has a step whose whole body is `just --version`, between the two install
actions and the GHCR login.

**Why it is there:** the four `install-*` actions each end by printing the version they installed, so a red
step names the tool. `setup-just` does not, so one caller added a bare step to get the same line - and only one
of the sixteen jobs that install just has it.

**What it becomes:** delete the step, and add a `just --version` line to the end of `.github/actions/setup-just`
so every caller gets it for free, the way the install actions already work.

**Benefit:** the convention applies in sixteen jobs instead of one, and a workflow loses a step that says
nothing about that workflow.

**Confidence:** high.

## 10. Sonar's five-minute poll loop is one scanner property

**Now:** `sonar-summary.sh` polls `ceTaskUrl` up to sixty times, five seconds apart, waiting for SonarCloud to
finish processing, because `Sonar end` returns when the upload finishes rather than when the analysis is done.
About twenty lines.

**The supported way:** `/d:sonar.qualitygate.wait=true` on the `begin` command. The scanner then blocks at
`end` until the analysis is processed, and exits non-zero if the gate fails. The summary script keeps its two
API calls but drops the loop: one request to `ceTaskUrl` returns `SUCCESS` straight away.

**Benefit:** twenty lines of retry logic replaced by one flag on a command that is already there, and the wait
becomes the tool's problem rather than a timeout we chose.

**Cost, and it is the reason this is not an obvious yes:** `wait=true` also **fails the step** on a red gate.
Coverage is deliberately not blocking anywhere yet - the read-only gate asks 80% on new code and the project is
below it, so this would paint the Sonar run red every time it ran. The summary step would need `if: always()`
to still write its table. Whether a red run that blocks nothing is honest or is noise is the maintainer's
call, and it is the whole decision here.

**Confidence:** high on the mechanism, and the decision is his, not mine.

## 11. The four `install-*` actions are one script written four times

**Now:** four composite actions, each about twenty lines of `mkdir`, `curl`, `sha256sum -c`, `tar`, `install`,
`GITHUB_PATH`, version print. They differ in the URL, the checksum, and where the binary sits inside the
archive - a bare binary for container-structure-test, a nested `bin/` for hurl, a folder for lychee, the root
for actionlint. This shell is the only shell in CI that nothing lints.

**No official action replaces any of the four**, and I checked each on its own:

- **hurl** - no action, and see the runner-pin note under **Already decided** for why the musl trick used for
  lychee is not available.
- **container-structure-test** - no action. Google publishes binaries and, as of finding 6, checksums.
- **actionlint** - upstream ships a `download-actionlint.bash` and a Docker action; the download script is the
  route upstream recommends, and it is what this action already does by hand.
- **lychee** - has an action, deliberately not used, and the reason still holds. See **Already decided**.

**So the finding is a shape, not a swap:** one script under `tooling/ci/`, taking the tool name, URL, checksum
and archive path as arguments, called from four thin actions that keep their version and checksum in `env:`
where a reader and Dependabot look. That is the direction the workflow-restructure work already settled on
(a local action is read out of the working copy, so it can call a script in the repo); this review confirms it
and adds that nothing official replaces any of the four, so the script is worth writing.

**Benefit:** the unlinted shell in CI goes to roughly zero, and finding 3's `shellcheck tooling/ci/*.sh` then
covers it automatically. Four copies of a download-and-verify become one.

**Cost:** an argument list is less readable than four flat scripts, and the "one action per tool so a red step
names the tool" property has to be kept by naming the caller's step, not the script.

**Confidence:** high that it works. This overlaps with work already planned elsewhere and is listed here only
because the investigation it was waiting on is the one above.

## 12. A workflow edit builds all three Jekyll sites

**Now:** `changed-paths.sh` sets `site=yes` for anything matching `^\.github/`. So editing a workflow, or a
markdown file under `.github/`, runs the site test suite and builds all three sites with their link checks.

**What it could be:** narrow that alternative to `^\.github/actions/`, which is the only part of `.github/` a
site build actually depends on - `build-jekyll-site`.

**Benefit:** a workflow-only pull request stops paying for three Jekyll builds and a fifteen-test suite.

**Cost, and it is why this is last:** a change to `shared-site-tests.yml` itself would then not run the site
tests it changes. That is a real hole, and it may be worth the current breadth. Both halves of the filter are
deliberately generous today and the reasoning for that is written down; this is a small saving against a small
risk, and either answer is defensible.

**Confidence:** medium on the mechanics, low that it is worth doing.

## What to keep as it is

Each of these was checked against an official or first-party alternative and the alternative is worse. Listed
so the next session does not check them again.

- **`changed-paths.sh` and its full-history checkout.** The API route is `repos/{repo}/compare/{base}...{head}`,
  and its `basehead` really is the three-dot merge-base comparison. But **the file list caps at 300 and does
  not paginate** - GitHub's own docs say so - and a truncated list reads as "nothing changed", which is the
  worst possible failure for this job. `fetch-depth: 0` costs nothing here either: the packed history is
  11 MiB.
- **`actions/checkout` sparse-checkout and `filter: blob:none` for the small jobs.** Same measurement. There is
  no time to save.
- **`gate.sh`.** GitHub provides no built-in "did every job pass" gate, and feeding it `toJSON(needs)` is
  already the only shape in which the job list cannot drift from `needs:`.
- **`check-version.sh`.** `workflow_dispatch` inputs have no pattern or regex validation; a `type: choice` would
  need every version enumerated. A check in the run is the only option.
- **`peter-evans/dockerhub-description`.** Docker publishes no action for the repository description, and the
  API route needs a JWT exchange - more hand-rolled code than the action replaces.
- **`cloudflare/wrangler-action` and `sigstore/cosign-installer`.** Both first-party, from the org that owns
  the tool. Nothing to change.
- **`extractions/setup-just`.** No first-party alternative exists; the just project points at this one.
- **`npm ci --ignore-scripts` repeated in six places.** Folding it into `setup-node` would make an action named
  for setup also install, which is the surprise the split exists to avoid.
- **`if: ${{ !cancelled() }}` on thirty-odd test steps.** GitHub has no job-level default for `if:`. Repeating
  it is the only way to express it.
- **`jq`, `gh`, `git` and `docker`.** All preinstalled on the runners, and no workflow installs any of them.
  Nothing to delete - this was checked because it is a common waste, and it is not happening here.
- **The staging reference assembled by hand in the `build` job.** `metadata-action`'s `tags` output would give
  the same string today, but only while exactly one tag rule is configured; it is newline-separated and would
  break as a job output the moment a second rule is added. Building it from the image and the version survives
  that.
- **`actions/attest-build-provenance`.** This is the first-party mechanism for what the cosign steps hand-roll,
  and it has one genuine advantage: attestations are stored by GitHub against the digest, so they survive the
  copy to Docker Hub and the image would not need signing twice. **It is still not worth it.** Adopting it
  means rewriting the verify command users are given on four public surfaces at once, and a failed verify after
  such a change reads to a user as tampering. Adding it *beside* cosign is worse again - two verification
  stories for one image. Written down so this does not get proposed as an easy win.
- **`.github/dependabot.yml`'s five near-identical blocks.** A glob in `directories:` for the github-actions
  ecosystem is still not supported and is reported to produce duplicate pull requests; explicit entries per
  action folder remain the recommended shape. Checked 2026-08-28.
- **`just check workflows` printing its file list and count.** actionlint would find the files on its own, but
  it says nothing on a clean run, so the count is what distinguishes a pass from a run that never started.

## Already decided

Things the ledger settled that this review would otherwise have raised. Entry numbers are given so the
reasoning can be found; in every case the reason still holds.

- **D16 - lychee as a pinned binary rather than `lycheeverse/lychee-action`.** The action takes lychee's flags
  from YAML, so the check would stop being `just check links <site>`. Unchanged, and finding 11 is built on top
  of it rather than against it.
- **D11 - SHA pinning, first-party actions included, with Dependabot moving them.** Nothing found argues with
  it. The Dependabot glob question is settled above.
- **D6 - `shared-smoke-image.yml` pinned to `ubuntu-24.04` because hurl links `libxml2.so.2`.** I looked for
  the escape used for lychee: **hurl publishes no musl build.** The 8.0.1 release assets are gnu tarballs, two
  `.deb` packages and the mac and windows builds, and the `.deb` links the same library. Checked against the
  release on 2026-08-28. The only route that would remove the pin is running hurl from its official container
  image, `ghcr.io/orange-opensource/hurl`, which costs the smoke recipe its "same command on a laptop"
  property. **The pin is right. Do not re-open this without a new upstream asset.**
- **D2 and D14 - build once, smoke the registry copy, copy by digest, GHCR as staging.** Finding 8 touches one
  action inside `publish` and changes none of this.
- **D1 - the release is dispatched with a version and the tag is made last.** Finding 2 strengthens it: the
  residual risk D1 accepts - a window between publishing and tagging - closes rather than narrows.
- **D4 - a step calls a recipe, and the shell lives in `tooling/ci/`.** One correction worth recording: the
  stated reason includes "a `run:` block is invisible to shellcheck", and that half is **not true** - actionlint
  runs shellcheck over every `run:` block when shellcheck is present, and both are installed in the `workflows`
  job. The decision stands on its other leg, which is sound and is the stronger one anyway: a `.sh` file is a
  real filename in a stack trace and can be run on its own. Finding 3 is what makes the shellcheck half true
  for the scripts themselves.
- **D12 - the framework-dependent publish flags.** The entry warns that the flags are written twice, in
  `tooling/build.just` and again in the release workflow's publish step. **They are not, any more** - the
  workflow calls `just build publish` and the flags appear once, in `build.just`. Nothing to do in CI; the
  warning is simply describing a shape that no longer exists.
- **D18 - two test suites split by what ships, with five javascript tests in both.** Correct as written.
  Finding 12 is about the path filter, not the split.
- **D8 - Sonar runs by hand and Automatic Analysis stays off.** Finding 10 does not touch the trigger.

## Done when

Every box here is a decision recorded or a change made. Several are independent; none blocks another.

- [ ] The Docker Hub plan question is answered - does the org have an OIDC connection available.
      **By eye.** Open the Docker Hub org's settings and look for GitHub OIDC connections. If yes, finding 1
      is live; if no, this file says so and the finding is struck.
- [x] The release workflow has no tag-push step, and the tag is created by the release itself.
      Done 2026-08-28. `github-release.sh` takes the commit and passes `--target`; the tag-push step is gone.
      D1 amended. **Only a real release proves it.**
- [x] Every script in `tooling/ci/` is shellchecked by a pull request.
      Done 2026-08-28. `just check scripts`, called by the lint job. Sixteen scripts, no errors.
- [x] `push-tag.sh` sets no git identity.
      Done 2026-08-28. The three deploy marker tags still use it.
- [ ] The `persist-credentials` question is answered either way.
      **By eye.** Either every checkout that does not push carries `persist-credentials: false`, or a line in
      the CI/CD decisions ledger says why not.
- [ ] The three deploy workflows share one body, or a line says why they should not.
      **By eye.** Either `.github/workflows/shared-deploy-site.yml` exists and the three callers are under
      twenty lines each, or the decision to keep three copies is written down where the next reviewer meets it.
- [x] The container-structure-test checksum names its upstream source.
      Done 2026-08-28. Fetched `checksums.txt` from the v1.22.1 release and compared: same value.
- [ ] The Docker Hub credential is scoped, or the decision not to is recorded.
      **By eye.** Either the `publish` job declares an `environment:` with a `main`-only branch policy, or a
      line says why a repository secret is accepted.
- [ ] `docker/setup-buildx-action` is gone from `publish`, proved by a real dispatch.
      **By eye.** A prerelease run whose copy step is green with no buildx setup above it. If it fails, the
      action goes back and a line here says so.
- [x] `setup-just` prints its version and the smoke workflow's bare version step is gone.
      Done 2026-08-28. Sixteen jobs get the line now instead of one.
- [ ] The Sonar wait is decided.
      **By eye.** Either `sonar.qualitygate.wait=true` is on the `begin` command and the summary step carries
      `if: always()`, or a line says the poll stays because a red run for coverage is not wanted yet.
- [x] The four install actions hold no inline shell.
      Done 2026-08-28. Each is a door onto `tooling/ci/install-<tool>.sh`, called by path rather than through
      `just`. Four scripts, not one parameterised script - the argument list would read worse than the copies.
- [ ] The site half of the path filter is decided.
      **By eye.** Either `changed-paths.sh` narrows `.github/` to `.github/actions/`, or a comment in that file
      says the breadth is deliberate.
