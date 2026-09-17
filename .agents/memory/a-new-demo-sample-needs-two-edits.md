---
name: a-new-demo-sample-needs-two-edits
description: A new file in shared/data/demo-samples reaches the demo on its own, but the ViPaq packed data only if you also add it to the generator's hardcoded list - which fails silently
type: gotcha
when: adding or renaming a file in shared/data/demo-samples
paths:
  - "shared/data/demo-samples/**"
  - "vipaq/tools/Binacle.ViPaq.PackedDataGenerator/**"
  - "vipaq/data/packed/**"
---

Two generators read `shared/data/demo-samples/`, and they find their input in different ways:

- `just regen demo-samples` walks the folder. A new file is picked up with no other edit.
- `just regen vipaq-packed-data` does **not**. `Binacle.ViPaq.PackedDataGenerator/Program.cs` carries the
  demo-sample file names as a hardcoded list, and packs only what is in it.

**So the second one fails silently.** A file the list does not name is skipped, the recipe exits 0, and
`vipaq/data/packed/demo-samples/` simply has no entry for it. Nothing red, nothing said. It looks exactly
like a recipe that has not been run yet.

This bit on 2026-09-16: `00-two-winners.json` had been in the folder for a day, `just regen demo-samples`
had picked it up, and `just regen vipaq-packed-data` would have written nothing however often it ran.

**Why the list is hand-written:** the generator takes no arguments and cannot half-run, which is what makes
its output reproducible. Enumerating the folder instead is a real change to that design, not a tidy-up.

**How to apply:** add the file name to the list in `Program.cs` in the same commit as the sample file. Then
check the output, not the exit code - `ls vipaq/data/packed/demo-samples | grep '^<prefix>'` returns one file
per algorithm, three today.
