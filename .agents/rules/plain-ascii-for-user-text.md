---
description: Text that reaches a user stays plain ASCII - no em dashes, curly quotes, ellipsis characters or arrows.
load: on-trigger
when: writing text a user will see - validation and exception messages, log lines, OpenAPI descriptions, UI strings
paths:
  - "api/src/**"
  - "packages/**"
  - "**/Config_Files/**"
---

# User-facing text is plain ASCII

Validation and exception messages, log lines, OpenAPI descriptions, UI strings: no em or en dashes, no curly
quotes, no ellipsis character, no arrows or symbols. Write `-` and `...`, and say "0-100", never "0–100".

**Prose under `.agents/` is free.** Nothing else is, code comments included. Settled 2026-09-04 - the
carve-out used to cover comments too, and it left 104 em dashes across 80 source files.

**Why:** these land in consoles, log files, JSON and terminals where the encoding is not ours to control. A
mangled character in a startup error is one more thing to debug.
