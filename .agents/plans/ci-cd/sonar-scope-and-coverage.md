---
description: No .rb file has ever been indexed, and it is not because ruby/ is in no MSBuild project - files from the same folders are indexed. Two hypotheses left and one run that tells them apart.
state: proposed
waits-on: "nothing. One Sonar run with the check below settles which of the two fixes is the right one"
paths:
  - "tooling/ci/sonar-analysis.xml"
  - ".github/workflows/sonar-analysis.yml"
  - "ruby/**"
  - "Binacle.Net.slnx"
---

# Why Sonar sees no Ruby

**Measured on the 2026-08-27 run, through the SonarCloud API.** The project holds 1000 files. Not one of them
is a `.rb`. Neither our 102 gem sources nor the thousands under `ruby/vendor` that CI installs. The language
breakdown has no `ruby` entry at all, and the project has never had a Ruby quality profile applied to it -
even though the profile exists and carries 67 rules.

Coverage for a file Sonar does not know about is dropped in silence, which is why the ten gems read zero.

## The earlier diagnosis is wrong

**It said: `ruby/` is in no MSBuild project, the scanner takes its file list from the build, so the `.rb`
files are never indexed. The fix was a `ruby/ruby.proj`.**

That cannot be the mechanism. From inside `ruby/vendor/bundle/`, which is in no project either, the run
indexed 240 files - kramdown's `.html` fixtures, jekyll's `.erb` templates, `safe_yaml`'s `.sh`, and four
gems' own `.yml` workflows. `.github/` is in no project and its 20 yaml files are indexed.

**Same folder, `.html` in and `.rb` out.** No path-based cause survives that. Whatever decides this is the
language, not the project.

## Two hypotheses, and the run that separates them

**A - the Ruby sensor does not run under the Scanner for .NET.** The walk that picks up loose files covers
web, yaml and shell but never invokes the Ruby analyser. If this is it, a `ruby.proj` changes nothing and the
fix is a second standalone `sonar-scanner` pass over `ruby/`, or dropping the ruby coverage line as a thing
that cannot work here.

**B - the walk only claims files a project does not, for a subset of languages, and membership fixes it.**
Sonar's own documentation says loose files are analysed "unless explicitly excluded" for SDK-style projects,
and that other file types are enabled by listing them in the project file. If this is it, `ruby/ruby.proj`
works - `Microsoft.Build.NoTargets`, `IsPackable=false`, `Content Include="**\*.rb"`, the same shape as
`tooling/tooling.proj`, added to `Binacle.Net.slnx`.

**It must exclude `vendor/**` either way**, or a bare glob sweeps thousands of other people's gems into the
build.

The evidence does not favour one. `tooling/`'s `.sh` and `.py` are in a project *and* would be caught by the
walk, so they prove nothing.

## The check that decides it, and it is one run

`dotnet-sonarscanner begin` writes `.sonarqube/out/`, and every project in the build gets a folder holding a
`ProjectInfo.xml` and a `FilesToAnalyze.txt`. **That list is the truth.** Grep it for a `.rb` path.

- A `.rb` path is there and SonarCloud still shows no Ruby - **hypothesis A**, the sensor never ran.
- No `.rb` path is there - **hypothesis B**, and adding the project is what puts it there.

`end` also reports how many files each analyser picked up. A language with zero files is a scope problem,
never a coverage problem. If the files appear and the coverage still reads zero, one run with
`sonar.verbose=true` names every report it parsed and every path in one it could not match - it is the only
thing that separates "the report was not read" from "the report was read and matched nothing". Turn it off
again; it is loud.

**Read them in that order.** Working backwards from the dashboard is how three weeks concluded the coverage
import was broken, when the files were never there to cover.

## What this does not settle

**Whether SonarCloud imports the simplecov json** stays unproven until the files are indexed. The report paths
are right - checked 2026-08-28, the json records absolute paths under the repository root, which is what the
importer wants.

**102 files arriving at once will raise new findings on a rule set that has never seen them.** That is triage,
not a defect, and `plans/sonar-issue-triage.md` owns it.

## Done when

- [ ] A `.rb` path either is or is not in a `FilesToAnalyze.txt` under `.sonarqube/out/`, and which one is
      written into the CI/CD decisions ledger along with the fix it picks.
      **By eye**, after a local `begin` + `build`. No token needed for the grep.
- [ ] The gems are not at zero in SonarCloud.
      **By eye**, on the Code page, after a run.
- [ ] If they are still at zero, one run with `sonar.verbose=true` says why, and the answer joins the ledger.
