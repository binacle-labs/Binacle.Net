---
id: ci-cd/github-surface
description: What GitHub offers a repository, what this one uses, and the ten Actions gotchas that fail quietly
verified: 2026-08-31
check: the What this repository uses today section against .github/ and the repo root - the workflow, action and health-file counts are what move. Every row under Platform settings is unverified: it was read from the working copy, not from the GitHub settings pages, so re-read the settings before trusting any of them; the Docker Hub page section against .github/dockerhub-overview.md, which must still carry the three-row tag policy table, no tag list, no GHCR mention, no committed version, and a cosign block matching SECURITY.md
paths:
  - ".github/**"
---

# The GitHub surface

**Compiled 17 Aug 2026** against `binacle-labs/Binacle.Net` at `main`. File status was read from the working copy; **platform status was not** - so every settings
row below is "check this", not "this is off".

---

## Four hard limits

None is a preference and none has a workaround.

**No subdirectories under `.github/workflows`.** The reusable-workflow documentation says it in one line. A
file at `.github/workflows/shared/test.yml` is not a workflow, it is an ignored file. Every workflow, shared
or not, sits in one flat directory.

**A reusable workflow cannot be hidden from the Actions tab.** There is no `show_in_ui` key. A workflow
declaring only `workflow_call` genuinely cannot be run - no button, no dispatch - but it still appears in the
sidebar with an empty run list.

**A composite action is the lever that does work.** It lives in a real subfolder, never appears in the Actions
tab, and cannot be run on its own: `.github/actions/<name>/action.yml`, called as `uses: ./.github/actions/<name>`.

**What a composite action cannot hold.** It is a run of steps inside somebody else's job, so it cannot set
`runs-on`, `services:`, `environment:`, `permissions:` or `timeout-minutes` - all job-level keys that stay
with the caller. It is also read out of the working copy, so the caller must have checked out first.

**The resulting rule:** the question is not "is this shared", it is **"does this need its own machine"**.
Needs its own runner, service containers, or to run beside other jobs, and it must be a workflow, and it will
be visible. Only a run of steps, and it can be an action, and it disappears.

---

## Ten gotchas that fail quietly

That is what makes them worth writing down rather than rediscovering.

- **Secrets do not cross into a called workflow.** The caller needs `secrets: inherit`. The failure looks like
  an empty variable, not a permissions error.
- **`environment:` is not supported in `on.workflow_call`.** A callable workflow cannot declare one, so the
  deployment URL has to stay with the caller.
- **Permissions only narrow going down a call chain.** A caller cannot grant a callee more than it holds, so a
  shared workflow's needs must be met at every entry point that calls it.
- **A skipped required check blocks the pull request forever.** This is what makes path filters dangerous on a
  required job. The fix is a job that always runs and reports the required name, deciding internally whether
  to do the work.
- **Renaming a workflow or a job changes the required status check name.** A required check that no longer
  reports leaves every PR waiting on it with nothing saying why.
- **A called workflow reports as `caller job / callee job`.** That whole string is what shows on a red check
  and what a ruleset matches, so both halves have to be short and different from each other.
- **A reusable workflow's output is mapped twice** - step output to job output, job output to
  `on.workflow_call.outputs`. Three edits per value.
- **Draft releases fire no events.** Publishing a draft fires `published`; `prereleased` does not fire for a
  prerelease published from a draft. There is no "create it quietly, announce it later" event to build on.
- **Events created using `GITHUB_TOKEN` generally do not start new workflow runs.** Useful as loop protection,
  and a trap when a chain is wanted.
- **Reusable workflows nest ten levels deep at most**, and loops in the call tree are rejected.

**Answered 2026-08-19, and the answer was the bad one.** Dependabot's `github-actions` ecosystem with
`directory: /` does **not** reach an `action.yml` in a subfolder, so every pin moved into a shared action
stopped being updated silently - worse than an unpinned action, because nothing reports it. **The fix is one
`dependabot.yml` entry per action folder**, which is the shape the file carries now. `decisions.md` D11 holds
the reasoning.

---

## What this repository uses today

`.github/` holds `dependabot.yml` (actions only, weekly, grouped minor and patch, with one entry per action
folder that pins an outside SHA), **eleven workflows**, **nine composite actions** under `actions/`,
`ISSUE_TEMPLATE/` and `PULL_REQUEST_TEMPLATE.md`, and `dockerhub-overview.md` — the source the release
pipeline renders onto the Docker Hub page. The root holds `README.md`, `SECURITY.md`, `CHANGELOG.md`,
`DEVELOPMENT.md`, `CONTRIBUTING.md`, `CLAUDE.md`, `CONTENT-TERMS.md`, `NOTICE`, and **one licence file,
`LICENSE.AGPL-3.0`**. `LICENSE.GPL-3.0` is a *directory* holding the old text and a `README.md`, and that
shape is load-bearing - `decisions.md` D6 has why. Counted 2026-09-04.

**`CONTRIBUTING.md` and `.github/PULL_REQUEST_TEMPLATE.md` landed on 2026-08-30**, and they say the same
thing: **code contributions are not being taken at the moment, issues are.** The stated reason is that
contribution terms do not exist yet and are worth writing before the first pull request rather than after. A
pull request opened today is closed with a link to the file, and the template carries the same notice so
somebody sees it before they spend the effort. **This is a holding position and the file says so** - it is
reversible in a way that merging a pull request without terms is not.

**Issue templates landed 2026-09-04** - `bug_report.yml`, `packing_result.yml` and `config.yml` under
`.github/ISSUE_TEMPLATE/`. **Still missing:** `CODE_OF_CONDUCT.md`, `CODEOWNERS`, `SUPPORT.md`, `FUNDING.yml`
and `CITATION.cff` — the last three are absent on purpose per the table below, and `SUPPORT.md` is answered
by `config.yml` linking out.

**Two worth a decision and nothing more:**

- **`CODEOWNERS`** - thin for a solo maintainer, except as the mechanism that flags any pull request touching
  `sites/`.
- **`CITATION.cff`** - usually academic decoration, except this repository ships the OR-Library and Bischoff
  suite scenarios, so bin-packing researchers are a plausible audience.

**`release.yml` is not wanted.** It configures GitHub's auto-generated release notes by PR label; the release
pipeline writes notes from `CHANGELOG.md`, which is better and hand-authored. It would fight it.

**`workflow-templates/` does nothing here.** Starter workflows only work from the organization's own `.github`
repository.

### Considered and not taken - so the survey does not get run again

| Surface | Call |
|---|---|
| `SUPPORT.md` | **Redundant.** The issue-form `config.yml` links out instead, which is the same job in a place people actually see |
| `FUNDING.yml` | **No.** Not wanted |
| `labeler.yml` | Path-to-label rules. Needs `actions/labeler` running; the config alone does nothing |
| `copilot-instructions.md` | The Copilot equivalent of `CLAUDE.md`, read by Copilot code review. Only if that reviewer gets used |
| `DISCUSSION_TEMPLATE/` | Discussions is on since 19 Aug, so this is now possible. Nothing to template yet |
| `release.yml` | **Fights the pipeline.** It configures GitHub's auto-generated notes by PR label; notes come from `CHANGELOG.md`, hand-authored |
| `workflow-templates/` | **Wrong repository.** Starter workflows only work from the org's own `.github` |

**Three platform settings, checked once and then ignored:**

- **The Community Standards page** at `/community` scores the repository against README, licence, code of
  conduct, contributing, and issue and PR templates. **Worth loading once for the checklist**, then ignoring
  the parts that are not wanted.
- **Merge settings** - squash-only, auto-delete head branches, allow auto-merge. Unverified.
- **Environments** - all three site deploys declare one, which is what carries the deployment URL. Required
  reviewers and wait timers are available and unused.

---

## What the Docker Hub page carries, and what it deliberately does not

`.github/dockerhub-overview.md` is the source; the release pipeline's `page` job renders and PATCHes it. Each
of these was decided and each is easy to undo by accident.

- **No tag list.** Fifteen hand-maintained entries duplicating a Tags tab that is always right, and it is what
  rotted the page the first time. The three-row policy table replaces it and answers the one thing neither tab
  explains: which tag belongs in a compose file.
- **No mention of GHCR.** It is staging. The reason to name it would have been handing rate-limited users an
  escape hatch, and the sponsored badge means there are none. A staging registry on a landing page becomes a
  support surface nobody meant to own.
- **No concrete version committed into the file.** Placeholders and substitution, or the page is wrong the day
  the next minor ships. Same reason the tag list went.
- **No service module, no ViPaq, no health endpoint.** The module is not advertised here, at the maintainer's
  call. ViPaq belongs on the docs site: on a page where someone is deciding whether to run one command, a
  second format name is a reason to hesitate. The health endpoint is off by default and its path is
  configurable, so a line about it is wrong for most readers.
- **The `cosign` block is copied verbatim from `SECURITY.md`, only the tag differs, and that file is the
  source.** A published verify command that fails reads as a bug in the project. For the same reason the page
  must never name a tag that is absent or unsigned - the org move re-keyed the certificate identity, so
  anything signed under the old owner fails the published command.
- **`DOCKERHUB_TOKEN` is not widened.** One registry push credential also writes the repository description,
  HTTP 200, proved 2026-08-19. It already does both jobs at the scope it has.
- **Do not dispatch `shared-dockerhub-overview.yml` with an empty version input to check a wording change.**
  Empty takes the latest non-prerelease release, which is not always the version the page was just edited for.
  Render it locally with `just image dockerhub-overview <version>`, or type the version.

**And it is not a substitute for the digest-preserving copy.** The page describes the tag policy; the
pipeline's shape is what makes it true.

---

## Platform settings - all unverified

**The one it argued hardest for is now in place, and this section was not updated.** A tag ruleset exists on
`refs/tags/v*` - note the pattern, not the `v[0-9]*` argued for here - blocking update and deletion, creation
still allowed, empty bypass list. `decisions.md` D24 is the record and it supersedes this paragraph. **That
one row of this section is settled; every other row below is still unverified**, and one of them turning out
to be stale is reason to suspect the rest.

**Why it matters.** Everything downstream of a tag treats it as permanent: the image carries the version as a
build argument, the release body is drawn from the changelog section that tag names, the signature covers a
digest built from that commit, and Docker Hub serves tags derived from it. **Move the tag and every one of
those becomes a claim about a commit that is no longer there - with nothing failing**, because they were all
correct when they ran.

It is also **the cheap half of the immutability question**, because it protects the tag without touching
prereleases, so beta re-cuts keep working.

**Immutable releases - check the caveat first.** Generally available since October 2025. Locks the tag to its
commit, locks the assets, and generates a release attestation automatically. But `3.0.0-beta.2` was re-cut on
13 Aug 2026, which is a normal thing to do to a beta, and under immutable releases that becomes impossible.
**Same answer as the Docker Hub immutability work: scope it to released versions, never to prereleases** -
check whether the GitHub setting can be scoped that way before switching it on. Also test it against the
release job's create-or-edit step, which rewrites the body of a release that may already exist.

**Free and probably worth it:** secret scanning with push protection (blocks a credential before it lands
rather than reporting it after), private vulnerability reporting (`SECURITY.md` already exists; this is the
mechanism that lets someone use it without falling back to email), and setting the default token permissions
to read-only - which changes nothing today, since every job declares its own, but means the next workflow
added starts safe.

**Artifact attestations are largely redundant here** - the pipeline already produces SLSA provenance and an
SPDX SBOM through buildx, and signs the digest with keyless cosign in both registries.

**CodeQL is complementary to Sonar rather than overlapping**, and free on public repositories. It costs one
more workflow file, which cuts against reducing the count.

---

## Why the reasoning is the part worth keeping

A list of settings goes stale; the reason a setting exists does not. **When GitHub renames a menu or ships a
replacement feature, the "why" is what lets the decision be made again rather than re-researched.**

Each of these would be defensible to skip. What decides it is the failure each one prevents, and in every case
that failure is silent - a moved tag that breaks nothing until someone verifies a signature, a bug report that
dies waiting for a detail, and the org's `.github` repository quietly giving a new repository no policy at all.
