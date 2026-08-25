---
description: Post-release - the work v3.0.0 causes and the checks to run once it is out
---

# Post-release - v3.0.0

**Status:** Open once `v3.0.0` is tagged and the pipeline has run. **None of it gates anything** - the release
is already out by the time this file opens.

**Two halves.** The work first, because it is what the release caused and it is the half that can be silently
skipped. Then the checks, which are reads.

- **The work** points at plans, the same way the release file does. Where a row has no plan, it is a single
  mechanical act with a known answer and it lives here.
- **The checks need no plan.** Every one is something you run, read or look at, and comes back yes or no.
  **If working an item needs a decision, a credential, a new file or a workflow, it is not a check** - it is a
  row in the work half, or a plan.

**Delete this file once both lists are clear.** The tag does not delete it; working through it does.

Rewritten 2026-08-14, when the release scope was reset. Pruned 2026-08-20. Extended 2026-08-24 with the
checks the rebuilt sites added. **Split into work and checks 2026-08-25.**

---

## The work

### 1. Move the sample pins to `3.0`

**Do this as soon as the publish job is green.** Until then `binacle/binacle-net:3.0` does not resolve, and
**a pin on `main` must name an image that exists on Docker Hub.** The tree at tag `v3.0.0` carries a beta pin
for the length of one run; that is the accepted cost of the rule.

- [ ] **Six `image:` lines move from `3.0.0-beta.4` to `3.0`.**
      `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` and
      `samples/kubernetes/minimal/binacle-deployment.yaml`.
      `grep -rn '3\.0\.0-beta' samples/` returns nothing.
- [ ] **Drop the expiring comment in the same six files.** Each `image:` line carries two extra lines - *"Pinned
      to the beta patch for now because `binacle/binacle-net:3.0` does not exist on Docker Hub yet - move to
      the 3.0 minor tag once v3.0.0 is published."* Delete those two, leaving only *"Pinned on purpose - a
      copied sample must not jump to a new major on the next pull."* **That reason expires the moment v3.0.0
      publishes; the second one does not.**
- [ ] **Rewrite the same reason in prose in `samples/README.md` and `samples/docker/README.md`.** Both name
      `3.0.0-beta.4` and explain why; both become `3.0` with the explanation cut.

**A copied sample carries its pin forward forever**, which is why this is worth doing on the day rather than
at leisure.

### 2. Deploy the docs

**The whole of `plans/sites/docs-v3-deploy.md`.** It holds the `v3.0.x` corrections, the swagger copies, the
release-notes carry-over and the deploy itself, with its own checks.

**It has to be after the tag** - the notes need the date and the `releases/tag/v3.0.0` link, and the worked
example in `verifying-a-release.md` quotes real output from the released image. **It has to land before
anything is announced.**

**This is the single most losable item in the release.** Nothing fails if the deploy is skipped. `main`
already says `current: v3.0.x`, so the site just quietly keeps serving v2.1.x as current and nothing says so.

### 3. Settle the immutability switch

**`plans/ci-cd/dockerhub-tag-immutability.md`** - it holds the trap, the regexp, why prereleases are excluded,
and the scratch-repo test procedure.

- [ ] **Correct the rule to released versions only, and read the value back from the API.** The stored value on
      2026-08-13 was `".*"`, which would freeze `latest` and `3.0` - the two tags this release moves. The
      switch is off, so nothing broke either way.
- [ ] **Decide the switch, now that a real release is behind it.** Turning it on means the scratch repo first:
      there is no undo, and an immutable tag cannot be deleted either, so a release tag pushed by mistake is
      permanent. **If the answer is no, write that down as a decision and drop the plan.** Leaving it a
      permanently open question is the one outcome with no value.

---

## The checks

### Confirm the release landed

- [ ] **Smoke the published image.** `just smoke all binacle/binacle-net:3.0.0`. About a minute, nothing to
      bring up.

      **This is a confirmation, not a safety net.** The pipeline smokes the GHCR copy before anything is copied
      across, so a broken image cannot reach Docker Hub. What this still buys is the one thing the pipeline
      cannot check: that the **copy** landed something runnable, not just something with the right digest.

- [ ] **Confirm `3.0` resolves on Docker Hub, and that `latest` moved.** Both are written for the first time by
      this release - every beta withheld them. Until now `latest` resolved to `2.1.1`.

- [ ] **Confirm `3.0.0` resolves, and that all three tags share one digest.**
      `docker buildx imagetools inspect binacle/binacle-net:3.0.0 --format '{{ .Manifest.Digest }}'`, then the
      same for `3.0` and `latest`. **Three names, one hash, or the copy did not do what it claims.**

- [ ] **Verify the signature and the attestations against the real `3.0.0`.**
      `cosign verify binacle/binacle-net:3.0.0` with **both** the certificate-identity regexp and the OIDC
      issuer - the identity flag is the entire value, since anyone with a GitHub account can sign anything.
      Then `docker buildx imagetools inspect` for the SBOM and provenance entries.

      **Docker Hub only.** Only the release workflow touches GHCR, so the staging copy's signature is read by
      nothing. All of this ran green against `3.0.0-beta.3` and `3.0.0-beta.4` under the identity v3.0.0 uses.
      **What is new is only that the copy writes three tags instead of one.**

- [ ] **Confirm nothing froze.** Read `immutable_tags_settings` back from the repository API. The publish
      should have written `3.0.0`, `3.0` and `latest` with no interference.

- [ ] **Read the licence GitHub detects for the repository, back off the API.** Detection runs server side, so
      pushing is the only way to find out. The repository declares GPL-3.0 and the expected answer is
      `GPL-3.0`. **This is a read, not a fix** - if it comes back wrong, that is a finding for a plan.

### Read what the release published

- [ ] **Read the repo landing page by eye.** `README.md` is the most read file in the repo, and its pin warning
      names `binacle/binacle-net:3.0` - a tag that only starts resolving with this release. **A wrong pin there
      outlives every other miss**, and a stale one would not fail loudly, because every tag this project has
      ever published is still pullable. You have to look.

- [ ] **Read the Docker Hub page - the release run published it, and that step had never run before.** The
      page was never dispatched by hand, so Docker Hub went from the old hand-written 2.x page straight to this
      one in a single write. Check that the description names 3.x rather than `2.1.1`, the version placeholders
      were substituted with real numbers, the hand-maintained tag list is gone, the verification section is
      there, and the logo and categories took.

      **Run the quick start off the page itself** - the `docker run` and the `curl` - and check the response
      matches what the page prints. It is the first thing most readers do.

      **This is an eyeball, not a rewrite.** If it turns into a rewrite, the pre-tag half did not happen.

- [ ] **Run the verification checks against the real `3.0.0`, from a clean shell.** **Confirm the invocation
      printed on the Docker Hub page and in `SECURITY.md` is the one that actually works** - a published
      command that fails reads as our bug.

- [ ] **Read the UI module in a browser, from the published image.** `docker run` it with `UI_MODULE=True` and
      open `/`, `/packing`, `/vipaq` and `/instance`. The theme switcher and every page under it were rebuilt
      twice this release. **The pre-tag pass read a local build; this reads what shipped.**

- [ ] **Check the docs site is on v3.0.x.** Confirm `/version/latest/` lands on `v3.0.x` and the version picker
      shows four versions. **The item most likely to be silently skipped**, because nothing fails when it is.
      **The rest of the live-site reads are in the docs plan's own checks** - they are not repeated here.

- [ ] **Confirm no public surface still names a beta.** Betas 1 and 2 are deleted from Docker Hub, so anything
      left pointing at one is a 404 rather than an old number.
      `grep -rn "3\.0\.0-beta" --exclude-dir=.agents` over the repo, and read the docs site's
      verifying-a-release page.

      **One expected hit, and it is not a public surface.** `sites/www/_data/exchange.yml` carries a YAML
      comment Jekyll never renders, saying the `3.0` tag is unpublished. **That comment is stale from the tag
      onward - delete it rather than re-reading it every release.**

---

## Tidy up

- [ ] **Delete `release-v3.0.0.md`** once the release is out and the docs are deployed.
- [ ] **Move anything left in it back into a plan** rather than carrying it forward. If it was not done for the
      release, it is standing work now.
- [ ] **Delete this file** when both of its lists are clear.

## Then what

**The plans.** `plans/_index.md` lists every one not tied to this release, with its state and what it waits on.

**Held back from v3.0.0 and waiting there:** the heavy architecture tools (ArchUnitNET, dependency-cruiser,
lychee), CI gates 2 and 3, the last of the UI test harness the coverage gate hangs on, every test leaf reaching
CI, the Ruby coverage answer, and rubocop, which has never been run.

**The first thing to do is not a build.** How far the ServiceModule is taken is the maintainer's call, and it
settles the three plans under `plans/api/`, the two ServiceModule one-liners in `plans/todos.md` and the Azure
Storage removal question at once. **Pick the next thing once this list is clear.**
