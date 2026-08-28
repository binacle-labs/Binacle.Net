---
description: The gems are indexed - ruby/ruby.csproj worked and Sonar analysed 99 .rb files. What is left is the coverage import, which failed on a path resolved against the wrong directory, and the 27 findings the gems arrived with.
state: ready
waits-on: "one Sonar run to confirm the two property changes now in the tree"
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "ruby/**"
  - "Binacle.Net.slnx"
---

# Getting the gems into Sonar

**`ruby/ruby.csproj` worked.** The run of 2026-08-27 23:33 detected **eleven** languages where the one before
it detected ten, `Sensor Ruby Sensor [ruby]` analysed 99 source files, and `ruby=2967` is in the language
breakdown. Listing the `.rb` files in a project that actually builds is what the scanner needed.

**Why the solution declares an `rbproj` type based on `C#`, and why `Shared` cannot work, is `$ci-cd/decisions` D22.**

## The coverage import failed, and the run named the reason

```
Sensor SimpleCov Sensor for Ruby coverage [ruby] (done) | time=1ms
ERROR: SimpleCov report not found: 'artifacts/coverage/sonar/*.json'
```

**A report path is resolved against the base directory of the module the sensor runs in**, and the only
module holding `.rb` files is `ruby/` - its own project, base dir `<repo>/ruby`. So it looked for
`ruby/artifacts/coverage/sonar/`. The C# and javascript imports work because their sensors run in the root
module, where the same shape of path means the repository root.

**Fixed by writing that one path as `../artifacts/coverage/sonar/*.json`.** The reports have not moved -
all three are in `artifacts/coverage/sonar`. Only the route to them differs, and the settings file says why.

Sonar documents these paths as relative to the project root. The run showed otherwise, and the run wins.

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
      **By eye**, on the Code page under `ruby/`. Zero means the import still is not landing; grep the run for
      `SimpleCov report not found`.
- [ ] Overall coverage is back above where it was before the gems were indexed.
      71.1% is the number to beat. Below it means the specs are still in the denominator.
