---
description: The Docker Hub repository page - both sections are done, and the file is kept only until the release that publishes the page has run
state: done
waits-on: "nothing. Delete this file once the v3.0.0 release has published the page"
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

One thing left, then this file goes.

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

## Do not

Each of these was decided and is easy to undo by accident.

- **Do not put the tag list back.** Fifteen hand-maintained entries duplicating a Tags tab that is always
  right, and it is what rotted the page the first time. The three-row policy table replaces it and answers the
  one thing neither tab explains: which tag belongs in a compose file.
- **Do not name GHCR on the page.** It is staging. The reason to name it would have been handing rate-limited
  users an escape hatch, and the sponsored badge means there are none - a staging registry on a landing page
  becomes a support surface nobody meant to own.
- **Do not commit a concrete version into the page.** Placeholders and substitution, or the file is wrong the
  day the next minor ships. That is the same reason the tag list went.
- **Do not add the service module, ViPaq, or the health endpoint.** The module is not advertised here, at the
  maintainer's call. ViPaq belongs on the docs site - on a page where someone is deciding whether to run one
  command, a second format name is a reason to hesitate. The health endpoint is off by default and its path is
  configurable, so a line about it is wrong for most readers.
- **Do not let the `cosign` block drift from `SECURITY.md`.** It is copied verbatim, only the tag differs, and
  that file is the source. A published verify command that fails reads as a bug in the project.
- **Do not publish a page naming a tag that is absent or unsigned.** The org move re-keyed the certificate
  identity, so anything signed under the old owner fails the published command.
- **Do not widen `DOCKERHUB_TOKEN`.** It already does both jobs at the scope it has.
- **Do not dispatch the workflow with an empty version input to check a wording change.** Empty takes the
  latest non-prerelease release, which is not always the version you just edited the page for. Render it
  locally instead, or type the version.

## Done when

- [ ] The quick start example quotes a response from a tag that exists on Docker Hub.
      **By eye.** Run the `curl` in `.github/dockerhub-overview.md` against a published tag and compare it to
      the pasted response.
- [ ] The rendered page was read before it was published.
      `just image dockerhub-overview <version>` prints what the pipeline writes; read that, not the source.
- [ ] This file is deleted. Section 1 is all that keeps it alive.
      **By eye.** With section 1 done there is nothing left in it.
