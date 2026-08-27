---
description: Post-release - what to look at once v3.0.0 is out, what the release caused, and the plans it stops blocking. None of it holds the release up.
---

# Post-release - v3.0.0

**Status:** Open once `v3.0.0` is tagged and the pipeline has run. **None of it gates anything** - the release
is already out by the time this file opens.

**A pointer surface, like the release file.** Where a row names a plan, the plan holds the work. Where a row
has no plan, it is a single mechanical act with a known answer and it lives here.

**Three lists, and they are in the order they should be worked.**

| List | What is in it | How to tell |
|---|---|---|
| **Things to look at** | you run it, read it, and it comes back yes or no | if it needs a decision, a password, a new file or a workflow, it is not one of these |
| **Things to do** | what the release *caused* and would be wrong to leave | it is only true because v3.0.0 shipped |
| **Plans that stop being stuck** | plans that were waiting on the release and now are not | each has its own file, state and blocker |

**Look first, because looking is cheap and it finds things.** The work comes second because it is the half
that can be quietly skipped. **Delete this file once the first two lists are clear** - the third is plans, and
plans outlive it.

Rewritten 2026-08-14 when the release scope was reset. Pruned 2026-08-20. Extended 2026-08-24 with what the
rebuilt sites added. Split into work and checks 2026-08-25. **Reworked into three lists, rewritten for the
test-release-then-real-release order, and put into plain English, 2026-08-26.**

---

## Things to look at

### Check the release landed

- [ ] **Run the published image and check it answers.** `just smoke all binacle/binacle-net:3.0.0`. About a
      minute, nothing to bring up.

      **This confirms, it does not protect.** The pipeline already runs and checks the staging copy before
      anything is copied across, so a broken image cannot reach Docker Hub. What this still buys is the one
      thing the pipeline cannot check: that the **copy** landed something that runs, not just something with
      the right ID.

- [ ] **Check that `3.0` and `latest` now point at the new image.** **This is the one thing no test release
      could rehearse.** A test release publishes its own exact name and nothing else, so `3.0` and `latest`
      are created for the first time here. Until now `latest` meant `2.1.1`.

- [ ] **Check all three names point at exactly the same image.**
      `docker buildx imagetools inspect binacle/binacle-net:3.0.0 --format '{{ .Manifest.Digest }}'`, then the
      same for `3.0` and `latest`. Three identical strings.

- [ ] **Check the signature and the build record on the real `3.0.0`.**
      `cosign verify binacle/binacle-net:3.0.0` with **both** the certificate-identity regexp and the OIDC
      issuer - the identity flag is the entire value, since anyone with a GitHub account can sign anything.
      Then `docker buildx imagetools inspect` for the SBOM and provenance entries.

      **Docker Hub only.** Only the release workflow touches the staging registry, so nothing ever reads the
      staging copy's signature. This passed on every test release including `3.0.0-beta.5`, under the same
      identity v3.0.0 uses. **All that is new is that the copy writes three names instead of one.**

- [ ] **Check no image name got locked by accident.** Read `immutable_tags_settings` back from the repository
      API. The publish should have written `3.0.0`, `3.0` and `latest` with nothing in its way.

- [ ] **Check GitHub reads the licence as GPL-3.0.** Read it back off the API. GitHub works the licence out on
      its own servers, so pushing is the only way to find out. **This is a read, not a fix** - if it comes
      back wrong, that is a finding for a plan.

### Read what the release put in front of people

- [ ] **Read the repository's front page.** `README.md` is the most read file in the repo, and its pin
      warning names `binacle/binacle-net:3.0` - a tag that only starts resolving with this release. **A wrong
      pin there outlives every other miss**, and a stale one would not fail loudly, because every tag this
      project has ever published is still pullable. You have to look.

- [ ] **Read the Docker Hub page - the release wrote it, and that step had never run before.** No test
      release reaches it; that job is skipped for them. So Docker Hub went from the old hand-written 2.x page
      straight to this one in a single write. Check that the description names 3.x
      rather than `2.1.1`, the version placeholders were substituted with real numbers, the hand-maintained
      tag list is gone, the verification section is there, and the logo and categories took.

      **Run the quick start off the page itself** - the `docker run` and the `curl` - and check the response
      matches what the page prints. It is the first thing most readers do, and **the response on it was
      taken from the test image**, so this is where you find out whether the two images agree.

      **This is an eyeball, not a rewrite.** If it turns into a rewrite, the pre-tag half did not happen.

- [ ] **Run the published verification commands from a clean shell.** **Check that the command printed on the
      Docker Hub page and in `SECURITY.md` is the one that actually works** - a published command that fails
      reads as our bug.

- [ ] **Open the demo in a browser, from the published `3.0.0` image.** `docker run` it with `UI_MODULE=True`
      and open `/`, `/packing`, `/vipaq` and `/instance`. **The test release covered the same code; this
      confirms the copy to Docker Hub carried it.** Use the packing page like a visitor - type your own
      numbers, press Add bin, press Clear all - and check none of them puts an error box on the screen.

- [ ] **Check the documentation site now shows v3.0.x.** `/version/latest/` should land on `v3.0.x` and the
      version picker should show four versions. **The item most likely to be quietly skipped**, because
      nothing fails when it is. **The rest of the live-site reads are in the docs plan** - they are not
      repeated here.

- [ ] **Check nothing public still names a test release.** Test releases 1 and 2 are deleted from Docker Hub,
      so anything left pointing at one is a dead link rather than an old number.
      `grep -rn "3\.0\.0-beta" --exclude-dir=.agents` over the repo, and read the docs site's
      verifying-a-release page.

      **One expected hit, and nobody can see it.** `sites/www/_data/exchange.yml` carries a YAML
      comment Jekyll never renders, saying the `3.0` tag is unpublished. **That comment is stale from the tag
      onward - delete it rather than re-reading it every release.**

---

## Things to do

### 1. Point the example files at `3.0`

**Do this the moment the publish job goes green.** Until then `binacle/binacle-net:3.0` does not exist, and
**an example on `main` must name an image someone can actually pull.** The tree at tag `v3.0.0` names a test
image for the length of one run; that is the accepted cost of the rule. **The test release did not change
this** - a test release creates no `3.0` name, so the examples correctly stayed on `3.0.0-beta.4` through
both tags.

- [ ] **Six `image:` lines move from `3.0.0-beta.4` to `3.0`.**
      `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` and
      `samples/kubernetes/minimal/binacle-deployment.yaml`.
      `grep -rn '3\.0\.0-beta' samples/` returns nothing.
- [ ] **Drop the expiring comment in the same six files.** Each `image:` line carries two extra lines - *"Pinned
      to the test build for now because `binacle/binacle-net:3.0` does not exist on Docker Hub yet - move to
      the 3.0 minor tag once v3.0.0 is published."* Delete those two, leaving only *"Pinned on purpose - a
      copied sample must not jump to a new major on the next pull."* **That reason expires the moment v3.0.0
      publishes; the second one does not.**
- [ ] **Rewrite the same reason in prose in `samples/README.md` and `samples/docker/README.md`.** Both name
      `3.0.0-beta.4` and explain why; both become `3.0` with the explanation cut.
- [ ] **Delete the test-build line from `README.md`.** The pin warning carries a second line - *"Until
      `3.0.0` is published, the only name that resolves is `binacle/binacle-net:3.0.0-beta.4`."* The `3.0`
      sentence above it is true on its own from this release onward, so the line becomes wrong the moment the
      publish job goes green. `grep -n '3\.0\.0-beta' README.md` returns nothing.

**Someone who copies an example keeps that version forever**, which is why this is worth doing on the day
rather than at leisure.

### 2. Finish the documentation site's v3.0.x pages

**`plans/sites/docs-v3-deploy.md`**, which is now down to two page edits and the live-site reads. The
corrections, the swagger copies and the release-notes carry-over are done.

**The deploy itself is done** - the site runs on a temporary domain and the maintainer switches DNS to it when
v3 lands. **Do not re-run it.**

**Both page edits have to be after the tag** - the notes need the date and the `releases/tag/v3.0.0` link, and
the worked example in `verifying-a-release.md` quotes real output from the released image. It currently
verifies `3.0.0-beta.2`, which is deleted from Docker Hub; re-cut it from `just image verify 3.0.0`.

**This is the single most losable item in the release.** Nothing fails if the pages are left as they are - the
verify page just keeps asking readers to run a command against a tag that no longer exists.

### 3. Decide whether image names can be locked

**`plans/ci-cd/dockerhub-tag-immutability.md`** - it holds the trap, the pattern, why test releases are left
out, and how to try it on a throwaway repository first.

- [ ] **Correct the rule to released versions only, and read the value back from the API.** The stored value on
      2026-08-13 was `".*"`, which would freeze `latest` and `3.0` - the two tags this release moves. The
      switch is off, so nothing broke either way.
- [ ] **Decide the switch, now that a real release is behind it.** Turning it on means the scratch repo first:
      there is no undo, and an immutable tag cannot be deleted either, so a release tag pushed by mistake is
      permanent. **If the answer is no, write that down as a decision and drop the plan.** Leaving it a
      permanently open question is the one outcome with no value.

### 4. Decide what happens to the old test images

**A decision, not a task, and it has no plan.** `3.0.0-beta.3`, `-beta.4` and `-beta.5` can still be pulled
from Docker Hub; 1 and 2 were deleted. **Either delete the rest now that a real image exists, or write down
that test builds are kept.** Whichever it is, the tag-policy table in `plans/ci-cd/dockerhub-overview.md` is
where the answer belongs. **Putting this row here was my call - strike it if the answer is obviously "leave
them".**

---

## Plans that stop being stuck

**None of these blocks anything, and none is release work.** Each has its own file with its state and what it
waits on. **They are named here because the release is what stopped being in their way**, not to schedule
them.

| Plan | What the release freed |
|---|---|
| `plans/api/packing-demo-bugs.md` | nine of the ten. **The submit button went in after all - checked 2026-08-27**, and both hosts render it. What is left is the browser pass on four, which rides on A1, plus the unfitted items: the inline block was rejected on layout the day it shipped, so the markup comes out of both templates and the answer becomes a tooltip. The strings stay in the package |
| `plans/ci-cd/dockerhub-overview.md` | section 2, the logo and the categories - **done 2026-08-27**. Section 1, the quick start's response, was taken by the release as B3 and is still open. **Delete the file when section 1 is done** |
| `plans/todos.md` | the docs site's old-register prose, and two one-liners in the shared UI package - a submit button that can stick where no visualizer listens, and an `Error` import that only reads wrong. **The theme defaults and the demo page's copy are done - checked 2026-08-27** |
| `plans/ci-cd/tests-reach-ci.md` | the ten Ruby tests reaching the PR gate. Held off the release gate because Ruby does not build the image |
| `plans/api/ui-clients-off-v3.md` | **the module half only.** The site half still waits on `api.binacle.net` serving a v3.0.x image |
| `plans/sites/docs-client-generation.md` | nothing was blocking it; it sits here because the docs deploy is the natural next docs session |
| `plans/ruby-gem-coverage.md` | a yes or a no. Ten published gems have no tests measured, and the alternative is writing down that this is deliberate |

**`plans/_index.md` lists every plan, with its state and what it waits on.** Nothing above is a ranking.

---

## Tidy up

- [ ] **Delete `release-v3.0.0.md`** once the release is out and the docs are deployed.
- [ ] **Move anything left in it back into a plan** rather than carrying it forward. If it was not done for the
      release, it is standing work now.
- [ ] **Delete this file** when the first two lists are clear. **The third list is not a reason to keep it** -
      those plans stand on their own.

## Then what

**The first thing to do is not a build.** How far the ServiceModule is taken is the maintainer's call, and it
settles the three plans under `plans/api/`, the two ServiceModule one-liners in `plans/todos.md` and the Azure
Storage removal question at once.

**Held back from v3.0.0 and waiting in `plans/`:** the heavy architecture tools (ArchUnitNET,
dependency-cruiser, lychee), CI gates 2 and 3, the last of the UI test harness the coverage gate hangs on, and
rubocop, which has never been run.
