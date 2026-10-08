---
description: The maintainer runs every suite, bench, measure and anything that starts the host. An agent never does.
load: always
when: before running any command that is not a read, a single-project build or a dry run
---

# Never run a long command

**The maintainer runs these. An agent gives him the one-line command and waits.**

- `just test all`, `just test all-with-services`, `just test image`, `just test sites`
- every `just bench <recipe>` - except `just bench <recipe> dry`
- `just measure all`
- `just serve`, `just build image`, `just smoke`, and anything else that starts the host or the API
- any loop or script that stands in for one of the above

**Safe to run:** file reads and greps, `dotnet build`, one project's tests through
`dotnet test --project <one project>`, `just bench <recipe> dry`, `just agents all`, `just changelog`.

**Why:** these runs take the whole machine for minutes to hours, and nothing else can run while they do. An
agent starting one on its own takes that decision away from the only person who knows what else is running.

**How to apply:** build it, run the one project that covers the change, then stop and hand him the command. Do
not run it "just to check", and do not run it because the change looks small. When a result is needed to carry
on, say so and wait.
