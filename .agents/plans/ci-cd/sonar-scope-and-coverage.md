---
description: The gems are indexed and Sonar analysed 99 .rb files, but the coverage import still reads 0% - the report path took a wildcard the ruby setting does not support. Fixed by merging the ten reports into one file; one run confirms it, and answers the open question about the leading ../ at the same time.
state: blocked
waits-on: "one Sonar run on the merged ruby.json now in the tree"
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "tooling/coverage.run.sh"
  - "ruby/**"
  - "Binacle.Net.slnx"
---

# Getting the gems into Sonar

**`ruby/ruby.csproj` worked.** The run of 2026-08-27 23:33 detected **eleven** languages where the one before
it detected ten, `Sensor Ruby Sensor [ruby]` analysed 99 source files, and `ruby=2967` is in the language
breakdown. Listing the `.rb` files in a project that actually builds is what the scanner needed.

**Why the solution declares an `rbproj` type based on `C#`, and why `Shared` cannot work, is in the CI/CD
decisions ledger.** In short: a `.slnx` infers nothing from `.proj` and refuses a project whose type it cannot
work out, so the extension is declared once in `Binacle.Net.slnx`; it is based on `C#` because `Shared`
projects are not built by the solution and `IsBuildable` does not bridge the two.

## The coverage import failed on the wildcard

```
Sensor SimpleCov Sensor for Ruby coverage [ruby] (done) | time=1ms
ERROR: SimpleCov report not found: 'artifacts/coverage/sonar/*.json'
```

**`sonar.ruby.coverage.reportPaths` takes no wildcard.** It is the only one of Sonar's three coverage
settings that does not - `sonar.cs.vscoveragexml.reportsPaths` and `sonar.javascript.lcov.reportPaths` sit
beside it in the same file and glob fine, which is why C# and javascript import and this one never did. Sonar
documents it as a comma-delimited list of paths, absolute or relative to the project root, and says nothing
about globs. Open on their side since February 2023. The sensor went looking for a file literally named
`*.json`, did not find one, and printed the pattern back.

**Fixed on 2026-08-31 by merging the ten gem reports into one `ruby.json`**, so there is nothing left to
match. `tooling/coverage.run.sh` does it with `jq` at the end of a sonar run - every `.json` in the folder is
a simplecov report, C# writes `.xml` there and jest writes `.info`, and a path appears in exactly one gem's
report so the coverage maps merge as a plain union. **The gem list stays in `tooling/tests.just` and nowhere
else.** Measured on the merged file: 67 files, 1022 of 1034 lines, 98.8% - the same total as the ten
separately, nothing lost.

## The leading `../` is still unproven

The path is `../artifacts/coverage/sonar/ruby.json`, and the `../` is a second, separate guess.

**The theory:** a report path is resolved against the base directory of the module the sensor runs in, and
the only module holding `.rb` files is `ruby/` - its own project, base dir `<repo>/ruby`. The C# and
javascript sensors run in the root module, where the same shape of path means the repository root.

**It has never actually been tested.** The wildcard failed first, so the `../` added on 2026-08-28 could not
have shown whether it helped. The run of 2026-08-31 on `fab50ba9` carried it and `ruby` still read
`coverage 0.0, lines_to_cover 749, uncovered_lines 749`.

**One run settles both.** If the gems report their coverage, the wildcard was the whole problem and the `../`
was right. If it still reads 0%, drop the `../` and dispatch again - the file name is the part that is
certain. Sonar documents these paths as relative to the project root, which is the argument for dropping it.

## The specs were counting as uncovered product code

Indexing the gems added 1506 lines to the coverage denominator and 1506 uncovered lines, dropping the project
from 71.1% to **58.4%** and new-code coverage from 77.0% to 26.3%.

Roughly half of `ruby/` is rspec - 2016 lines of spec against 1974 of lib. **SimpleCov is configured to
`skip '/spec/'`**, so it reports nothing for them and they would sit at 0% forever. `**/spec/**` is now in
`sonar.coverage.exclusions`, beside the `**/tests/**` that does the same job for the typescript suites.
`spec/` exists nowhere outside `ruby/`, so the glob needs no qualifying.

With both changes the ten gems should land at the 96-100% their own runs report.

## What the gems arrived with

**27 new high-severity findings, every one `ruby:S1192`** - a duplicated string literal in a spec file, like
`"site.webmanifest"` five times in `jekyll-webmanifest/spec/generator_spec.rb`. Sonar rates S1192 high for
Ruby where it rates it low for C#, which is why 27 test-code findings outrank everything else on the list.
They are the same class as the .NET test findings and the same answer applies: they stay visible.
`plans/sonar-issue-triage.md` owns the triage.

**`sonar.tests` is not set**, and the Ruby sensor said so: it fell back to a path heuristic to tell spec from
lib. Setting it globally is not open to us - the .NET half of the project gets its test/source split from
`SonarQubeTestProject`, and a global `sonar.tests` would fight it.

## Done when

- [ ] The ten gems report their real coverage.
      **By eye**, on the Code page under `ruby/` - it read 0% over 749 lines on 2026-08-31, and their own runs
      say 98.8%. Zero again means the import still is not landing; grep the run for
      `SimpleCov report not found`, and if it is there, drop the `../` and dispatch once more.
- [ ] Overall coverage is back above where it was before the gems were indexed.
      71.1% is the number to beat, from 64.3% on 2026-08-31. Below it means the specs are still in the
      denominator.
