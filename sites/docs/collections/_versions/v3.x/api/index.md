---
title: API
description: >-
  The two API versions in Binacle.Net: V3, which is stable and recommended, and V4, which is
  experimental. V2 was removed in v3.0.0.
nav:
  order: 4
  icon: 📡
---

Two API versions are available in {{ page.version_label }}.

**API v2 was removed in v3.0.0.** If you still call it, see the
[v2.x documentation]({% vlink v2.x /index.md %}) for the old contract and the
[Release Notes]({% vlink release-notes.md %}) for how to move off it.

Read [Core Concepts]({% vlink /core-concepts.md %}) first - it is what fit, pack and the algorithm names mean.

---

## ✅ Version 3

Version 3 provides fitting and packing over a set of bins, using either a preset or custom bin dimensions.
Every endpoint takes an algorithm and returns one result per bin.

V3 is **stable and recommended**. It is unchanged in this release apart from the ViPaq payload, which uses the
new format.

➡️ Learn more about [Version 3]({% vlink /api/v3.md %})

---

## 🧪 Version 4

Version 4 covers everything V3 does across 16 endpoints, and splits a request by the answer you want: one bin,
the smallest bin that works, the bin the items fill most, or a result for every bin. It also adds the `Best`
algorithm and single-preset lookups.

V4 is **experimental and can change at any time** - a minor release may change it. Use V3 for anything you keep.

➡️ Learn more about [Version 4]({% vlink /api/v4.md %})

---
