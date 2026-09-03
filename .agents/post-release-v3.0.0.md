---
description: Post-release - what to look at once v3.0.0 is out, what the release caused, and the plans it stops blocking. None of it holds the release up.
---

# Post-release - v3.0.0

**Status:** `v3.0.0` was tagged on 2026-09-01 and the pipeline ran green. **Open. None of it gates anything.**

**Where it stands, 2026-09-02.** Everything under *Things to look at* that a command can answer has been run
and passes - the image, the three names on one digest, the signature, the published verify commands, the
Docker Hub page, the docs host, the beta grep. **What is left there needs a browser.** Under *Things to do*,
job 1 is done and committed, job 2 is one deploy away, job 3 is a decision, and job 4 has not started.

**A pointer surface. `release-v3.0.0.md` is deleted** - this file is what is left of the release set. Where a
row names a plan, the plan holds the work. Where a row has no plan, it is a single mechanical act with a known
answer and it lives here.

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

- [x] **Ran 2026-09-02 - all profiles green.** `just smoke all binacle/binacle-net:3.0.0`, 27 requests in the
      full profile, no failures.

      **This confirms, it does not protect.** The pipeline already runs and checks the staging copy before
      anything is copied across, so a broken image cannot reach Docker Hub. What this still buys is the one
      thing the pipeline cannot check: that the **copy** landed something that runs, not just something with
      the right ID.

- [x] **`3.0` and `latest` are on the new image - read off the registry 2026-09-02.** Both created for the
      first time by this release; until now `latest` meant `2.1.1`. **This is the one thing no test release
      could rehearse.** It also closed the last two boxes of the release-by-dispatch plan, which is now deleted
      - the scratch-repo run it was blocked on was never needed, because the real release proved the same step.
      The design record is D25.

- [x] **All three names are one image - 2026-09-02.** `3.0.0`, `3.0` and `latest` all report
      `sha256:974f3dda3923`, 105986171 bytes, published 2026-09-01.

- [x] **Signed, and the build record is there - `just image verify 3.0.0` PASS, 2026-09-02.** Signed by the
      release workflow on `refs/heads/main`, a 166-package SBOM, and provenance naming run
      `33553918851`. Revision `150b61ed`, base `mcr.microsoft.com/dotnet/aspnet:10.0`, runs as `app` (1654),
      4 `System.*.dll` in `/app`. **The identity flag is the entire value**, since anyone with a GitHub
      account can sign anything.

      **Docker Hub only.** Only the release workflow touches the staging registry, so nothing ever reads the
      staging copy's signature. This passed on every test release including `3.0.0-beta.5`, under the same
      identity v3.0.0 uses. **All that is new is that the copy writes three names instead of one.**

- [x] **Nothing got locked - read off the repository API 2026-09-02.** `immutable_tags_settings` is
      `{"enabled": false, "rules": [".*"]}`, unchanged since 2026-08-13. The publish wrote `3.0.0`, `3.0` and
      `latest` with nothing in its way. **The rule is still `.*`**, which is the value job 3 has to correct
      before the switch is ever touched.

- [x] **GitHub reads the licence as AGPL-3.0 - checked 2026-08-31, off the repository API.** `spdx_id:
      AGPL-3.0`, `key: agpl-3.0`. **It did not need the tag** - GitHub works it out on push, so the answer
      arrived with the licence commit.

- [x] **The `LICENSE.GPL-3.0/` folder link still resolves - checked 2026-08-31.**
      `blob/main/LICENSE.GPL-3.0` returns a 301 to `tree/main/LICENSE.GPL-3.0`. **That link is baked into
      `2.1.1`, `3.0.0-beta.3` and `3.0.0-beta.4`**, whose footers cannot be changed, so the folder shape was
      chosen to keep it working. It does.

### Read what the release put in front of people

- [x] **Read 2026-09-02, and it is right.** `README.md`'s pin warning names `binacle/binacle-net:3.0` and
      nothing else; the test-build line is gone. `3.0` resolves on Docker Hub to the release digest. **A wrong
      pin there outlives every other miss**, and a stale one would not fail loudly, because every tag this
      project has ever published is still pullable - which is why it had to be looked at.

- [x] **Read 2026-09-02, off the registry API, and it is right.** The page names `3.0.0` and nowhere names
      `2.1.1`, no placeholder survived, there is no hand-maintained tag list, the verification section is
      there, and both categories took - *API management* and *Developer tools*. **That step had never run
      before**: no test release reaches it, so Docker Hub went from the old hand-written 2.x page straight to
      this one in a single write.

      **This closed `plans/ci-cd/dockerhub-overview.md`, which is now deleted.** It existed only to be
      checked against the published page.

- [ ] **Run the quick start off the page itself** - the `docker run` and the `curl` - and check the response
      matches what the page prints. It is the first thing most readers do, and **the response on it was
      taken from the test image**, so this is where you find out whether the two images agree.

      **This is an eyeball, not a rewrite.** If it turns into a rewrite, the pre-tag half did not happen.
      **`just smoke all` passing on 2026-09-02 is not this check** - the smoke suite runs its own requests,
      not the two commands printed on the page.

- [x] **The published commands work as printed - 2026-09-02.** Run from a clean shell against `3.0`:
      `cosign verify` printed the three checks and a certificate naming the release workflow on
      `refs/heads/main`, and `imagetools inspect` printed the digest and the attestation manifest. **A
      published command that fails reads as our bug**, and this one does not. The same block appears in
      `SECURITY.md`, on the Docker Hub page and on the docs site's verifying page.

- [ ] **Open the demo in a browser, from the published `3.0.0` image.** `docker run` it with `UI_MODULE=True`
      and open `/`, `/packing`, `/vipaq` and `/instance`. **The test release covered the same code; this
      confirms the copy to Docker Hub carried it.** Use the packing page like a visitor - type your own
      numbers, press Add bin, press Clear all - and check none of them puts an error box on the screen.

- [x] **The site shows v3.0.x - checked 2026-09-02.** `docs.binacle.net/version/latest/` answers 200 and
      lands on `v3.0.x`, and all four version folders answer 200. The host serves from Cloudflare, so the
      DNS move happened too.

- [x] **The two v3.0.x page edits are published - 2026-09-02.** The release-notes page reads *"Released 1
      September 2026"* and links `releases/tag/v3.0.0`; the verifying page no longer carries the moving-tag
      sentence. **The four live-site reads passed the same day** - every version page carries a title, a
      description and a canonical; `/version/latest/` is a meta-refresh page with `noindex` and a canonical
      pointing at `/version/v3.0.x/`; the three sites' `robots.txt` files are identical apart from the
      `Sitemap:` host; and all three manifests and their icons resolve.

- [x] **Checked 2026-09-02 - exactly the five correct hits and nothing else.** `grep -rn "3\.0\.0-beta"
      --exclude-dir=.agents` over the repo. Read the list rather than the count:

      | Hit | Why it stays |
      |---|---|
      | `NOTICE:16` | the AGPL boundary sentence - versions up to and including `3.0.0-beta.6` stay GPL-3.0 |
      | `CHANGELOG.md:30` | the same sentence |
      | `LICENSE.GPL-3.0/README.md:4` | the same sentence |
      | `sites/docs/collections/_versions/v3.0.x/release-notes.md:55` | the same sentence |
      | `tooling/ci/check-version.sh:10` | the string is inside a semver error message, not a pin |

      **Anything else is a real hit.** The `sites/www` comment that named two was deleted on 2026-08-31, and
      job 1 takes the README line and the seven sample pins. `tooling/image.just:9` used to name one and no
      longer does - the example was cut on 2026-08-31 and the sentence kept, because only a beta could have
      illustrated it.

---

## Things to do

### 1. Point the example files at `3.0`

**Do this the moment the publish job goes green.** Until then `binacle/binacle-net:3.0` does not exist, and
**an example on `main` must name an image someone can actually pull.** The tree at tag `v3.0.0` names a test
image for the length of one run; that is the accepted cost of the rule. **They sit on `3.0.0-beta.6`, moved
there on 2026-08-31** so that no public surface names a beta the published verify command rejects. **beta.7
shipped later the same day and the pins were left alone** - they all go to `3.0` here anyway, and moving nine
files twice buys nothing.

**All four boxes landed and are committed** - `4c735c25`, *post release pins*, 2026-09-02. Checked against
the tree the same day.

- [x] **Six `image:` lines are on `3.0`.**
      `grep -rn '3\.0\.0-beta' samples/` returns nothing.
- [x] **The expiring comment is gone from all six**, leaving only *"Pinned on purpose - a copied sample must
      not jump to a new major on the next pull."*
- [x] **`samples/README.md` and `samples/docker/README.md` both name `3.0`** with the explanation cut.
- [x] **The test-build line is out of `README.md`.** `grep -n '3\.0\.0-beta' README.md` returns nothing.

**Someone who copies an example keeps that version forever**, which is why this is worth doing on the day
rather than at leisure.

**Committed, so job 4 is unblocked.** Nothing public names a beta any more, which is the condition job 4
was waiting on.

### 2. Finish the documentation site's v3.0.x pages

**Done - 2026-09-02.** The site is on `docs.binacle.net` from Cloudflare, both page edits are published, and
the four live-site reads passed. `/version/latest/` lands on `v3.0.x` and all four version folders answer.

**What is left of that plan is one wording decision** - whether the v3.0.x quickstart page uses the canonical
tool names. Everything else in it was cut on 2026-09-02.

**The instruction that used to sit here was stale and was not followed.** It said to run the verify and paste
the real output onto `verifying-a-release.md`. The docs plan settled the opposite on 2026-08-31, later and
more specifically: no page under `sites/docs` quotes a figure that expires. Pasting output would have
reintroduced the fault that page was rewritten to fix. **The rule is now in the general decisions ledger under
D4**, so it stops living in a plan that is nearly deleted. **The check was still worth running** - both
commands, copied off the page, ran green from a clean shell on 2026-09-02.

**This was called the single most losable item in the release.** It was not lost.

### 3. Decide whether image names can be locked - **answered, no**

- [x] **Answered 2026-09-04: no. The switch stays off and the plan is deleted.** The decision and the trap
      that produced it are in the CI/CD decisions ledger under D26 - the stored rule is `.*`, which would
      freeze `latest` and `3.0`, and that is the first thing to correct if this is ever reopened.

### 4. Delete the old test images - **moved out 2026-09-04**

**Answered 2026-08-31 as "they go", and on 2026-09-04 the maintainer chose to leave them a few months.**
That makes it standing work rather than a post-release check, so it is a plan now:
`plans/ci-cd/delete-the-beta-images.md`, `state: deferred`. It holds the list of eight, including
`3.0.0-beta.8` which is recorded in no other file.

**Nothing here is waiting on it.** The example pins moved to `3.0` in `4c735c25`, so no public surface names
a beta either way.

---|---|
| `3.0.0-beta.1` to `-beta.4` | fail the published verify command |
| `3.0.0-beta.5`, `-beta.6`, `-beta.7` | pass it |
| `3.0.0-beta.8` | 2026-09-01, recorded nowhere else |

**This is what makes job 1 above mandatory rather than tidy-up.** After the deletions a beta pin names nothing
at all. **Job 1 is done and committed - `4c735c25`, so nothing is in the way of the deletion.**
The tag-policy table in `.github/dockerhub-overview.md` is where the answer belongs. **Putting this row here
was my call - strike it if the answer is obviously "leave them".**

---

## Plans that stop being stuck

**None of these blocks anything, and none is release work.** Each has its own file with its state and what it
waits on. **They are named here because the release is what stopped being in their way**, not to schedule
them.

| Plan | What the release freed |
|---|---|
| `plans/api/packing-demo-next.md` | nine of the ten. **The browser pass it was waiting on happened on 2026-08-27.** Two things are still open and neither waits on the release: the unfitted items, where the inline block was rejected on layout the day it shipped so the answer becomes a tooltip, and the submit button, which can stay disabled with no visualizer listening. The strings stay in the package |
| `plans/ci-cd/dockerhub-overview.md` | **deleted 2026-09-02.** Its last box was reading the published page, and the page is read and correct. The reasoning behind what the page carries is in the CI/CD design records |
| `plans/api/ui-clients-off-v3.md` | **both halves now.** `api.binacle.net` serves a 3.0.x image as of 2026-09-02 - AGPL in its OpenAPI document, `/openapi/v4.json` answers, `/openapi/v2.json` is gone. The blocker in its `waits-on:` is the shape, not the host |
| `plans/sites/docs-client-generation.md` | nothing the release owned. **It has a blocker of its own** - every page on the site sits under a version folder and this page is not version-specific, so where it lives is unanswered. It sits here because the docs deploy is the natural next docs session |

**`plans/_index.md` lists every plan, with its state and what it waits on.** Nothing above is a ranking.

---

## Tidy up

- [x] **`release-v3.0.0.md` is deleted - 2026-09-02.** Every gate row was done: B4 and B5 landed with the tag,
      and B2 and B3's remaining boxes are covered by the checks above against the published image rather than
      the test one. Its gotchas were checked against the design records first and none was the only copy - the
      changelog mechanics are in `$ci-cd/release-pipeline`, label ownership under D13, the moving-tag gap
      under D25.
- [x] **Nothing was carried forward.** The one thing it held that had not landed was the browser pass on the
      test image, and *Open the demo in a browser, from the published `3.0.0` image* above supersedes it -
      checking the published image is strictly better than checking the one it was copied from.
- [ ] **Delete this file** when the first two lists are clear. **The third list is not a reason to keep it** -
      those plans stand on their own.

## Then what

**The first thing to do is not a build.** How far the ServiceModule is taken is the maintainer's call, and it
settles the three plans under `plans/api/` and the Azure Storage removal question at once.

**Held back from v3.0.0 and waiting in `plans/`:** the heavy architecture tools (ArchUnitNET,
dependency-cruiser, lychee), CI gates 2 and 3, and rubocop, which has never been run.
