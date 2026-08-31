---
description: Generate the repo's dependency graph, draw it, lint it - plus three greps over api/src, and the one boundary violation to fix first
state: idea
waits-on: "a maybe"
paths:
  - "**/*.csproj"
  - "tooling/**"
---

# Architecture checks

Why the graph is generated rather than declared, what no generator can see, and the heavier tools that were
surveyed and dropped are all in the architecture-graph design record. This is what is left to build.

**One generator, two readers.** Walk the project files, derive the graph, write it to `artifacts/architecture/`
which is untracked. Read it as a **Mermaid diagram** (a viewer draws it from text, nothing to install) and as a
**Spectral ruleset** - Spectral is not an OpenAPI tool, it is already here, no new toolchain. Nothing it writes
is committed, so there is no second copy to fall behind. Write the ruleset **exhaustively**, one rule per slice
naming what it may reference, transcribed from the graph as it stands - a new dependency then costs one edit,
which is the friction you want.

**Traps:**

- **Assert every derived list is non-empty.** A generator that finds no projects passes over nothing and
  reports clean forever. This is the most common way a check like this dies.
- **Break a rule on purpose and read the message.** A rule never seen to fail has not been shown to work.
- **No `xargs`** - exit 123 on an empty batch, and six algorithm folders have spaces in their names.
- **Exclude `obj/`** from the api greps. Generated assembly info names all three modules, so a naive grep is
  red forever for no reason. **Derive the module list** on the segment ending in `Module`.

**Fix the violation before building the check that watches it.** `Services/BinacleService.cs:5` and
`ExtensionMethods/LogChannelExtensions.cs:3` both `using Binacle.Net.DiagnosticsModule.Logs.Models;` for one
type. The real problem is the core doing diagnostics work. Cheap fix: move the request record into `Kernel`,
half an hour. Right fix: `Kernel` declares an observer contract, DiagnosticsModule implements it and owns
building the log line, and `LogChannelExtensions.cs` leaves the composition root. A mistake made impossible
beats a mistake detected.
