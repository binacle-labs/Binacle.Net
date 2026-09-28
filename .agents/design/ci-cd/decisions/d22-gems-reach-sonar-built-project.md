---
id: ci-cd/decisions/D22
description: the gems reach Sonar through a built project, and the project type is what decides that
status: pending
verified: 2026-09-29
check: D22 against Binacle.Net.slnx, whose rbproj ProjectType must stay BasedOn C# and not Shared, and against tooling/ci/sonar-analysis.xml, whose ruby coverage path must stay relative to ruby/
paths:
  - "ruby/ruby.rbproj"
  - "Binacle.Net.slnx"
  - "tooling/ci/sonar-analysis.xml"
---

# D22 — the gems reach Sonar through a built project, and the project type is what decides that

`ruby/ruby.rbproj` exists for one reason: to list the gem sources as `Content` so the scanner is offered
them. `Microsoft.Build.NoTargets`, so it compiles nothing. It excludes `vendor/**`, the bundle CI installs.

**Without it Sonar saw no Ruby at all.** The run of 2026-08-27 23:08 reported `10 languages detected` and no
`Sensor Ruby` line anywhere; the run after it, with the project, reported `11` and analysed 99 `.rb` files.
The scanner's own walk of the repository root reaches `ruby/` — it indexed 240 files under `ruby/vendor`
before that folder was excluded — so it sees the `.rb` files and does not claim them. Being listed in a built
project is what makes the difference.

**The extension is declared in `Binacle.Net.slnx`, so the project entry carries no `Type`:**

```xml
<ProjectType Name="Ruby" Extension="rbproj" BasedOn="C#" />
```

A `.slnx` rejects any project whose type it cannot infer — `ProjectType '' not found` makes the whole solution
unloadable — and it infers nothing from `.proj`. Declaring the extension answers that once. The mapping is
scoped to `rbproj`, so the `Type="Shared"` content projects are untouched.

**It is based on `C#` and not on `Shared`, and `IsBuildable` does not bridge the two.** Measured on
2026-08-28, each from a deleted `ruby/obj` and a full `dotnet build Binacle.Net.slnx`:

| Declaration | Built by the solution |
|---|---|
| `.csproj`, no type | yes |
| `.proj`, `Type="C#"` | yes |
| `.rbproj`, `BasedOn="C#"` | yes |
| `.rbproj`, `BasedOn="Shared" IsBuildable="true"` | **no** |

`IsBuildable="true"` on a type based on `Shared` does not make it build. An unbuilt project never reaches the
scanner, which is this one's whole job — which is also why `tooling/obj` and `assets/obj` are stale from
6 Aug 2026 and none of those projects has ever been analysed.

**The consequence for the others is worth knowing.** `tooling.proj`, `assets.proj`, the site projects and the
other content projects are all `Type="Shared"`, so none of them has ever been built or appeared as a Sonar module. Their
files are analysed anyway, through the scanner's root walk — which is why the `.sh` and `.py` findings exist
while the projects that list them do not.

**The type names are not documented, so they were tested.** Accepted: `C#`, `VB`, `F#`, `Website`, `Shared`,
`WiX`, `Docker`, `Folder`, `JavaScript`, and a raw GUID. Rejected: `Classic C#`, `MSBuild`, `Web`,
`Solution Folder`, `Python`, `Node`, `TypeScript`, `Custom`, `None`.

**The scanner does not care what the file is called.** Its targets pick files by MSBuild item type -
`SQAnalysisFileItemTypes` names `Content` among others - and the only projects it skips are Microsoft Fakes
projects, `_wpftmp`/`WorkerExtensions` temp projects, and anything setting `SonarQubeExclude`. Nothing keys
off the extension, and `ProjectLanguage` comes from `$(Language)`, which is empty for a NoTargets project
whatever it is named. So the extension is a readability choice, not a functional one.

**One knock-on: the ruby coverage path is the odd one out.** A report path is resolved against the base
directory of the module the sensor runs in. The C# and javascript sensors run in the root module; the
SimpleCov sensor runs in `ruby/`, this project's folder, and said so —
`SimpleCov report not found: 'artifacts/coverage/sonar/*.json'`. So that one property opens with `../` while
the other two do not. Every report is still written to `artifacts/coverage/sonar`.
