---
id: ci-cd/decisions/D8
description: Sonar analysis is a CI run, Automatic Analysis stays off, and the published sites are in scope
status: pending
verified: 2026-09-29
check: D8's scope claims against tooling/ci/sonar-analysis.xml, whose exclusions must still name sites/*/js, sites/*/lib, the media folders and sites/**/*.html and must not exclude either site whole
paths:
  - ".github/workflows/sonar-analysis.yml"
  - "tooling/ci/sonar-analysis.xml"
---

# D8 — Sonar analysis is a CI run, and Automatic Analysis stays off

**Why:** coverage requires a build. Automatic Analysis only reads source, so it can never report coverage, and
the two fight if both are on. The build and the coverage run must sit between `Sonar begin` and `Sonar end` —
the scanner only sees projects compiled inside that pair — and a failing suite skips `Sonar end`, so a broken
run publishes nothing. That last part is deliberate, not a bug.

Two mechanical consequences: the checkout needs `fetch-depth: 0`, because a shallow clone makes all code look
new to the new-code comparison; and the scanner is a Java program whatever language it analyses, so the job sets
up a JDK.

**The trigger was `workflow_dispatch` and nothing else — recorded 2026-08-28, superseded 2026-09-10.** No
schedule: a nightly run re-analyses a commit nothing changed and reports the same numbers, which teaches
everyone that the run means nothing. The `pull_request` half of this is no longer current — the recorded
reason was that the coverage condition was red before anyone wrote a line, and that argument was spent once
the gate started passing on `main` on 2026-08-31. **`$ci-cd/decisions/D28` is the current shape**: `workflow_call`, called
from `pull-request.yml`, not a direct `pull_request` trigger.

**Scope and coverage paths live in `tooling/ci/sonar-analysis.xml`, not in the workflow.** The Scanner for .NET
ignores `sonar-project.properties`, so that XML is the file form it reads, and `/s:` needs an absolute path.
Only the key, org, token and host stay in the YAML.

## The published sites are in scope, and that was a reversal {#sites-in-scope}

**Changed 2026-08-09.** `sonar.exclusions` used to drop both site directories whole, and the reason given was
that they are a separate deliverable written in their own session - **a workflow reason, not a scope reason.**
The test in the analysis xml is whether the code is ours to author, review and change. It is.

**The cost landed exactly where it hurt.** Those Jekyll sites are the only public attack surface in the
repository, `5e5f8c02` was an XSS fix in one of them, and the exclusion kept Sonar from looking for the next
one.

What is in scope is small - the hand-written js and scss, and the site yml and json. The generated and
vendored parts (`sites/*/js`, `sites/*/lib`, the two `media` folders) are named individually and stay out;
they are gitignored, so a CI checkout would not see them anyway. `sites/**/*.html` stays out too, because a
Jekyll template with `---` front matter and Liquid in its attributes is not an HTML document and Sonar's HTML
analyser can only misread it.

**Measuring and fixing are separate jobs, and only fixing was ever restricted.** A finding under `sites/` is
not fixed in a coding session; it becomes a row for the session that owns those files.

## Four settings that cannot live in the repo {#sonar-ui-settings}

Scope, coverage paths and the test/product split are all in the repository. These four are only in the
SonarCloud UI, and they are what the gate actually hangs on.

- **New code period.** It was `previous_version`, and because the scanner is never passed `/v:` the project
  version never changed, so the period stayed pinned to the **first analysis, 2025-04-15**. Sixteen months of
  work counted as new code: 882 of 1059 code smells, and a gate asking 80% coverage on new code was really
  asking it of everything ever written. Set to **"Number of days = 30"** on 2026-08-09, and the new-code smell
  count fell from 882 to 8. **The textbook answer once Sonar runs on pull requests is "reference branch =
  main" - do not assume it applies here.** The Free plan analyses only `main` plus pull requests targeting it,
  and a pull request is already graded on its own diff whatever this says, so the setting may be unavailable
  or a no-op and "days = 30" may be permanent.
- **Two findings marked in the UI**, neither with an honest code fix: `S2245` on `getRandomInt.ts`, which
  picks demo data rather than secrets, and `S2068` on the `AccountGetResponse` OpenAPI example, where
  `PasswordHash` is the literal `"type::hash::salt"`.
- **Automatic Analysis stays off**, per this decision.
- **No source glob in the UI.** A leftover `sonar.inclusions` of `src/**/*` from a flat layout is what made
  the 2026-08-07 run index zero files **and still report success**. Scope is exclusions only, and they live in
  the xml.
