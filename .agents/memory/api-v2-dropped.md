---
name: api-v2-dropped
description: The v2 API does not exist on this branch — only v3 and v4; never add v2 code, docs, or references
type: convention
when: touching API versioning, routes or docs
paths:
  - "api/**"
---

There is no v2 API on this branch and no `api/src/Binacle.Net/v2/`.

**Why:** v4 is a major redesign, and v2 was removed while cleaning up legacy API versions. It is gone from the
branch, not deprecated in place.

**How to apply:** do not create v2 agent docs, do not reference v2 endpoints, and do not assume v2 code exists
when reading old plans or release notes that predate the removal. The live versions are v3, which is frozen,
and v4, in development. Docs: `$api/v4/contracts`.
