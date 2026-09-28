---
id: ci-cd/decisions/D28
description: Sonar runs on every pull request that can carry the token, parallel and non-blocking
status: decided
verified: 2026-09-29
check: D28 against sonar-analysis.yml, whose `on:` must carry `workflow_call` and a concurrency group that does not read `github.workflow`, and against pull-request.yml's `sonar` job, whose `if:` must still gate on `changes.outputs.code`, a fork check and a Dependabot check, and which must not appear in `gate`'s `needs`
paths:
  - ".github/workflows/sonar-analysis.yml"
  - ".github/workflows/pull-request.yml"
  - "tooling/ci/sonar-analysis.xml"
  - "tooling/ci/sonar-summary.sh"
---

# D28 — Sonar runs on every pull request that can carry the token, parallel and non-blocking

**Decided (the maintainer, 2026-09-10):** "send an agent to do the sonar on pull request try to do it smarly so
it saves time?" That covers Sonar running on pull requests. **Not confirmed:** that it stays out of `gate`'s
`needs` - the agent chose that.

`sonar-analysis.yml` gained `on: workflow_call`; `pull-request.yml` adds a `sonar`
job off `changes`, calling it with `secrets: inherit`. `workflow_dispatch` stays, so a manual run is still
possible.

**Path-filtered the same as the other code jobs.** `if: needs.changes.outputs.code == 'yes'` - a docs-only or
site-only pull request does not run it, the same trade-off the manual-only run already had. A site-only pull
request still gets no PR-time Sonar even though the sites-in-scope reversal in `$ci-cd/decisions/D8` means Sonar
does read those files on `main` - the job reads only the filter's `code` half, and adding the `site` half is a
separate question, not this one.

**Not in `gate`'s `needs` - argued both ways, recorded rather than assumed.**

*For blocking:* the read-only "Sonar way" gate already passes on `main` - 80% on new code, crossed 2026-08-31,
`$ci-cd/decisions/O2` - so blocking would cost nothing today, and it would catch a regression before merge instead of after.

*For reporting only, which is what shipped:* whether coverage blocks a merge is a separate decision from
whether Sonar runs on the merge candidate, and `$ci-cd/decisions/O2` already reserves that call for the maintainer,
unanswered. Blocking here would decide it by side effect. It is also the whole lever on wall-clock: `gate`
finishes when its slowest required dependency does, and Sonar would become that dependency - see the
measurement below - while reporting only lets `gate` finish on the jobs it already waits for, with Sonar's
result landing a little later, read but never waited on.

**Measured before choosing, not assumed.** Read off the public Actions API for `binacle-labs/Binacle.Net` on
`main`, 2026-09-10. The nine most recent completed `sonar-analysis.yml` dispatches each ran one job, `Analyse
and publish`, in 260-357 seconds (roughly 4.5-6 minutes). The fifteen most recent `pull-request.yml` runs'
jobs: `Image tests` (`shared-image-tests.yml`, the current longest) at a 189-second median, up to 471 seconds
under runner contention; `Site tests` similar; `Image build`, `Lint` and the three `Site build` jobs all
faster. So Sonar typically becomes the slowest job in the fan-out, by about two minutes over the current
longest, worse under contention. Blocking would add that to every pull request's time to merge; reporting only
adds nothing, because `gate` does not wait for it.

**Skips, never fails, when the token cannot be present.** Two blockers, one `if:`:

```yaml
if: >-
  ${{ needs.changes.outputs.code == 'yes' &&
      github.event.pull_request.head.repo.full_name == github.repository &&
      github.actor != 'dependabot[bot]' }}
```

`secrets.SONAR_TOKEN` reads empty on a `pull_request` run from a fork - the `head.repo.full_name` comparison
catches that. A Dependabot pull request runs from a branch on this repository, not a fork, but reads from the
Dependabot secret store rather than the Actions one - the actor check catches that case separately, because
the fork check alone would not.

**`pull_request_target` was ruled out, not merely passed over.** It would hand secrets to a run that checks
out and can execute the pull request's own, possibly untrusted, code, on a public repository - a
credential-exposure route. Skipping the job is the only shape considered.

**`gate.sh` already reads a skipped dependency as passing** - `[[ "$result" != "success" ]] && [[ "$result" !=
"skipped" ]]` - so this would have worked whichever way blocking went; it is not what decided blocking
against.

**The concurrency group could not stay `${{ github.workflow }}-${{ github.ref }}`.** `github.workflow`
resolves to the *caller's* name when a workflow runs as a called (`workflow_call`) workflow, so under the old
key a PR-triggered Sonar run would carry the same group string as `pull-request.yml`'s own top-level
concurrency (`Pull Request-refs/pull/<n>/merge`) - colliding with a group already enforced one level up.
Changed to a literal `sonar-analysis-${{ github.ref }}`, unique to this workflow whichever way it starts. The
dispatch path is unaffected beyond the group's string changing.

**Nothing shared with the `image` job's build.** `image` runs `just build image`, a multi-stage Docker build;
Sonar runs a plain `dotnet build Binacle.Net.slnx --configuration Release` that the coverage run and the
scanner both need un-containerised. They produce different artifacts for different consumers, and
`shared-image-tests.yml` already runs its own separate `dotnet build` in parallel with both - a third
independent build following the same, already-accepted shape. Caching one job's output for another would mean
uploading and restoring a full solution build across jobs, for a step that costs on the order of a minute -
not worth building.

**Decided (the maintainer, 2026-09-11):** "Approve (Recommended)" - picked in a question prompt, to "Sonar -
replace the hand-written five-minute poll loop with `/d:sonar.qualitygate.wait=true`?"

**Amended 2026-09-12 — the job goes red on a failed gate, and still blocks nothing.** `sonar.qualitygate.wait`
is set in `tooling/ci/sonar-analysis.xml`, so `Sonar end` blocks until SonarCloud has processed the analysis
and exits non-zero when the gate is red. That replaced a hand-written sixty-turn poll in `sonar-summary.sh`
that existed only to wait for the same thing; the summary step now carries `if: always()` so the table is
written either way. The wait is the scanner's own setting, in the file it reads, rather than a loop we timed
ourselves. **"Non-blocking" is unchanged**: the job is still outside `gate`'s `needs`, so a red Sonar is a red
check on the pull request and nothing more. Whether it should become more is still `$ci-cd/decisions/O2`.
