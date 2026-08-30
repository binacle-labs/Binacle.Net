---
description: What the v3.0.x docs pages still need - the release date and link - plus the live-site checks nothing else watches
state: blocked
waits-on: "the v3.0.0 tag - the release notes need its date and its release link"
paths:
  - "sites/docs/**"
---

# What the v3.0.x docs still need

**A docs session** - `sites/docs/` is published, so a coding session does not touch it.

**The site is deployed.** It runs on a temporary domain, and the maintainer switches DNS to it when v3 lands.
**Nobody needs to re-run the deploy** - what is left is page content and the checks below.

**Most of this file has landed.** The UI module corrections, the two swagger copies and the release-notes
carry-over are all done and verified against the tree on 2026-08-27, and the release-notes page was diffed
bullet for bullet against `CHANGELOG.md` again on 2026-08-28, and the FluxResults bullet it gained on
2026-08-29 is on the page too. **The licensing edits of 2026-08-30 landed the same way** - two more bullets,
the footer, the three site manifests and a samples section on all four versions. **One page edit remains that
needs the tag to exist** - section 2, the release date and link. Section 1 closed on 2026-08-31. **Section 7
is a third that does not need the tag - it should go on the next docs session whether or not it is cut.**

---

## 1. `v3.0.x/verifying-a-release.md` quoted a beta - done 2026-08-31

**It was broken, not merely stale.** It read *"Against `3.0.0-beta.2`"* - a tag deleted from Docker Hub, and
signed under the old owner anyway. Anchoring the command in section 7 would have made it wrong twice over, so
the same session rewrote it under the same override.

**It was rewritten as a record of `3.0.0-beta.5` on 2026-08-31 and that lasted a day.** The record carried
real figures - digest, 167 packages, the run URL. **It was replaced the same day, because the betas are
deleted once `3.0.0` is live**, and a record of a deleted image is the exact fault the first rewrite existed
to fix.

**What it says now.** Both commands name `3.0`, and *What a checked release looks like* describes what each
one prints - the checks, the certificate, the index, the SBOM, the provenance run, the `app (1654)` user -
**with no digest, no package count and no run URL.** Nothing on the page can go stale, so nothing needs
re-cutting after the tag. **This section is closed.**

**The rule, settled 2026-08-31: no page under `sites/docs` names a beta, or quotes a figure that expires.** A
versioned page names its own version explicitly - the `v3.0.x` pages name `3.0`. Betas are deleted from Docker
Hub once `3.0.0` is live, so a prerelease on a docs page becomes a dead reference on that day.

**The rest of the repository names `3.0.0-beta.6` until the tag** - the README and the seven sample pins moved
there on 2026-08-31, because a public surface must not name a beta the published verify command rejects.
**They move to `3.0` on the day of the tag and that is now mandatory rather than tidy-up**: after the
deletions a beta pin names nothing at all. `post-release-v3.0.0.md` carries it as its first job.

**The rule this came from, worth keeping: name a version where the version is the fact, never as a floor or an
example.** A floor or a sample tag goes stale on its own; the `3.0` minor tag does not.

## 2. `v3.0.x/release-notes.md` still says the release has not happened

Line 29 reads *"Not released yet - the date and the release link are added when v3.0.0 is tagged."* Swap that
italic line for *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other
version folder.

**Everything else on that page is done.** The `/app/data` bullet, the `libgssapi-krb5-2` line, the Service
Module, Diagnostics and UI Module sections, migration steps 7 and 8, the signing block and the organisation
move are all on the page - checked 2026-08-27 against `CHANGELOG.md`.

### The FluxResults bullet, added on 2026-08-29 by a coding session, on the maintainer's explicit override

**`never-edit-published-sites` was overridden for this one change.** Same sequence as the three below: the
session wrote down what the page must say and stopped, and the override came after.

`FluxResults` was a NuGet package the service module depended on. It is now in the tree as
`shared/src/Binacle.FluxResults`, with only the parts the API uses carried over, a renamed namespace and no
behaviour change. It is inside the image, so the bullet went in the **first** group of Internal Work bullets -
the ones above *"Everything below is work on the repository"* - directly after the `Binacle.Geometry` line,
matching the order in `CHANGELOG.md`.

**`NOTICE` also dropped its `FluxResults` entry**, because a first-party library in the tree is not a
third-party component. No page on the site lists the third-party dependencies, so nothing else moved - checked
2026-08-29.

### The licensing edits, made on 2026-08-30 by a coding session, on the maintainer's explicit override

**`never-edit-published-sites` was overridden for these, and the override came first this time** - the
maintainer granted it before the session touched anything, naming both the site files and the release notes.

**Two bullets, mirrored word for word against `CHANGELOG.md`.** *"Licensing is now stated per part"* went last
in Overview, after the `binacle-labs` move. *"Every part of the repository now names its own licence"* went in
the **first** Internal Work group, above *"Everything below is work on the repository"*, because the image's
`org.opencontainers.image.licenses` label changed with it. The intro sentence naming what is inside the image
gained the licence label alongside the packing log rework.

**A third bullet landed on the same page on 30 Aug**, for `CONTRIBUTING.md` and the pull request template. It
went below *"Everything below is work on the repository"* because neither reaches the image, and it is mirrored
word for word against `CHANGELOG.md` like the other two.

**Three other page edits went with them.** `_data/footer.yml` moved from CC BY-SA 4.0 to CC BY 4.0 and dropped
the ShareAlike icon; the three `sites/*/package.json` licence fields were corrected the same way, with
`sites/docs` also naming MIT because the versioned sample folders live inside it; and
`v3.0.x/samples/index.md` gained a **📄 Copying these files** section, because those files are MIT and nothing
on the site said so.

**Every version got the licence section, not only `v3.0.x`.** The maintainer extended the override to the
older lines when asked, so `v1.3.x`, `v2.0.x` and `v2.1.x` carry it too - **byte-identical in all four**, and
deliberately version-neutral: an earlier draft named a compose file, a manifest and a `Presets.json`, and
`v1.3.x` has no Kubernetes manifests. A licence grant is not version-specific, so one wording is correct
everywhere and four wordings would drift.

### Three fixes made on 2026-08-28 by a coding session, on the maintainer's explicit override

**`never-edit-published-sites` was overridden by name for this one change.** Recorded here because that rule
says a coding session writes down what the page must say and stops. It did that first; the override came
after. **The carve-outs in the rule did not cover this and still do not.**

What a full bullet-for-bullet diff against `CHANGELOG.md` found, and what was changed on the page:

| Was | Now |
|---|---|
| The `algorithm` 422 line was missing - the changelog gained it on 2026-08-28, after the carry-over | Added under the V3 stability bullet, no `vlink`, because `v3.0.x` has no errors or responses page |
| *"The Packing Demo carries **17** worked examples"* | **20**, which is what `shared/data/demo-samples/` holds and what the changelog says. The page was written when there were 17 |
| *"**A tag** now builds the image once"* | The release is dispatched with a version and the tag is made last. **The same sentence was wrong in `CHANGELOG.md` and was fixed there too** |

**Two changelog bullets have no counterpart on the page and that is deliberate** - the Service Module
exemption, which the page states as a callout at the top instead, and the forwarded-headers line, which the
page turns into a link rather than bold. Do not "fix" either.

## 3. Read the live site - these four checks exist nowhere else

**A deploy publishes every version folder**, not only `v3.0.x` - `v1.3.x`, `v2.0.x` and `v2.1.x` too, plus the
landing, the 404, the sitemaps, `robots.txt`, the typography and the code-block styles.

Four things a live read settles and nothing else does:

- Every version's pages still carry a title, a description and a canonical.
- `/version/latest/` still redirects, is still `noindex`, and its canonical still points at
  `/version/<current>/` rather than at itself.
- The three sites' `robots.txt` files are byte identical to each other apart from the host in the `Sitemap:`
  lines, and each is byte identical to what it was before. **One known difference:** the built file is 1367
  bytes where the hand-written one was 1368 - the gem no longer emits the leading blank line, so it starts
  directly with `# As a condition`. Content is otherwise identical and the three sites still match each other.
- The site serves and links a web app manifest, and the icon paths in it resolve. **Check it in a browser's
  application panel** - a wrong icon path fails with no console error.

**These can be read against the temporary domain now.** They do not need the tag.

## 4. Nothing watches the frozen OpenAPI copies

`tooling/openapi.just` writes only to `artifacts/openapi`, and `tooling/regen.just check` lists five globs,
none of them the swagger folder. **This file is the only control there is** on
`v3.0.x/swagger/v3.json` and `v4.json`. Both match a fresh generate today; nothing will say so when they stop.

## 5. One deliberate 404, do not "fix" it

The `v3.0.x` ViPaq page links the wire spec at a `blob/v3.0.0` path, which 404s until that tag is pushed.
**A versioned page should pin the spec it describes; do not repoint it at `main`.**

## 6. Small and open: canonical tool names on the quickstart page

`v3.0.x/samples/docker/quickstart/index.md` line 40 says *"the packing demo and the ViPaq decoder"* and line 47
says *"the visual packing demo"*. If that page should use the canonical tool names, both become
*"the Packing Demo and the ViPaq Decoder"*. One decision, two lines.

## 7. The verify command on two pages accepted more than it should - done 2026-08-31

**Done by a coding session on the maintainer's explicit override, granted after the session had written this
section and stopped.** Same sequence as the three under section 2. Both pages are edited.

**What landed:**

| Page | Change |
|---|---|
| `verifying-a-release.md` | the command anchored; a cosign version floor under *Install cosign*; a paragraph on the anchor under *Why both cosign flags matter*; the worked example rewritten, see section 1 |
| `release-notes.md` | the command anchored, nothing else - it follows `CHANGELOG.md` |

`grep -rn "yml@'" sites/docs/` returns nothing. The flag name is unchanged, so
`verifying-a-release.md:40` and `release-notes.md:313`, which name `--certificate-identity-regexp` in prose,
are still correct and were not touched.

**The original note, kept because it is what a reader of this file needs:** The repo-side edits are in the working tree: `SECURITY.md`, `CHANGELOG.md`,
`.github/dockerhub-overview.md` and `tooling/image.just`. `design/ci-cd/decisions.md` D3 records why, and it
supersedes an instruction there that used to say the regexp must not be tightened.

**The change.** The identity regexp ended at the `@` with no `$`, so it was a prefix match and accepted a
signature made from any ref in this repository. It now ends:

```
release-docker-image.yml@refs/heads/main$
```

**Two pages carry the old string:**

| Page | Line |
|---|---|
| `v3.0.x/verifying-a-release.md` | 20 |
| `v3.0.x/release-notes.md` | 117 |

**Replace `\.yml@'` with `\.yml@refs/heads/main$'` in the code block on each.** Nothing else in either block
moves - the issuer flag does not change, and the flag name stays `--certificate-identity-regexp` on purpose so
the prose around it stays true. That prose includes `verifying-a-release.md:40`, *"Drop
`--certificate-identity-regexp` and you are only asking whether anyone signed the image"*, and
`release-notes.md:313`, the migration step naming the same flag. **Both stay correct with no edit.**

**`release-notes.md` is the copy of `CHANGELOG.md`, so it follows the changelog**, which is already changed.

**One more sentence to carry, also from `SECURITY.md`:** *"Use cosign 2.6.0 or later, or 3.0.1 or later.
Sigstore is moving the public transparency log its signatures are recorded in, and older cosign builds cannot
read entries in the new one. An out-of-date binary fails the check the same way a tampered image would."*
It belongs on `verifying-a-release.md`, which is the page that teaches the command.

**One paragraph to carry across, from `SECURITY.md`**, which is the wording the others follow. It is new there:

> The identity ends `@refs/heads/main`, the branch the release workflow is dispatched on, and the `$` anchors
> it there. Without the anchor the check accepts a signature made from any branch in this repository, and
> pushing a branch is not a release.

**Do not carry across a prerelease note.** `SECURITY.md` briefly had one and it was removed the same day.
The reason given was that betas 3 and 4 were deleted; **they were not, and the note still should not come
back** - `3.0.0-beta.5` and `3.0.0-beta.6` verify with the command above, and a page that documents a
workaround for two prereleases nobody is asked to pull is carrying weight for nothing.

**Checked, not assumed.** The new command passes on `3.0.0-beta.5` and fails on `3.0.0-beta.4` and
`3.0.0-beta.3`; the documented prerelease swap passes on `3.0.0-beta.4`.

## Done when

- [ ] The verifying-a-release example quotes the released image.
      `grep -n 'beta' sites/docs/collections/_versions/v3.0.x/verifying-a-release.md` returns nothing, and the
      command in it runs green from a clean shell.
- [x] **The three drift fixes are on the page - 2026-08-28.** `grep -c 'answers 422 rather than 400'` and
      `grep -c '20 worked examples'` each return 1, and `grep -c 'A tag now builds'` returns 0, all against
      `sites/docs/collections/_versions/v3.0.x/release-notes.md`.
- [x] **The release notes page carries the FluxResults bullet - 2026-08-29.**
      `grep -c 'FluxResults' sites/docs/collections/_versions/v3.0.x/release-notes.md` returns 1, and the line
      sits above *"Everything below is work on the repository"*.
- [x] **The error page status line went on the page the same day it went in the changelog - 2026-08-31.**
      `grep -c 'answers with the status it names'` returns 1 on both `CHANGELOG.md` and the release notes page.
      The error page used to answer `200` on a direct hit at `2.1.1` too, so the fix describes a behaviour that
      shipped and earns a line under A5's rule.
- [ ] The release notes page names the release date and links the GitHub release.
      `grep -n 'Not released yet' sites/docs/collections/_versions/v3.0.x/release-notes.md` returns nothing,
      and the line below `## v3.0.0` names a date and a `releases/tag/v3.0.0` link.
- [ ] The four live-site reads in section 3 pass on the served host.
      **By eye**, plus `curl` on `/robots.txt` and `/version/latest/`, and the browser's application panel for
      the manifest icons.
- [ ] The two swagger copies still match a fresh generate.
      `diff <(jq -S . artifacts/openapi/Binacle.Net_v3.json) <(jq -S . sites/docs/collections/_versions/v3.0.x/swagger/v3.json)`
      and the same for v4 - both empty.
- [ ] Both docs-site copies of the verify command end `@refs/heads/main$`.
      `grep -c 'yml@refs/heads/main\$' sites/docs/collections/_versions/v3.0.x/verifying-a-release.md
      sites/docs/collections/_versions/v3.0.x/release-notes.md` returns 1 for each, and
      `grep -rn "yml@'" sites/docs/` returns nothing.
- [ ] The quickstart tool names are settled either way.
      `grep -n 'packing demo' sites/docs/collections/_versions/v3.0.x/samples/docker/quickstart/index.md` -
      the page is consistent with itself and with whatever was decided.
