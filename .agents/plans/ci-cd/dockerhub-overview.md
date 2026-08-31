---
description: The Docker Hub repository page - both sections are done, and the file is kept only until the release that publishes the page has run
state: blocked
waits-on: "the v3.0.0 release publishing the page. Both sections are done - the file is kept only to be checked against the published page, then deleted"
horizon: next-release
paths:
  - ".github/workflows/**"
---

# The Docker Hub repository page

**The wiring is done.** `.github/dockerhub-overview.md` is the page, `just image dockerhub-overview <version>`
renders it, `shared-dockerhub-overview.yml` publishes it, and the release pipeline's `page` job calls that
last and skips prereleases. The CI/CD, release pipeline, tooling and commands docs carry all of it.
The short description on the repository was already correct.

**The credential is answered.** `DOCKERHUB_TOKEN` writes the description - HTTP 200 on the existing registry
push credential, proved 2026-08-19. No second secret, no password fallback. The CI/CD doc records it.

One thing left, then this file goes: reading the page the release publishes.

**The reasoning behind what the page carries and leaves out moved into the CI/CD design records on 2026-08-31**,
because it outlives this file.

## 1. The `curl` example - done 2026-08-31, and it needed no edit

**The response on the page was already correct.** It was run against `3.0.0-beta.5` and every field matches
byte for byte: `NotAllItemsFit` / `locker-S` / `60.0` / `72.97`, then `AllItemsFit` / `locker-M` / `30.83` /
`100`.

**This section said the response came from a deleted tag.** Whatever its source, the numbers are right, so the
worry was the wrong one. **The lesson worth keeping is not "re-run it" but "check before rewriting"** - a
rewrite here would have replaced a correct response with an identical one and looked like progress.

`just image dockerhub-overview 3.0.0` was run and the page renders with every placeholder filled. The recipe
refuses a version with a suffix, so it takes `3.0.0` even though the response came off beta.5.

## 2. The logo and the categories - done 2026-08-27

**The maintainer set both.** His words: *"did logo and categories."* Nothing here is open.

**Delete this file once section 1 is done.** Section 1 is all that keeps it alive.

## Done when

- [x] **The quick start example quotes a response from a tag that exists on Docker Hub - 2026-08-31.** Run
      against `3.0.0-beta.5`; every field on the page matched byte for byte, so no edit was needed.
- [x] **The rendered page was read before it was published - 2026-08-31.**
      `just image dockerhub-overview 3.0.0` was run and every placeholder filled.
- [ ] The published page matches what was read locally.
      **By eye**, on `https://hub.docker.com/r/binacle/binacle-net` after the real release has run the `page`
      job. That job is skipped for prereleases, so nothing has exercised it.
- [ ] This file is deleted.
      **By eye.** It goes with the box above; nothing else records what the `page` job is supposed to produce.
