---
id: ci-cd/decisions/D20
description: CodeQL runs buildless, on merge only, and reports nothing on a check
status: pending
verified: 2026-09-29
check: D20 against codeql-analysis.yml, whose matrix must stay four languages on build-mode none with a category per language
paths:
  - ".github/workflows/codeql-analysis.yml"
  - "tooling/ci/codeql-summary.sh"
---

# D20 — CodeQL runs buildless, on merge only, and reports nothing on a check

**Recorded 2026-08-28**, from the comments in `codeql-analysis.yml` that were the only copy of it. One job per
language: `actions`, `csharp`, `javascript-typescript`, `ruby`. Findings land in the repository's Security
tab.

**Every language runs `build-mode: none`**, so no job installs a toolchain. If C# extraction is ever found to
miss code, that one matrix entry becomes `manual` with a `dotnet build` step — **never `autobuild`**, which
guesses at a solution this repository builds through `just`.

**On `push` to `main`, not on `pull_request`.** The gate is the only required check and code scanning is
advisory. A second pull request check that never blocks trains people to ignore it.

**The schedule earns its place here, unlike Sonar's.** The query packs change, so the same commit reports new
findings weeks later. That is a reason a Sonar schedule does not have — see `$ci-cd/decisions/D8`.

**`security-extended`, and quality queries stay off.** Sonar already reports quality, and two tools
disagreeing on the same line is how a finding stops being read.

**`category: /language:<name>` is load-bearing.** Without it, each upload replaces the last, so the job that
finishes last clears the other three languages' findings.

**The permissions are the smallest set that works.** `security-events: write` for the upload. GitHub's
template also lists `actions: read` and `packages: read`; both are for private repositories and private query
packs, and this one is public and uses neither.

**The `summary` job exists because no page in the run says how many alerts are open** once the matrix has
finished. It does not repeat the per-language results — the job list already shows those.
