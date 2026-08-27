---
description: Generate the repo's dependency graph, draw it, and lint it with a small ruleset - plus three greps over api/src, and the one boundary violation to fix before building the check that watches it
state: idea
waits-on: "v3.0.0 at the earliest, and a maybe even then"
paths:
  - "**/*.csproj"
  - "tooling/**"
---

# Architecture checks

**Why the graph is generated rather than declared, what no generator can see, and the heavier tools that were
surveyed and dropped are all settled** - the architecture-graph design record carries them. This file is only
what is left to build.

## The goal, in order

1. **Something a person can look at to understand this repo's shape.** A picture, kept current by a machine.
2. **A small set of rules that fail loudly when the shape moves in a way that matters.**
3. **A record of what no tool will ever check**, so a green run is not read as "all of this is covered".

## One generator, two readers

Walk the project files, derive the graph, write it out. Then read it two ways.

- **A Mermaid diagram.** Nothing to install and nothing to host - a viewer draws it from text. This is goal 1
  and the cheapest part of the whole plan.
- **A Spectral ruleset**, the linter this repo already runs on the OpenAPI documents. Spectral is not an
  OpenAPI tool; those rules are a ruleset that ships with it. **No new dependency, no new toolchain.**

**Nothing it writes is committed.** Everything goes to `artifacts/architecture/`, a declared sink that is not
tracked. CI runs the generator, lints the output and throws it away, so there is no second copy to fall behind
and `regen` stays uncalled from CI. **The cost is that the diagram is not browsable** - if linking someone at
the picture matters, publish it as a CI build artifact rather than committing it.

**Spell out "architecture" everywhere** - recipes, folders, files, job names, output paths. The reason is in
the design record.

### Write the ruleset exhaustively

Every slice gets a rule naming exactly what it may reference. About four lines each, so thirteen slices is
roughly sixty lines. Adding a legitimate new dependency then costs one edit to one rule - **which is the
friction you want.**

**Write them from the generated graph as it stands**, or the whole set lands red on day one. The graph is
already known to be clean, so this is transcription, not investigation.

```yaml
rules:
  shared-references-nothing:
    given: $.slices.shared.*
    then: { function: length, functionOptions: { max: 0 } }
    severity: error

  lib-only-references-shared:
    given: $.slices.lib.*[*]
    then: { function: enumeration, functionOptions: { values: [shared] } }
    severity: error
```

Spectral's built-in functions cover all of it, no custom code. A rule is one readable thing and its failure
message names it - not "an undeclared edge appeared" but "lib may only reference shared". Severity is built
in, so a rule that cannot be made true today lands as a warning and is tightened later.

### Traps

- **Assert every derived list is non-empty.** A generator that finds no projects writes an empty file, the
  ruleset passes over nothing, and the check reports clean forever. **This is the most common way a check like
  this dies.**
- **Test the checks by breaking things on purpose.** A rule that has never been seen to fail has not been
  shown to work. The reverted build did this and it was the best part of it.
- **Do not reach for `xargs`.** Exit code 123 when a batch matches nothing, and six algorithm folders whose
  names contain spaces. `grep` over a directory has neither problem.
- **Three sites under `sites/`, not two**, and the slice names differ between the agent guidance and disk -
  `.github` is `ci-cd` there. How a `sites/` slice is written is still open.

## The api check - three greps, and it is a different kind of check

**The slice ruleset cannot express this one and never will.** It reads project references; these rules are
about which *types* a file names, and the reference they would flag is legitimate and declared. So it is a
separate check scoped to `api/src/` alone.

1. **Only `Program.cs` may name a module**, within `api/src/Binacle.Net/`. **Red today** - see below.
2. **No module may name another module.** Green today, at project and type level.
3. **`Kernel` may name no module.** Green today.

**Exclude `obj/`.** The generated assembly info under `api/src/Binacle.Net/obj/` names all three modules in
`ApplicationPartAttribute` and `InternalsVisibleTo` lines. A naive grep returns eight files, six of them build
output, and the check is red forever for no reason.

**Derive the module list; do not type it out.** Group it on the segment ending in `Module`, so the ServiceModule
collapse cannot make rule 2 read `.Domain` as a different module and go red on a legitimate reference.

### Fix the violation before building the check that watches it

`Services/BinacleService.cs:5` and `ExtensionMethods/LogChannelExtensions.cs:3` both carry
`using Binacle.Net.DiagnosticsModule.Logs.Models;`, and both need exactly one type from it:
`AlgorithmOperationLogChannelRequest`.

**The real problem is that the core is doing diagnostics work.** `BinacleService` packs, then hands its bins,
items and results to an extension method that builds a log request and enqueues it - and that extension sits
in the composition root, which is log-channel plumbing in the wiring project.

- **Cheap.** Move the request and its `PackingLogEntry` record into `Kernel`. Two `using` lines change, Kernel
  gains a `Binacle.Packing` reference, about half an hour. **It also moves a type into Kernel that the generic
  mechanism says should not be there** - `AddLogProcessor<TChannelRequest, TLog>` is fully generic and the
  module supplies the concrete types in its own `AddOptionsBasedPackingLogProcessor`. That split is already
  correct.
- **Right.** Kernel declares an observer contract, the core calls it with its own types, and DiagnosticsModule
  implements it and owns the request-building. **`LogChannelExtensions.cs` leaves the composition root
  entirely**, because building a packing log line is a diagnostics feature.

**A mistake made impossible beats a mistake detected.** The check is still worth having afterwards - the rule
is permanent - but do not build it to watch a violation standing there waiting to be moved.

## One loose end the audit found

`binacle-compact-notation` moves from `dependencies` to `devDependencies` in
`vipaq/packages/binacle-vipaq/package.json`. Not an illegal edge, and no check would have caught it.

## Done when

- [ ] A person can look at a current picture of the repo's shape without reading a project file.
      `just architecture` (or whatever it is called) writes a Mermaid graph into `artifacts/architecture/`.
- [ ] Every slice has a rule naming what it may reference, and the set runs green on the tree as it stands.
      Spectral over the generated graph exits 0.
- [ ] Each rule has been seen to fail.
      **By eye.** Break one edge on purpose, run the check, and read the message it prints.
- [ ] The generator refuses to report clean on an empty graph.
      Point it at an empty directory; it exits non-zero.
- [ ] What the generator cannot see is named in its output.
      **By eye.** The five undeclared edges appear in what it prints, not only in the design record.
- [ ] `api/src/Binacle.Net` names a module in `Program.cs` and nowhere else.
      `grep -rn "DiagnosticsModule\|ServiceModule\|UIModule" api/src/Binacle.Net --include=*.cs` excluding
      `obj/` hits `Program.cs` only.
