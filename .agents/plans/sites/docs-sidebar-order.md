---
description: The docs sidebar reads in the order the pages happened to get, not the order a reader needs - the six moved pages landed between Release Notes and API
state: ready
waits-on: "a session of its own - the maintainer said so on 2026-09-12, when the six common pages moved into the version folders"
horizon: now
paths:
  - "sites/docs/collections/_versions/**"
---

# The sidebar order

## What is wrong

Every page under `_versions/<folder>/` sets `nav.order` in its front matter, and the sidebar sorts by it. The
six pages that moved in from `_common_pages/` on 2026-09-12 kept the orders they had on the old root, so the
current line's sidebar reads:

```
1  Quick Start
2  Release Notes
3  Core Concepts
4  Configuration Basics
4  Generate a Client
5  API
6  Configuration
7  ViPaq Protocol
8  Verifying a Release
10 Integration Guide
20 Samples
```

Configuration Basics sits away from Configuration, Generate a Client sits before the API it generates from,
Release Notes sits second, and Integration Guide sits after Verifying a Release. The old lines have the same
shape minus Generate a Client and Verifying a Release.

## What to decide

One order for the top level, written into `nav.order` in every folder that has the page. A reader's order,
roughly: start (Quick Start), understand (Core Concepts), the API and how to call it (API, Generate a Client),
run it (Configuration Basics, Configuration, Samples), the rest (ViPaq, Verifying a Release, Integration
Guide, Release Notes). Whether Configuration Basics becomes a child of Configuration is part of the same
decision - it would then need `nav.parent`.

Old folders take the same edit: `nav.order` is a number in front matter, changes no content, and the freeze
rule guards truth, not numbers.

## Done when

- [ ] The top-level order is one list, the same in every folder that has the page.
      `grep -rn -A1 '^nav:' sites/docs/collections/_versions/*/*.md sites/docs/collections/_versions/*/*/index.md | grep order`
      sorted by folder reads the same sequence three times, minus the pages a line does not have.
- [ ] No two top-level pages share an order.
      The same grep, per folder, has no repeated number.
- [ ] Configuration Basics and Configuration are next to each other, or one is inside the other.
      **By eye** on the built sidebar at `/`.
