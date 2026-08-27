---
description: Confirmed on the 2026-08-27 run - no Ruby analyser loads, so the ten gems can never report coverage. ruby/ruby.csproj is the one cheap experiment left, and it is in the tree waiting for a run.
state: proposed
waits-on: "one Sonar run. The experiment is built; the log says whether it worked"
paths:
  - "tooling/ci/sonar-analysis.xml"
  - ".github/workflows/sonar-analysis.yml"
  - "ruby/**"
  - "Binacle.Net.slnx"
---

# Sonar sees no Ruby, and now we know why

**Settled by the `ad2e96b8` run of 2026-08-27.** The scanner log is the evidence, not the dashboard.

```
INFO: 10 languages detected in 1322 preprocessed files
INFO: Loading plugins for detected languages
```

**Ruby is not one of the ten**, and no `Sensor Ruby` line appears anywhere in the run. The plugin list is
loaded *after* detection, from the languages found - so the `.rb` files were walked, claimed by nothing, and
dropped. The language breakdown that reached SonarCloud is `cs, css, docker, js, py, shell, ts, web, yaml`.

An earlier version of this plan blamed MSBuild project membership. That was wrong and is disproved twice
over: before `ruby/vendor/**` was excluded, 240 files from that same folder were indexed - `.html`, `.erb`,
`.yml`, `.sh` - none of them in any project either.

**The coverage half was never the problem and is finished.** All ten gems produce a SimpleCov JSON report
with absolute repository paths, which is exactly what `sonar.ruby.coverage.reportPaths` wants. Verified by
running one by hand, and by the ten `JSON Coverage report generated for RSpec` lines in the same run.
There is nothing to build. There is nothing for the reports to attach to.

## The experiment is built: `ruby/ruby.csproj`

`Microsoft.Build.NoTargets`, `IsPackable=false`, `Content Include="**\*.rb"` excluding `vendor/**` - 102
files, none of them vendored. In `Binacle.Net.slnx`. If listing the files in a built project is what makes the
scanner offer them to the language detector, this is the whole fix.

**It is `.csproj` and not `.proj`, and that is the finding underneath it.** The solution file cannot infer a
project type from `.proj`, so every content project here is declared `Type="Shared"` - and a Shared project is
**never built by `dotnet build Binacle.Net.slnx`**. `tooling/obj` and `assets/obj` are stale from 6 Aug 2026
and CI has never recreated them. So none of those projects has ever reached the scanner, and a `ruby.proj`
copied from `tooling.proj` would have been a guaranteed no-op.

That also settles where `tooling/`'s `.sh` and `.py` come from: not from `tooling.proj`, which never builds,
but from the scanner's own walk of the repository root. The same walk reaches `ruby/` - it is what indexed 240
files under `ruby/vendor` before that folder was excluded. **It sees the `.rb` files and does not claim
them.**

**A second Sonar project is the fallback, and it is a real decision.** A standalone `sonar-scanner` pass over
`ruby/` cannot write into `binacle-labs_Binacle.Net` - one project takes one analysis - so it means a second
project key, a second dashboard, and a second gate. Ten gems at 96-100% coverage would look good on it. It
also doubles the thing anyone has to look at.

**Doing neither is defensible.** The gems have their own suites, all ten pass in CI, and their coverage is
printed by `just coverage table`. What is lost is a number on a dashboard. What is not lost is the testing.
**If that is the answer, say so and delete the `sonar.ruby.coverage.reportPaths` line**, because a setting
that cannot work is worse than no setting - it is what made three weeks of this look like an import bug.

## Done when

- [ ] One run says whether the Ruby plugin loads now that `ruby/ruby.csproj` is in the build.
      Grep the `end` log for `Quality profile for ruby`. The run of 2026-08-27 listed ten and ruby was not
      among them; eleven with ruby means it worked.
- [ ] If it cannot work here, the choice between a second project and dropping it is written into the CI/CD
      decisions ledger, and the coverage property matches that choice.
      **By eye.** A `sonar.ruby.coverage.reportPaths` line with no Ruby analyser behind it is the failure.
