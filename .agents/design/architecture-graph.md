---
id: architecture-graph
description: Why the repo's dependency graph is generated rather than declared, what no generator can see, what an InternalsVisibleTo grant means for the graph, and the heavier architecture tools that were surveyed and not taken.
verified: 2026-08-27
check: The InternalsVisibleTo rule against `grep -rn 'InternalsVisibleTo' --include=*.csproj .` - every grant must name an assembly that references the granter; the ArchUnitNET xunit caveat against the runner pin in Directory.Packages.props; the claim that no root tsconfig.json exists
paths:
  - "**/*.csproj"
  - "Directory.Packages.props"
---

# The dependency graph - why it is derived, not declared

Why any architecture check in this repository generates the graph and lints it, rather than comparing the tree
against a file someone wrote. What such a check would *do* is a plan; this is the reasoning behind its shape,
so it is not re-litigated.

## A hand-written declaration was built, run green, and reverted

A root `architecture.yml` stated the shape and **nothing read it.** It was a second copy of the truth, it
could drift from the tree in silence, and it did - twice. Deleted 2026-08-25 at the maintainer's call.

What that cost is worth keeping:

- **The compiler already enforces the project graph.** A reference that does not exist cannot be used. No
  check can *prevent* an edge; it can only notice one was added. **Knowing which of those two a check is
  changes how it should be built.**
- **A hand-written file drifts in two directions.** Reality moves away from it, and it moves away from
  reality. Reconciling those needs shorthand rules, resolution rules and carve-outs - and **every line of that
  is a place the check can pass for the wrong reason.** A silently green check is worse than no check.
- **A generated graph cannot be wrong.** It is derived on the spot from the project files, so there is no
  second copy to keep in step.
- **The rules are not the declaration.** A ruleset asserts; it is never compared against anything. That is why
  it can be exhaustive at a fraction of the cost - no reconciliation layer, no shorthand to resolve, no
  entries naming things no tool can see.

**The readable statement of intent belongs in the ruleset, not beside it.** The deleted file tried to be both
a human-readable declaration and a thing tools would one day check. It was neither: written twice, read never.

## Never abbreviate architecture to "arch"

Recipes, folders, files, job names, output paths - spell it out. **`arch` already means CPU architecture
here**, and there is a plan to publish images for a second one. A name that means two things sends someone to
the wrong file.

## What no generator will read

The .NET project references and the npm workspace packages are the easy half and cover most of the code. These
edges exist and no reference audit sees them:

- `docs` and `demo` on `ruby` - Gemfile `path:` gems.
- `demo` and `api` on `packages` and `vipaq` - webpack chunk regexes, one config each.
- `tooling` on everything - path strings inside `just` recipes.
- `assets` on `docs`, `demo` and `api` - a gulp copy into each.
- `vipaq/tools` on `shared/data` - a path resolved at run time.

**Anything that reports on this graph has to name these in its output.** Silence about them is how a green run
gets read as total coverage.

## An `InternalsVisibleTo` grant annotates an edge; it never adds one

Settled 2026-08-13. When `A` grants `InternalsVisibleTo(B)`, nothing in `A` resolves `B` - `A` compiles fine
if `B` does not exist. The grant records that **`B` depends on `A`**, more deeply than usual: on internals
rather than on the public API. So the dependency runs `B -> A`, the same direction `B`'s own reference already
points.

**The rule that falls out:** every grant must name an assembly that references the granter, directly or
transitively. One that does not is dead weight - it grants access nobody can take. It has already found one, a
grant for internals the named test project never touched, since deleted.

**Expand `$(ProjectName)` before comparing.** Most grants are written that way, and one expands in a way a
suffix rule would not guess: `Binacle.Net` grants `$(ProjectName).ServiceModule.IntegrationTests`.

**It is tidiness, not architecture.** Nothing breaks when a dead grant stays.

## A bare slice name means that slice's `src`

Without that convention the graph reads as cyclic - `shared/test -> lib` against `lib/src -> shared`. It is a
fact about how the graph is read, not about the generator, but whatever reports on it needs to know.

## The heavier tools, surveyed 2026-08-17 and not taken

| Tool | Version surveyed | How it would read a graph file |
|---|---|---|
| `TngTech.ArchUnitNET` + `.xUnitV3` | 0.13.3 | rules are runtime objects, so it reads YAML directly |
| `dependency-cruiser` | 18.2.0 | reads the YAML from its `.cjs` config |

Four things to settle before adopting ArchUnitNET, and the first decides whether it is an afternoon or a week:

- **Check `.xUnitV3`'s transitive xunit dependency.** This repo pins `xunit.v3.mtp-v2` precisely because
  mixing the MTP v1 and v2 adapters throws `TypeLoadException` before a test runs. If `.xUnitV3` pulls plain
  `xunit.v3`, the new test reproduces it.
- **It collides with work that grows the shared TestsKernel fixtures.** Both touch the tests, so
  whichever runs second reads the other's result.
- **Decide which graph is authoritative.** ArchUnitNET measures *type* dependencies from loaded assemblies; a
  derived graph comes from project references. They disagree - `api/src/Binacle.Net/Binacle.Net.csproj`
  declares `<Using Include="Binacle.Geometry" />` with no reference to it.
- **Its test project must reference every slice it inspects**, becoming a node with an edge to everything.

For `dependency-cruiser`, reading a graph file is the easy half. **There is no root `tsconfig.json`** - there
are nine, one per workspace - and imports are bare specifiers resolved
through npm workspace symlinks, so rules must be written against resolved real paths with symlink handling
pinned.

## What no tool here will ever check

The internal layer rules for the agent guidance - permanent files never pointing at ephemeral ones, docs
referencing only docs - are graph-shaped over the `$` reference scheme. A regex can ban a pattern within a
path; it cannot check that a `$` reference resolves to a file in an allowed layer. **Either it stays
prose-only, or it is the one place a small custom check is worth owning.**
