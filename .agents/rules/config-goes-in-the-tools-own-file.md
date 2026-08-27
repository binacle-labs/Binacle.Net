---
description: If a tool reads a config file, use it. Do not invent a config format the code has to parse itself.
load: on-trigger
when: passing settings to a tool, or writing a table or list inside a script, recipe or workflow
paths:
  - "**/*.just"
  - "**/*.sh"
  - "**/*.yml"
---

# Config goes in the tool's own file

**If the tool reads a config file, use it.** lychee reads `tooling/check.lychee.toml`, spectral reads
`tooling/openapi.spectral.yaml`, container-structure-test reads `tooling/smoke/structure.yaml`, compose reads
a compose file, jest reads `jest.config.js`. Settings for that tool live there, not spelled out as flags in a
recipe.

**Do not invent one.** A config file that the code has to parse itself is not a config file, it is a second
program. Our own tables and lists move out only when whatever reads them parses that format natively - a
bash reader that needs a yaml parser is worse than the array it replaced.

**A recipe nothing in CI calls may use any tool that reads well.** `just agents` runs on a laptop only, so it
can reach for python, or anything else with a real parser, and put its tables in a yaml file beside it. A
recipe CI calls pays a setup step for every tool it needs, so that one stays cheap.

**Why:** a setting in the tool's own file is found by anyone who knows the tool. The same setting as a flag
in a recipe is found only by reading the recipe.
