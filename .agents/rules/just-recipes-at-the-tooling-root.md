---
description: tooling/ root holds the .just modules and single files named <module>.<name>.<ext>. Three or more files for one module means a folder, and every folder there has a README.
load: on-trigger
when: adding a file under tooling/, or a file a just recipe reads
paths:
  - "tooling/**"
---

# The tooling root is the list of what you can run

**One `.just` file per module at the root**, named after the module: `smoke.just`, `check.just`,
`agents.just`.

**A file a module reads is named after that module.** `<module>.<name>.<ext>` - `check.lychee.toml`,
`serve.services.yml`, `openapi.spectral.yaml`, `tests.ruby-coverage.rb`. The prefix says who owns it, so a
reader never has to open a file to find out which recipe uses it.

**Count the files a module needs, and that decides where they go:**

| Files | Where |
|---|---|
| one | the root, with the `<module>.` prefix |
| two | judgement - keep them at the root unless they are clearly a set |
| three or more | a folder named after the module |

`tooling/smoke/`, `tooling/ci/`, `tooling/agents/` are folders because each holds three or more.

**Inside a folder, a name does not repeat the folder.** `tooling/agents/indexes.toml`, not
`agents-indexes.toml`. `tooling/smoke/structure.yaml`, not `smoke-structure.yaml`.

**Every folder under `tooling/` has a `README.md`** - what it is, a table of what is in it, how to run it,
and what will bite you. The gitignored ones a tool creates for itself are the exception: `bin/`, `obj/`,
`azurite/`, `data/`.

**Why:** so the root of `tooling/` reads as the list of things you can run, and nothing else.
