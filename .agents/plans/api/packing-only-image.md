---
description: The public image becomes packing-only and the Service Module moves to its own image
state: proposed
waits-on: "nothing. The tag landed 2026-09-01. It still needs a yes from the maintainer, which is what `proposed` means"
paths:
  - "api/**"
---

# The public image becomes packing-only

**Decided 2026-08-31, and it is the inverse of what this file used to say.** The normal image ships packing
and fitting only, with no ServiceModule assemblies in it at all. The ServiceModule gets its own image, and
that one is the odd one out.

The module is for the maintainer's own instance, so shipping its auth and database assemblies to every
self-hoster is surface defended for nobody. Removing them removes the documentation and migration obligation
with them.

**The work:** an MSBuild condition drops the ServiceModule project reference from the packing-only publish;
`Program.cs` and the DI wiring reference ServiceModule types directly, so they need `#if` or partial wiring
to compile without it; `release-docker-image.yml` gains a second build and tag.

**After the tag, not in v3.0.0.** Strictly a breaking change to the image contents, but `SERVICE_MODULE`
defaults off and the module has had no public documentation since v2.0.0, so it reaches nobody.

**Open:** tag naming, and how much `#if` plumbing `Program.cs` and the DI setup actually need.

**Goes with it:** the `samples/docker/service/` sample page and the last ServiceModule mentions in the v3
docs.

## Done when

- [ ] The normal image contains no ServiceModule assembly.
      `just build image` then `docker run --rm --entrypoint ls binacle/binacle-net:<tag> /app` names no
      `Binacle.Net.ServiceModule*.dll`.
- [ ] The packing-only build compiles with the project reference gone, not with it present and unused.
      `grep -n 'ServiceModule' api/src/Binacle.Net/Binacle.Net.csproj` shows the reference behind a condition.
- [ ] A second image exists for the module, and the release publishes both.
      `grep -c 'docker/build-push-action' .github/workflows/release-docker-image.yml` returns 2 or more, and
      each names its own tag.
- [ ] The tag naming is decided and written where a user meets it, not only here.
      **By eye.** The Docker Hub page and `samples/` name both images. If the answer is only in this plan,
      the box is open.
- [ ] The v3 docs and `samples/docker/service/` point at the module image, not at the normal one.
      `grep -rn 'SERVICE_MODULE' samples/ sites/docs/collections/_versions/` resolves against the new image.
