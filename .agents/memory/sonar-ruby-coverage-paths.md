---
name: sonar-ruby-coverage-paths
description: sonar.ruby.coverage.reportPaths takes no wildcard and resolves against the ruby/ module, not the repo root - the other two coverage properties beside it do neither
type: gotcha
when: changing a coverage report path in the Sonar settings, or adding a gem
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "tooling/coverage.run.sh"
---

Three coverage properties sit together in `tooling/ci/sonar-analysis.xml` and look interchangeable. Two of
them are. The ruby one differs on both counts:

| | Wildcard | Resolved against |
|---|---|---|
| `sonar.cs.vscoveragexml.reportsPaths` | yes | repo root |
| `sonar.javascript.lcov.reportPaths` | yes | repo root |
| `sonar.ruby.coverage.reportPaths` | **no** | **`ruby/`** |

**No wildcard.** It is documented as a comma-delimited list of paths and nothing more; open on SonarSource's
side since February 2023. Hand it a glob and the sensor looks for a file literally named `*.json`:

```
ERROR: SimpleCov report not found: 'artifacts/coverage/sonar/*.json'
```

So `tooling/coverage.run.sh` merges the ten gem reports into one `ruby.json` at the end of a sonar run. The
gem list stays in `tooling/tests.just`; the merge reads whatever is there.

**Resolved against the module, not the root.** A report path is resolved against the base directory of the
module the sensor runs in. `ruby/ruby.rbproj` makes `ruby/` its own module and it is the only one holding
`.rb` files, so the path has to climb out: `../artifacts/coverage/sonar/ruby.json`. Sonar documents these
paths as relative to the project root. The runs say otherwise.

**Why this cost three runs:** the wildcard failed before the path was ever resolved, so the `../` added on
2026-08-28 could not show whether it helped, and the run that followed still read 0%. That looked like the
`../` being wrong. Fixing only the wildcard on `1fa5ee40` proved both at once - ruby went 0% to 98.8% and the
project 64.3% to 74.9%.

**`sonar.tests` stays unset, and that is deliberate.** The Ruby sensor says so on every run - it falls back
to a path heuristic to tell `spec/` from `lib/`. Setting it globally is not open to us: the .NET half of the
project takes its test/source split from `SonarQubeTestProject` in `Directory.Build.props`, and a global
`sonar.tests` would fight it. The heuristic gets the gems right, so nothing is lost.

**How to apply:** change one of the two variables at a time, or a red run tells you nothing. If ruby reads 0%
again, grep the run for `SimpleCov report not found` - the message prints the path it actually tried, which
is the whole diagnosis. See the memory on Sonar scope exclusions for the neighbouring properties in the same
file.
