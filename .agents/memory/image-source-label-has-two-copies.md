---
name: image-source-label-has-two-copies
description: The image-source URL is written as a literal in two files and nothing links them - change one and the smoke step goes red with a message that does not name the cause
type: gotcha
when: changing the repository URL, moving the repository, or editing the Dockerfile label block
paths:
  - "Dockerfile"
  - "tooling/smoke/**"
---

`org.opencontainers.image.source` is written out twice, as a plain string both times, and nothing ties the
two together:

- `Dockerfile:13` sets it in the `LABEL` block.
- `tooling/smoke/structure.yaml` asserts it in `labels:` - **a literal value, not a regex.** Every other
  entry near it that can vary carries `isRegex: true`; this one does not.

That smoke file runs against a locally built image and against the CI one, so changing the URL in one place
turns the smoke step red on every build. The failure reads as a label mismatch and does not say that the two
copies disagree, so the cause takes a while to find.

**Why:** the repository moved organisation once already. The next move, or any rename, touches this URL, and
the second copy is the one nobody remembers.

**How to apply:** change both in the same commit. If you only have one of them in front of you, grep for the
URL before you finish - `grep -rn 'github.com/binacle-labs/Binacle.Net' Dockerfile tooling/smoke/`.
