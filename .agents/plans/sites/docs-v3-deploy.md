---
description: What the v3.0.x docs pages still need - a worked example quoting a deleted tag, the release date and link - plus the live-site checks nothing else watches
state: blocked
waits-on: "the v3.0.0 tag - the worked example quotes real output from the released image, and the release notes need its date and its release link"
paths:
  - "sites/docs/**"
---

# What the v3.0.x docs still need

**A docs session** - `sites/docs/` is published, so a coding session does not touch it.

**The site is deployed.** It runs on a temporary domain, and the maintainer switches DNS to it when v3 lands.
**Nobody needs to re-run the deploy** - what is left is page content and the checks below.

**Most of this file has landed.** The UI module corrections, the two swagger copies and the release-notes
carry-over are all done and verified against the tree on 2026-08-27, and the release-notes page was diffed
bullet for bullet against `CHANGELOG.md` again on 2026-08-28. Two page edits remain, and both need the tag to
exist.

---

## 1. `v3.0.x/verifying-a-release.md` quotes a tag that is gone

**It is broken, not merely stale.** Line 59 reads *"Against `3.0.0-beta.2`"* - a tag deleted from Docker Hub,
and signed under the old owner anyway. **The page is live.** It asks a reader to run a command that cannot
succeed, and a published verify command that fails reads as our bug.

Run `just image verify 3.0.0` and replace three things with what it prints: the Docker Hub digest, the package
count, and the provenance run URL. **It quotes real output, so it cannot be written before the tag exists.**

**The rule this came from, worth keeping: name a version where the version is the fact, never as a floor or an
example.** A floor or a sample tag goes stale on its own; a record of what was signed does not.

## 2. `v3.0.x/release-notes.md` still says the release has not happened

Line 29 reads *"Not released yet - the date and the release link are added when v3.0.0 is tagged."* Swap that
italic line for *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other
version folder.

**Everything else on that page is done.** The `/app/data` bullet, the `libgssapi-krb5-2` line, the Service
Module, Diagnostics and UI Module sections, migration steps 7 and 8, the signing block and the organisation
move are all on the page - checked 2026-08-27 against `CHANGELOG.md`.

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

## Done when

- [ ] The verifying-a-release example quotes the released image.
      `grep -n 'beta' sites/docs/collections/_versions/v3.0.x/verifying-a-release.md` returns nothing, and the
      command in it runs green from a clean shell.
- [x] **The three drift fixes are on the page - 2026-08-28.** `grep -c 'answers 422 rather than 400'` and
      `grep -c '20 worked examples'` each return 1, and `grep -c 'A tag now builds'` returns 0, all against
      `sites/docs/collections/_versions/v3.0.x/release-notes.md`.
- [ ] The release notes page names the release date and links the GitHub release.
      `grep -n 'Not released yet' sites/docs/collections/_versions/v3.0.x/release-notes.md` returns nothing,
      and the line below `## v3.0.0` names a date and a `releases/tag/v3.0.0` link.
- [ ] The four live-site reads in section 3 pass on the served host.
      **By eye**, plus `curl` on `/robots.txt` and `/version/latest/`, and the browser's application panel for
      the manifest icons.
- [ ] The two swagger copies still match a fresh generate.
      `diff <(jq -S . artifacts/openapi/Binacle.Net_v3.json) <(jq -S . sites/docs/collections/_versions/v3.0.x/swagger/v3.json)`
      and the same for v4 - both empty.
- [ ] The quickstart tool names are settled either way.
      `grep -n 'packing demo' sites/docs/collections/_versions/v3.0.x/samples/docker/quickstart/index.md` -
      the page is consistent with itself and with whatever was decided.
