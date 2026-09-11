---
description: Seven open CI questions left by the platform sweep - Docker Hub OIDC, persist-credentials, one deploy workflow instead of three, scoping the registry credential, dropping setup-buildx-action, the Sonar wait, and the site half of the path filter. All seven close on a sentence; all were re-verified on 2026-09-11
state: ready
waits-on: "nobody - answered by the maintainer 2026-09-11. Findings 1, 4, 5, 8, 10 and 12 are approved, 7 is rejected. Finding 1 still needs the Docker Hub eligibility checked at the org, and finding 12 goes in narrowed to site files"
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
  - "tooling/ci/**"
---

# CI - the questions the platform sweep left open

Read on 2026-08-28: the eleven workflows in `.github/workflows/`, the nine composite actions in
`.github/actions/`, the scripts in `tooling/ci/` and `tooling/ci.just`, plus `tooling/check.just`,
`tooling/build.just` and `.github/dependabot.yml` where a workflow step reaches into them.

**Answered by the maintainer, 2026-09-11 - each one put to him for a yes or a no.**

| Finding | Answer | What the yes takes |
|---|---|---|
| 1 | **yes** | check the org can create an OIDC connection first; the page job keeps its token |
| 4 | **yes, both halves** | delete the two dead lines, tag through `gh api`, then `persist-credentials: false` on every checkout |
| 5 | **yes** | one workflow, the site as a `choice` input at dispatch - his call on 2026-09-12, over the called workflow first proposed |
| 7 | **no** | recorded as `D29` in the CI/CD decisions ledger |
| 8 | **yes** | delete the step from `publish` |
| 10 | **yes** | `qualitygate.wait=true` on `begin`, drop the loop, `if: always()` on the summary step |
| 12 | **yes, narrowed** | not `^\.github/actions/` alone - also `shared-site-tests.yml` and the site deploy workflow, so a change to a site workflow still runs the site tests |

**How the release-path changes get proved.** Findings 1 and 8 both edit the `publish` job, and nothing short
of a run proves that job. The proof is a prerelease dispatched from `main` after the changes merge - the
workflow that runs is the changed one and the signature is on the right ref. **Not from a branch**: `gate`
refuses any other ref, a branch-built image signs under the branch and fails the published verify command,
and the direction is that a prerelease never lands where users pull from. The release set says when.

**Every open finding was re-checked on 2026-09-11** and each carries a dated verdict line. Where a number in
this file was measured and found wrong it has been corrected in place. One of those: `tooling/ci/` holds
twenty-two `.sh` files, and `just check scripts` globs `tooling/*.sh tooling/ci/*.sh`, so it checks
twenty-eight.

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
  enrolled in the Docker Sponsored Open Source Program. **Check the org qualifies before doing any of this** -
  that is a login-only fact, and if it does not, this finding is dead.
- **It does not close the secret completely.** `shared-dockerhub-overview.yml` writes the repository
  description through the Docker Hub **web API**, not the registry, using `peter-evans/dockerhub-description`,
  which takes a username and password. An OIDC registry login does nothing for it. So the secret survives for
  the page job; what goes away is the credential in the job that moves the artifact.

**Confidence:** high on the mechanism and the action version, unknown on eligibility.

**Verified 2026-09-11 - holds, except the eligibility half.** The pin `dbcb8138` resolves through the GitHub
tags API to v4.6.0, and v4.6.0 is also where the `v4` tag points, so this is the newest v4. OIDC login landed
in v4.5.0, released 2026-07-23. The README still shows the snippet above word for word: `username:` kept,
no `password:`, `DOCKERHUB_OIDC_CONNECTIONID` in `env:`, `id-token: write` in `permissions:`. The tiers are
still Team, Business, Hardened Images and the Sponsored Open Source Program, and they still cannot be checked
from outside the org. The page half holds too - `shared-dockerhub-overview.yml` still hands `username` and
`password` to `peter-evans/dockerhub-description`, so that secret survives either way.

## 2. `gh release create` makes the tag itself, so the release job's tag step can be deleted

**Done 2026-08-28.** `github-release.sh` takes the commit and passes `--target`; the "Push the release tag"
step is gone. D1 was amended with it - the window in which the image is published and the tag does not exist
**closes** rather than narrows, because the tag and the release are one API call.

**v3.0.0 is the real release that proved it**, 2026-09-01.

## 3. Nothing in CI runs shellcheck over `tooling/ci/*.sh`

**Done 2026-08-28.** `just check scripts`, called by the lint job. Sixteen scripts, no errors. It also covers
the four install scripts that came out of finding 11, so the unlinted shell in CI is now roughly zero.

**It also forced one correction to D4** - see *Already decided*.

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
`actions/checkout` persists. `persist-credentials: true` is checkout's default. With no pushes left, every
checkout in every workflow can carry `persist-credentials: false` uniformly, so a compromised step in any job
cannot use or read a working token. **Measured 2026-09-11:** twenty-two `actions/checkout` steps across
`.github/workflows/`, none in `.github/actions/`. Three push - the `tag` job of each of the three deploy
workflows, which is the only `git push` left in the repository. Nineteen do not.

**Cost:** nineteen `persist-credentials: false` lines today, twenty-two once the tag push is replaced, and a
failure message from the API that reads worse than git's ("Reference already exists" rather than "tag already
exists"). The deploy `tag` job still checks out, because `deploy-summary.sh` reads the commit subject with
`git log`.

**Confidence:** high on the dead config lines - they are proved. High on the API working; medium on whether
the added lines are worth the hardening, which is the maintainer's call.

**Verified 2026-09-11 - holds in mechanism, and two claims were wrong.** The pin `3d3c42e5` is
`actions/checkout` v7.0.1, the newest v7. `persist-credentials` still defaults to `true` - read from
`action.yml` at v5.0.0, v6.0.0 and v7.0.1 - so nothing about the default has moved. The breaking change in
those versions was `allow-unsafe-pr-checkout`, a different input.

Two corrections. **First, the attribution.** GitHub's own Actions security pages do not mention
`persist-credentials` at all - the *Secure use reference* and three neighbouring pages were grepped and none
carries it. The guidance is real but it comes from workflow auditors, chiefly zizmor's `artipacked` audit, not
from GitHub. **Second, the benefit is smaller than written.** checkout v6.0.0 moved the persisted credential
out of `.git/config` into `$RUNNER_TEMP`, and zizmor lowered `artipacked`'s severity for checkout v6 and above
because of it. This repository is on v7.0.1, so the leak this guards against - a `.git/config` carried out
inside an uploaded artifact - is already closed. What remains is that a compromised step inside the job can
still use the token.

`POST repos/{repo}/git/refs` is still the documented way to create a ref, taking `ref` and `sha`, so the
replacement for `push-tag.sh` works as written.

## 5. The three deploy workflows are one workflow with three sets of five values

**Now:** `deploy-docs-site.yml`, `deploy-demo-site.yml` and `deploy-www-site.yml` are 80 lines each and
identical except for the site slug. 240 lines, three copies of the same three-job chain, three places to edit
when the host changes or a step is added.

**Measured 2026-09-11 by diffing all three:** everything that differs is derived from one value, the slug
`docs` / `demo` / `www`. The environment is `binacle-net-<slug>`, the URL `https://<slug>.binacle.net`, the
directory `sites/<slug>`, the wrangler config `tooling/cloudflare/<slug>.wrangler.jsonc`, the marker tag
`<slug>-<run number>`, and the link check `just check links <slug>`. Only the human-readable `name:` at the
top of the file is a second value. So the reusable workflow takes one input, not five.

**The supported way:** a reusable workflow, `shared-deploy-site.yml`, taking the slug as an input. A reusable
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

**Verified 2026-09-11 - holds, and it is cheaper than the finding claimed.** The mechanics are confirmed
twice over. GitHub's context table allows `inputs` in both `jobs.<job_id>.environment` and
`jobs.<job_id>.environment.url`, so the environment name and the deployment URL can both come from the input.
A called workflow's jobs are ordinary jobs and set their own `runs-on`, `permissions` and `timeout-minutes` -
`shared-site-tests.yml` already does all three and is called with `uses:` from four places. `environment:` at
job level in a called workflow is documented as allowed.

**One gotcha to carry into the work.** GitHub's docs warn that `on.workflow_call` has no `environment`
keyword, and that where the called job declares `environment:`, an environment secret wins over one passed
from the caller. The two Cloudflare secrets are repository secrets today, so they must be passed on the call
or moved into each environment - not left to chance.

## 6. `container-structure-test` publishes checksums now, and the pin's comment says it does not

**Done 2026-08-28.** Fetched `checksums.txt` from the v1.22.1 release and compared: the same value, byte for
byte. The comment in `.github/actions/install-container-structure-test` names upstream as the source now, so
the checksum stopped being self-confirming.

## 7. An environment with a branch policy would scope the Docker Hub credential to `main`

**Now:** `DOCKERHUB_TOKEN` is a repository secret, not scoped to an environment. The release is kept to
`main` by `check-release-ref.sh`.

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

**Rejected 2026-09-11 by the maintainer.** With finding 1 the long-lived token leaves `publish` anyway, so
most of what this would scope stops existing; what is left is the page job's token. And an environment that
admits `main` only refuses any prerelease dispatched elsewhere, which is a door worth keeping open. `D29`.

**Verified 2026-09-11 - holds.** GitHub's docs are explicit on all three parts. An environment secret is
readable only by a job that names that environment; a job cannot reach it until the environment's protection
rules have passed; and the deployment branch or tag rule is matched against the run's `GITHUB_REF`. So a job
on any other ref cannot read the secret, which is exactly what the finding claims. Nothing upstream moved and
the cost argument is unchanged.

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
at the copy step - late, and on a real release. A prerelease dispatch is still the cheapest place to find out.

**Confidence:** high. See the verdict below - it is no longer a reading.

**Verified 2026-09-11 - holds, and it is now proved rather than argued.** Three pieces of evidence.

- **The numbers are right.** `ubuntu-latest` still resolves to Ubuntu 24.04 - 26.04 exists but only under its
  own label and in preview - and the 24.04 image README still lists `Docker-Buildx 0.36.1`. Docker's reference
  still says the sources "must already exist in the registry where the new manifest is created".
- **It was run.** With only the default `docker`-driver builder, which is the state a runner is in with no
  `setup-buildx-action`, `docker buildx imagetools create` copied a tag inside a throwaway local registry and
  the new tag read back as the source digest. Local buildx was 0.30.1 against the runner's 0.36.1.
- **The authenticated case is already proved in this repository.** `shared-smoke-image.yml` has no buildx
  setup at all, and `pull-image.sh` runs `docker buildx imagetools inspect` there against an authenticated
  `ghcr.io` image. That step ran green on the v3.0.0 release. Auth for `imagetools` comes from the docker
  config that the login action writes, which `setup-buildx-action` has nothing to do with.

What a prerelease dispatch would still prove is the release path end to end, not whether `imagetools` needs a
builder. That question is answered.

## 9. A step that prints `just --version` and nothing reads it

**Done 2026-08-28.** `.github/actions/setup-just` prints its version and the bare step in
`shared-smoke-image.yml` is gone. Sixteen jobs get the line now instead of one.

## 10. Sonar's five-minute poll loop is one scanner property

**Now:** `sonar-summary.sh` polls `ceTaskUrl` up to sixty times, five seconds apart, waiting for SonarCloud to
finish processing, because `Sonar end` returns when the upload finishes rather than when the analysis is done.
Twelve lines of loop, sixteen with its comment and the `status` it seeds. The ten lines below it that handle a
task which never reached `SUCCESS` stay whatever happens.

**The supported way:** `/d:sonar.qualitygate.wait=true` on the `begin` command. The scanner then blocks at
`end` until the analysis is processed, and exits non-zero if the gate fails. The summary script keeps its two
API calls but drops the loop: one request to `ceTaskUrl` returns `SUCCESS` straight away.

**Benefit:** the retry logic replaced by one flag on a command that is already there, and the wait becomes the
tool's problem rather than a timeout we chose.

**Cost, and it is the reason this is not an obvious yes:** `wait=true` also **fails the step** on a red gate.
Coverage is deliberately not blocking anywhere yet - the read-only gate asks 80% on new code and the project is
below it, so this would paint the Sonar run red every time it ran. The summary step would need `if: always()`
to still write its table. Whether a red run that blocks nothing is honest or is noise is the maintainer's
call, and it is the whole decision here.

**Confidence:** high on the mechanism, and the decision is his, not mine.

**Verified 2026-09-11 - the mechanism holds, but the cost is bigger than when this was written.** Sonar's
docs confirm the flag: the Scanner for .NET takes `/d:sonar.qualitygate.wait=true` on `begin`, waits for the
gate and fails the job when it is red. `sonar.qualitygate.timeout` defaults to 300 seconds, the same five
minutes the loop here chose by hand.

**What changed.** On 2026-08-28 `sonar-analysis.yml` was `workflow_dispatch` only, and its own comment said
"By hand only - no schedule, and no pull request trigger yet". It now also carries `workflow_call`, and
`pull-request.yml` has a `sonar` job that calls it on every pull request that touches code. So "this would
paint the Sonar run red every time it ran" is no longer once per hand-dispatch, it is once per pull request.
The merge is not affected either way: `sonar` is deliberately outside `gate`'s `needs`. Whether that red is
honest or is noise is still the whole decision, only louder.

The 80%-on-new-code gate is a SonarCloud setting and could not be read from here.

## 11. The four `install-*` actions are one script written four times

**Done 2026-08-28, and it settled the opposite of what this finding proposed.** Each action is a single
`run: tooling/ci/install-<tool>.sh` - **four scripts, one per tool, not one script taking the tool as an
argument.** A repeated readable thing beat a shared clever one. The version and the checksum moved into the
script too, rather than staying in the action's `env:` where a reader and Dependabot were said to look -
Dependabot never looked, because it rewrites `uses:` pins and these four are hand-pinned binaries.

**Nothing official replaces any of the four, and this is the reason not to check again.**
container-structure-test and hurl publish no action, actionlint's own advice is the download script this
already does by hand, and lychee's action is refused under D16.

## 12. A workflow edit builds all three Jekyll sites

**Now:** `changed-paths.sh` sets `site=yes` for anything matching `^\.github/`. So editing a workflow, or a
markdown file under `.github/`, runs the site test suite and builds all three sites with their link checks.

**What it could be:** narrow that alternative to `^\.github/actions/`, which is the only part of `.github/` a
site build actually depends on - `build-jekyll-site`.

**Benefit:** a workflow-only pull request stops paying for three Jekyll builds and a sixteen-test suite.

**Cost, and it is why this is last:** a change to `shared-site-tests.yml` itself would then not run the site
tests it changes. That is a real hole, and it may be worth the current breadth. Both halves of the filter are
deliberately generous today and the reasoning for that is written down; this is a small saving against a small
risk, and either answer is defensible.

**Confidence:** medium on the mechanics, low that it is worth doing.

**Verified 2026-09-11 - holds, and the benefit is smaller than it reads.** `changed-paths.sh` does carry
`^\.github/` in its `site_input` alternation, so the mechanic is exactly as described, and the suite it fires
is sixteen tests, not fifteen - six typescript, ten ruby.

**But the same script sets `code=yes` for a `.github/` edit too.** Its `not_code` pattern is
`^(\.agents/|sites/|ruby/)|\.md$`, which does not exclude `.github/`, so a workflow-only pull request already
runs the image tests, the image build, the lint job and now Sonar whatever the site half decides. Narrowing
the site half removes the three Jekyll builds and the site suite; it does not make a workflow-only pull
request cheap. A markdown file under `.github/` on its own is the one case that goes fully quiet, because
`\.md$` takes the code half out.

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
  already the only shape in which the job list cannot drift from `needs:`. Re-checked 2026-09-11: GitHub's own
  docs still say a workflow skipped by a `paths:` filter leaves its checks pending and blocks the merge, and
  still recommend one always-running workflow instead. A skipped *job* reports success. That is exactly the
  shape here - one workflow, a `changes` job, per-job `if:`, one collector - so this stands.
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
- **D8 - Automatic Analysis stays off.** Finding 10 does not touch the trigger. The "runs by hand" half of
  that entry is out of date and the ledger needs it corrected: since 2026-08-28 `sonar-analysis.yml` has
  gained `workflow_call` and the pull request gate calls it on every code change.

## Done when

Every box here is a decision recorded or a change made. Several are independent; none blocks another.

**Trimmed 2026-09-02.** Findings 2, 3, 6, 9 and 11 landed on 2026-08-28 and their arguments were cut to what
a reader still needs. Seven are left and **every one of them now closes on a sentence, not on work** - the one
that needed a run got it on 2026-09-11:

| Finding | What it takes |
|---|---|
| 1, Docker Hub OIDC | **one login.** Open the Docker Hub org settings and see whether OIDC connections are offered. If not, the finding is dead and this is a one-line strike |
| 4, `persist-credentials` | a yes or a no. Yes is fifteen lines across every checkout; no is one line in the ledger |
| 5, one deploy workflow | a yes or a no. **This one is taste, not mechanics** - where you want to read the host name |
| 7, scope the credential | a yes or a no. Yes is a GitHub environment and moving two secrets into it |
| 8, drop `setup-buildx-action` | a yes or a no. The mechanics were proved on 2026-09-11; a prerelease dispatch now only proves the release path |
| 10, the Sonar wait | a yes or a no. Yes paints the Sonar run red every time until coverage passes, and that is the whole decision |
| 12, the path filter | a yes or a no, and *low confidence it is worth doing* is already written into it |

**Nothing here blocks anything and nothing decays.** It is seven answers, and a no is an answer.

- [ ] The Docker Hub plan question is answered - does the org have an OIDC connection available.
      **The workflow edit landed 2026-09-12** - `grep -c 'password:' .github/workflows/release-docker-image.yml`
      returns 2 outside comments, both GHCR. `D33`. **The maintainer creates the connection and sets
      `DOCKERHUB_OIDC_CONNECTIONID` before the beta**; the release set carries it as a to-do. Until then every
      dispatch fails at the Docker Hub login.
      **By eye.** Open the Docker Hub org's settings and look for GitHub OIDC connections. If yes, finding 1
      is live; if no, this file says so and the finding is struck.
- [x] The release workflow has no tag-push step, and the tag is created by the release itself.
      Done 2026-08-28. `github-release.sh` takes the commit and passes `--target`; the tag-push step is gone.
      D1 amended. **Only a real release proves it.**
- [x] Every script in `tooling/ci/` is shellchecked by a pull request.
      Done 2026-08-28. `just check scripts`, called by the lint job. Sixteen scripts, no errors.
- [x] `push-tag.sh` sets no git identity.
      Done 2026-08-28. The three deploy marker tags still use it.
- [x] **2026-09-11.** The `persist-credentials` question is answered either way.
      `grep -c 'persist-credentials: false' .github/workflows/*.yml` sums to 22, one per checkout, and
      `grep -rn 'git push' tooling/ci` returns nothing - the marker tag is `create-tag.sh`, through `gh api`.
      `D30`. **The API call is unproved until a site deploys** - the demo deploy in the release set is the
      first.
- [x] **2026-09-12.** The three deploy workflows share one body, or a line says why they should not.
      `.github/workflows/deploy-site.yml` exists with a `choice` input, and `ls .github/workflows/deploy-*`
      lists nothing else. `just check workflows` - 9 workflows, no errors. `D32`. **Unproved until a site
      deploys.** `sites/README.md:40-41` still names the three old workflows - the release set carries that
      for a site session.
- [x] The container-structure-test checksum names its upstream source.
      Done 2026-08-28. Fetched `checksums.txt` from the v1.22.1 release and compared: same value.
- [x] **2026-09-11.** The Docker Hub credential is scoped, or the decision not to is recorded.
      Not scoped; the decision is `D29` in the CI/CD decisions ledger.
- [ ] `docker/setup-buildx-action` is gone from `publish`, proved by a prerelease dispatched from `main`.
      **The step was deleted 2026-09-11** - `grep -c setup-buildx .github/workflows/release-docker-image.yml`
      returns 1, the `build` job's. The box closes on the beta run.
      **By eye.** A prerelease run whose copy step is green with no buildx setup above it. If it fails, the
      action goes back and a line here says so.
- [x] `setup-just` prints its version and the smoke workflow's bare version step is gone.
      Done 2026-08-28. Sixteen jobs get the line now instead of one.
- [x] **2026-09-12.** The Sonar wait is decided.
      `grep -c qualitygate.wait tooling/ci/sonar-analysis.xml` returns 1, `grep -c 'for _' tooling/ci/sonar-summary.sh`
      returns 0, and the summary step in `sonar-analysis.yml` carries `if: always()`. D28 amended.
      **Unproved until the next pull request runs it.**
- [x] The four install actions hold no inline shell.
      Done 2026-08-28. Each is a door onto `tooling/ci/install-<tool>.sh`, called by path rather than through
      `just`. Four scripts, not one parameterised script - the argument list would read worse than the copies.
- [x] **2026-09-11.** The site half of the path filter is decided.
      Narrowed to the site's own `.github/` files - `D31`. `just check scripts` clean; the pattern was run by
      hand against nine paths, and only the actions, `pull-request.yml`, `shared-site-tests.yml` and a deploy
      workflow match.
