---
description: Eight 3.0.0 beta images are still pullable on Docker Hub. They go, deliberately later rather than now.
state: deferred
waits-on: "the maintainer, who chose to leave them a few months. Nothing depends on it and nothing decays"
horizon: undecided
paths:
  - ".github/dockerhub-overview.md"
---

# Delete the old test images

**Was job 4 of `post-release-v3.0.0.md`.** It moved out because it is standing work, not a check, and it was
holding a file open that is otherwise two browser passes from being deleted.

**Answered 2026-08-31: they go. Answered again 2026-09-04: not yet, a few months.** Both are the
maintainer's, and the second is the live one.

**Two reasons they go at all.** Betas 1 to 4 fail the published verify command, so anyone following
`SECURITY.md` against one of them sees what reads as tampering. And a test build kept forever is a second
answer to "which image do I pull".

**Eight, not six, and one is recorded in no other file.** Read off the registry 2026-09-02.

| Tag | Why |
|---|---|
| `3.0.0-beta.1` to `-beta.4` | fail the published verify command |
| `3.0.0-beta.5`, `-beta.6`, `-beta.7` | pass it |
| `3.0.0-beta.8` | pushed 2026-09-01, digest `sha256:e2a7135bd6bd4`. Every other file in this repository stops at beta.7 |

**Nothing is in the way.** The example pins moved to `3.0` in `4c735c25`, so no public surface names a beta -
which is what made the deletion safe, and is also why waiting costs nothing.

**The tag-policy table in `.github/dockerhub-overview.md` is where the outcome belongs**, so the page stops
implying the betas are still a supported thing to pull.

## Done when

- [ ] No `3.0.0-beta.*` tag answers on Docker Hub.
      `docker buildx imagetools inspect binacle/binacle-net:3.0.0-beta.1` fails, and the same for the other
      seven. Read the registry, not this table.
- [ ] The tag-policy table says what happened to them.
      **By eye** in `.github/dockerhub-overview.md`.
