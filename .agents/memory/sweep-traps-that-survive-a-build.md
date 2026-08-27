---
name: sweep-traps-that-survive-a-build
description: Two edits that pass a full build and ten thousand tests while being wrong - prepending a line ahead of a BOM, and removing one redundant null-conditional
type: gotcha
when: doing a mechanical sweep across many C# files - inserting a using, removing a redundant operator
paths: ["**/*.cs"]
---

Two ways a mechanical sweep produces a wrong tree that builds. Both were hit for real during the 2026-08-09
Sonar pass.

**A script that prepends a line to a file relocates the BOM instead of preserving it.** Inserting
`using System.Net.Mime;` ahead of a BOM-carrying first line left a stray `U+FEFF` stranded at the start of
line 2 in sixteen files. It survived a full build and 10,041 tests, **and no BOM tool would find it**, because
position 0 was no longer a BOM. Write after the BOM, not before it.

**Removing one redundant `?.` can introduce `CS8602`.** Dropping `context?.` on one line while leaving it on
another left flow analysis still treating `context` as possibly-null, so the bare dereference warned. **It did
not show in an incremental Debug build** - only `--no-incremental -c Release` surfaced it, which is what CI
runs.

**Why:** both fail past the compiler on the machine that made the edit, so the sweep looks done.

**How to apply:** after a prepend sweep, check byte 0 of every touched file. After a null-check sweep, verify
the warning count with a clean Release build, never an incremental one.
