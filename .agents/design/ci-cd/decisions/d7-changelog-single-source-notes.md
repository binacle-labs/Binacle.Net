---
id: ci-cd/decisions/D7
description: CHANGELOG.md is the single source of release notes, and a missing section is fatal
status: pending
verified: 2026-09-29
check: D7 against tooling/changelog.just and the two changelog.*.sh it calls
paths:
  - "CHANGELOG.md"
  - "tooling/changelog.just"
  - "tooling/changelog.*.sh"
  - "tooling/ci/changelog-section.sh"
  - "tooling/ci/github-release.sh"
---

# D7 — `CHANGELOG.md` is the single source of release notes, and a missing section is fatal

`gh release create --notes-file`, with the body produced by `just changelog extract <section>`. The `gate` job
proves the section exists before anything is built. There is **no fallback to generated notes.**

**Why a changelog and not a per-release file.** The body used to come from `.agents/release-notes-<tag>.md`,
which was body-only so it could be published whole — that part was right and is kept. What was wrong is where
it lived: `.agents/` deletes a release's companions once the version ships, so the notes source was a file whose
own contract guaranteed it would disappear. A published release body is a permanent record, and it belongs in a
permanent file at the repo root where users read it.

**Why one section accumulates per cycle.** A beta checks that `## [Unreleased]` exists and publishes it as
the body of its prerelease (`$ci-cd/decisions/D3`); renaming that heading to the version is the last edit before the real tag.
A beta's notes are the in-progress notes at that moment, not a version of their own, which is why prereleases
are excluded from the file.

**Why no fallback.** The old `--generate-notes` fallback existed because a prerelease normally had no written
body. Under the current shape a prerelease checks `[Unreleased]`, which always exists mid-cycle, so the
fallback's only remaining effect would be to let a *real* release silently publish a commit list as its body.
Failing the build in seconds is the better outcome, and it is checked first for exactly that reason.

**Why the parsing is a script behind a `just` recipe and not inline YAML.** Same reason as `$ci-cd/decisions/D4` — CI and a
laptop must read the file the same way, and the exact body has to be previewable before the tag is pushed.
The awk moved from the recipe body to `tooling/changelog.extract.sh` on 2026-09-14 so shellcheck reads it;
the output was compared before and after and is byte-identical for `3.0.0` and `Unreleased`. A section terminates at
the next heading that *parses as a version*, not at the next `## `, because bodies carry their own subheadings
and stopping at those would truncate every section at its first one.

**Heading depth is normalised in the file and restored on the way out.** A release is `##` and its own sections
are `###`, so the file nests under a single `# Changelog`. `extract` shifts each section so its shallowest
heading returns to `##`, since a release body has no parent heading. Deriving the shift from the section's own
minimum keeps relative depth intact and means nothing has to be recorded anywhere.

**The docs site's release-notes page is hand-copied from this file, not generated from it — since
2026-08-14.** Each version folder on the docs site carries its own `release-notes.md`, and a version's section
is copied into it by hand; a patch release appends a section rather than replacing the page. **The cost is
known and accepted: the same notes live in two places and they drift.** v3.0.0 shipped with three additions the
page never had, because the release body gained content after the page was written.

**So the docs handover is the control.** Every release's docs deploy checklist has to list what the changelog
gained since that page was last written. `just changelog extract <section>` prints the current text to compare
against. Generating the page instead was rejected: the two audiences differ — a GitHub release body is read
once at the tag, and the docs page is read by someone already on that version — and a generator would have to
own the site's front matter and heading style as well as the text.
