---
id: decisions
description: General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact, why the licence file keeps its name and why the root holds only one of them, why only the current docs version is indexable and old ones are bug-fix only, how the agent reference layer is kept honest against the code, and what was deliberately not reduced to a shared model.
verified: 2026-08-31
check: D6 by running `licensee detect .` at the repo root, which must report GPL-3.0 with LICENSE.GPL-3.0 as the only matched file, and by confirming the root holds exactly one file whose name contains LICENSE, LICENCE, COPYING or COPYRIGHT and that no LICENSES/ folder exists; D1 against the copyright lines in NOTICE, README.md, CONTENT-TERMS.md, the root package.json author, the UI module's Pages/Shared/_Footer.cshtml and the two gemspecs, and against org.opencontainers.image.vendor in Dockerfile; every repository.url stays on binacle-labs; D3 against the certificate-identity-regexp, which must name binacle-labs everywhere and must be anchored everywhere - an unanchored copy accepts a signature made from any ref in the repository; the three published copies in SECURITY.md, CHANGELOG.md and .github/dockerhub-overview.md must each end yml@refs/heads/main$ literally, and tooling/image.just must default signed_from to refs/heads/main and close the regexp with $ because it builds the string to keep the old betas checkable; the two docs-site copies are not a coding session's to change and sit in plans/sites/docs-v3-deploy.md section 7 until a docs session moves them; D7 by building sites/docs and confirming every non-current version page carries `noindex, follow` and no sitemap lists a `noindex` URL; D8 against `shared/src/Binacle.Packing/Abstractions/`, which must hold `IWithID.cs`, `IWithReadOnlyID.cs`, `IIdentifiableBin.cs` and `IIdentifiableItem.cs`, and against `shared/src/Binacle.Packing/Models/` for the two `internal readonly struct` types
paths:
  - "NOTICE"
  - "README.md"
  - "SECURITY.md"
  - "CHANGELOG.md"
  - "Dockerfile"
  - "CONTENT-TERMS.md"
  - "sites/docs/**"
  - "shared/src/Binacle.Packing/**"
---

# General — decisions ledger

Decisions that belong to no single slice. What each area *is* lives in its own doc; this file is the reasoning,
so a later session does not undo a deliberate choice.

## Locked

### D1 — the repository moved to an organization, and copyright did not move with it

The repo has lived at `binacle-labs/Binacle.Net` since 2026-08-16.

**Copyright and authorship stay on the person, everywhere they appear** — `NOTICE` and `README.md`
("Copyright (c) 2023-2026 Chris Mavrommatis"), `CONTENT-TERMS.md` ("© 2026"), the root `package.json`
`author`, the copyright line in the UI module's `Pages/Shared/_Footer.cshtml`, and the `authors` in both
`.gemspec` files. Moving a repository into a GitHub organization does not move copyright, and `binacle-labs`
is a namespace rather than a legal entity — there is nothing for it to hold. Writing the org name into a
copyright line would make that line less true.

**A `repository.url` is the opposite case and does carry the org**: both `package.json` files point at
`github.com/binacle-labs/Binacle.Net`, which is where the repository actually is. `packages/binacle-net-ui/package.json`
has a `repository` and a `license` but **no author field at all**, so there is nothing on it to protect — do not
add one to make the set look symmetrical.

**The vendor label does move.** `org.opencontainers.image.vendor` in the `Dockerfile` reads `Binacle Labs`.
It is descriptive and makes no legal claim, which is the whole reason it can differ from the copyright line.
In CI, metadata-action overrides title, description, source, url and licenses — **`vendor` is not in that
list**, so the `Dockerfile` value is what reaches published images.

**This is written down because a sweep that replaces one string tends to replace the other.** The copyright
lines are correct as they are. Do not tidy them.

### D6 — the licence file keeps its name, and two other things fix the badge

GitHub reported the repository's licence as `NOASSERTION`. **The cause was measured with `licensee`, the gem
GitHub runs, not reasoned about** — an earlier plan blamed the GPL file's name and was wrong.

| Root files present | `licensee detect` reports |
|---|---|
| `LICENSE` alone | `GPL-3.0` |
| `LICENSE.GPL-3.0` alone | `GPL-3.0` |
| `LICENSE` + `CONTENT-LICENSE.md` | **`NOASSERTION`** |
| `LICENSE` + root `package.json` carrying `GPL-3.0-only AND CC-BY-SA-4.0` | **`NOASSERTION`** |
| `LICENSE.GPL-3.0` + `CONTENT-TERMS.md`, no `license` field | `GPL-3.0` |

Measured 23 Aug 2026, licensee 10.1.0.

**Two causes, neither of them the GPL file's name.** Licensee matches any root filename containing `LICENSE`,
`LICENCE`, `COPYING` or `COPYRIGHT`, so `LICENSE.CC-BY-SA-4.0` and `CONTENT-LICENSE.md` are both candidates —
and their content is a plain-English summary matching no known licence, which is what makes the set
unresolvable. Separately, licensee reads the root `package.json` `license` field and cannot resolve a
**compound SPDX expression**; an `AND` there is enough on its own.

**So `LICENSE.GPL-3.0` keeps its name.** Renaming it to `LICENSE` would have been free tidiness with a
permanent cost: `Footer.razor` in every published image — `2.1.1`, `3.0.0-beta.3`, `3.0.0-beta.4` — hardcodes
`{GitHub}/blob/main/LICENSE.GPL-3.0`, and an image cannot be fixed after it ships. The live marketing site
serves the same URL from a stale build. **A path a shipped artifact points at is not free to move.**

**The root `package.json` declares no licence at all.** It is `private: true` and never published, so the
field was decorative; the honest options were a single ID narrower than the truth or no field, and no field
makes no false claim. `NOTICE` and `README.md` carry the real licence map.

**The root holds exactly one licence file, and every other licence lives in a subdirectory.** Measured again
on 30 Aug 2026 with the same gem: **two root files report `NOASSERTION` whatever they are named**, and a
second file cut down to a short pointer does not escape it — filename scoring runs on the name alone, so a
root file that matches the pattern is a candidate even when its content matches nothing, and "matches
nothing" resolves to `other`, which counts as a second licence. That is the same mechanism `CONTENT-LICENSE.md`
tripped above, with different files.

**Subdirectories are invisible to licensee**, which is why the nineteen `LICENSE` files under `ruby/`,
`samples/`, `tooling/`, the two wire-format libraries and their npm twins cost the badge nothing. Confirmed
against the repository after they landed: `GPL-3.0`, 100%, exact matcher.

**One trap in that: never create a `LICENSES/` folder.** licensee scans that name specifically, per the REUSE
spec, and two files inside it would be two licences again. Any other folder name is fine.

**The move to AGPL-3.0 landed on 31 Aug 2026, and this layout is what let it.** `LICENSE.GPL-3.0` became a
*directory* holding the old text and a `README.md`; `LICENSE.AGPL-3.0` is the one licence file at the root.
Measured after the move with the same gem: `AGPL-3.0`, 100%, exact matcher, `LICENSE.AGPL-3.0` the only file
matched. **GitHub redirects `/blob/<ref>/<dir>` to `/tree/<ref>/<dir>`**, so the footer link that `2.1.1`,
`3.0.0-beta.3` and `3.0.0-beta.4` hardcode still resolves - it lands on the folder's `README.md`, which says
which licence covers which versions. That is better than the file it replaces, which was 35KB of raw legal
text with no context.

### D2 — a version's published page must match what that version's image serves

Each folder under `sites/docs/collections/_versions/` describes the image that shipped under that minor version.
`2.1.1` really does serve `https://github.com/ChrisMavrommatis/Binacle.Net` in its OpenAPI documents and its
UI, so rewriting v1.3.x, v2.0.x or v2.1.x to say `binacle-labs` would make the page disagree with the running
artifact. **Only v3.0.x changed**, because `3.0.0` is built after `Metadata.cs` moved and serves the new owner.
How the site is versioned is `$sites/docs`.

The same reason covers every other survivor of the move: the `v1.3.0...v2.0.0` compare link in `CHANGELOG.md`,
the `ChrisMavrommatis.*` NuGet package names listed there, the 2024 records under `results/lib/benchmarks/`,
and the links to workflow runs that happened under the old owner. They are records of what was true then.
GitHub redirects them forever, and rewriting them makes them false.

**The swagger json under each version folder is generated output.** Regenerate it, never hand-edit it — the
rule and the generator are in `$sites/docs`.

### D7 — an old docs version is de-indexed, and after that it is only ever bug-fixed

Four documentation versions are published and only one is current. Before 2026-08-23 all four were indexable,
all four were in a sitemap, and no `<title>` said which version it was: 72 of 118 built pages shared both a
title and a meta description with a sibling, and five said `Quick Start - Binacle.Net Docs`. A search engine
had nothing to choose on, so readers landed on documentation for image tags that will never ship again.

**Only `current` is indexable.** Every other version is served `noindex, follow` and is in no sitemap, and
every versioned title carries its version. `follow`, not `nofollow` — an old page's links still lead
somewhere worth crawling. The mechanism is in `$sites/docs#search-and-current`; it reads
`_data/versions.yml` and names no version, so opening a new line is still the one edit it always was.

**Swagger pages leave the sitemaps too, current version included.** They were already `noindex` and still
listed, which Search Console reports as "Submitted URL marked 'noindex'" — an error, ranked with real
breakage, so it buries it.

**What this buys: an old version is not worth editing except to fix a bug.** Nobody arrives on it from
search, so a wording improvement there reaches nobody and a typo costs nothing. That is a change of policy,
not just of tone — the site used to carry the risk that a stale page was someone's first result. It does not
now. The freeze on old folders was already structural (`$sites/docs`); de-indexing is what makes leaving them
alone safe rather than merely convenient.

**It is deliberately cheap to reverse, and that is why the old versions have written descriptions.** Every
legacy page carries a hand-written `meta_description` naming the version it documents, written on the same day
the versions were de-indexed. **That was the maintainer's call and the reasoning is the point:** if indexing an
old line ever turns out to be worth it, flipping it back is a change to `current` and a sitemap, not a writing
project across seventy-four pages. **De-index freely; do not also let the copy rot** — the two decisions look
like one and are not.

### D3 — the signing identity moved with the repository, and there are three bands

cosign keyless writes the repository's full path into the certificate, so the published verify command names
the owner. **GitHub redirects web links; it does not redirect a signing identity.** A stale one fails the
check rather than warning, and a `cosign verify` failure reads as tampering rather than as a moved repo.

| Band | Images | Verifies with |
|---|---|---|
| unsigned | up to `3.0.0-beta.1`, and `2.1.1` and earlier | nothing — `no signatures found` |
| old identity | `3.0.0-beta.2` alone | `ChrisMavrommatis` in the identity regexp |
| new identity | `3.0.0-beta.3` onward | `binacle-labs`, the string every surface now carries |

Signing, the SBOM and the GHCR staging copy all start at beta 2. Beta 3 is the first tag pushed after the
move, and `3.0.0-beta.4` followed on 2026-08-19 into the same band.

**`3.0.0-beta.3` is the only image a verify run has passed against under the current identity**, so it is the
tag to name wherever a doc needs a real one and the tag to re-run any verification against. Beta 2 is signed
but the published command rejects it. Beta 4 is in the band and untried.

**Proven end to end on 2026-08-17.** `just image verify 3.0.0-beta.3` passed all four checks; the command
printed in `SECURITY.md` passed verbatim from a clean shell; and the SLSA provenance names
`github.com/binacle-labs/Binacle.Net/actions/runs/31970609518` — Fulcio's record of which workflow signed it,
not a string this repo controls.

Which surfaces carry the invocation, and what else would change it, is `$ci-cd/decisions#D15`.

### D4 — name a version where the version is the fact, never as a floor or an example

A floor ("signed from `X` onward") and a sample tag both go stale on their own. A record of what was signed
does not. So a floor names the current released version, an example uses a placeholder the reader
substitutes, and a concrete version survives only where the point is what happened to that version.

**No public surface names a beta image at all** — decided 2026-08-17. A beta stays pullable long after it
stops being the right thing to pull, and a published command that fails against it reads as our bug rather
than as history. Agent docs under `.agents/` may name one, and have to: the bands in D3 mean nothing without
the numbers.

### D5 — the reference layer is checked by a dated query, and the query is the fragile part

Every file under `.agents/docs/` and `.agents/design/` carries `verified:` (when someone last confirmed it
against the code) and `paths:` (the code it describes). A file whose `paths:` have been committed to since its
`verified:` date has not been proven wrong — it has been proven *unconfirmed*, which is the only signal there
is, because **a stale doc reads exactly like a current one**.

This is what derives that list. It runs from the repo root and prints every file it could not check, which is
the load-bearing half:

```bash
for f in $(find .agents/docs .agents/design -name "*.md" ! -name "_index.md" | sort); do
  fm=$(awk '/^---[ \t]*$/{n++; next} n==1' "$f")
  v=$(printf '%s\n' "$fm" | grep -m1 "^verified:" | sed 's/verified:[ \t]*//;s/"//g')
  pl=$(printf '%s\n' "$fm" | sed -n '/^paths:/,$p' \
        | grep -oE '^[ \t]*-[ \t]*"[^"]+"' | grep -oE '"[^"]+"' | tr -d '"')
  if [ -z "$v" ]; then echo "NO-VERIFIED  $f"; continue; fi
  if [ -z "$pl" ]; then echo "NO-PATHS     $f  (verified $v)"; continue; fi
  last=$(git log -1 --format=%ad --date=short -- $pl 2>/dev/null)
  if [ -z "$last" ]; then echo "NO-COMMITS   $f  paths=[$(echo $pl | tr '\n' ' ')]"; continue; fi
  [[ "$last" > "$v" ]] && printf "BEHIND       %-42s verified %s  code moved %s\n" "$f" "$v" "$last"
done
```

A clean run prints exactly two lines, both deliberate: `design/README.md` (navigation, claims nothing about
code) and `design/vipaq/history.md` (frozen at the date it was measured, and path-less on purpose so a live
session is never handed superseded numbers). **Anything else in the skip list is a hole, not a result.**

**This decision is not watched by its own query.** Its subject is the reference layer, and a `paths:` broad
enough to cover that would fire on every edit to it. Some claims are not expressible as a pathspec; saying so
beats a glob that matches everything.

#### What the 2026-08-19 sweep established

Thirty-two files across eight slices were re-verified in one pass. Five things generalise, and they are the
reason this is a decision rather than a closed task:

1. **Widen the `check:` while verifying.** Every slice's real errors sat just *outside* what its check asked
   for. A check naming three types is satisfied by three types while the fourth paragraph rots. The check is
   part of the deliverable, not a label on it.
2. **Count anything the doc counts.** Wrong counts turned up in five separate files — frozen packs (716 vs
   721), tests (334 vs 380), solution projects, `.dcproj` files, `just` modules. A number is the cheapest claim
   to verify and the one nobody re-reads.
3. **Confirm a check is runnable, not just written.** `lib/findings` pointed its check at
   `BenchmarkDotNet.Artifacts/`, which `.gitignore` excludes — so the one instruction for confirming those
   numbers could never be followed by anyone but the machine that produced them. When a check names an
   artifact, confirm the artifact is committed.
4. **Distrust the tool before the data.** The query above was wrong twice, in three ways, and every failure
   presented as a clean result: a `sed` range that ran past the front matter and swept prose into the pathspec
   (hiding three files, overstating a fourth), and a `[[:space:]]` class that in this environment does not match
   `^[[:space:]]*-[[:space:]]*"` against `  - "api/**"` — so `pl` came back empty for every file and the query
   checked nothing while printing nothing.
5. **A checker must report what it skipped.** Two files — `docs/concepts.md` and `docs/README.md` — carried a
   `verified:` date and a `check:` naming code with **no `paths:` at all**, so nothing could ever have flagged
   them; `concepts.md` sat unwatched at 2026-07-15. They surfaced only once the skips were printed. "No output"
   and "nothing to report" are the same line on a terminal.

**Measured evidence a doc quotes is not renumbered when the world moves.** Both findings records quote dataset
sizes that were correct when measured and have since grown; they now say so and name the live count, rather
than restating splits that would need a re-run to be true. Re-dating a measurement is falsifying it.

### D8 — identity is not geometry, and most model duplication is kept on purpose

The `Binacle.Geometry` extraction is finished: one leaf holds the `IWith*` geometry family and the generic
concrete `Dimensions<T>` / `Coordinates<T>` / `Item<T>`.

**Identity is not geometry.** `IWithID` / `IWithReadOnlyID` and the read-only composites `IIdentifiableBin` /
`IIdentifiableItem` sit in `shared/src/Binacle.Packing/Abstractions/`, a layer above the leaf, and never went
into `Binacle.Geometry`. That layer is what made a leaf rename unnecessary — `Geometry` → `Primitives` or
`Core` was the alternative and was not needed. Which interface sits in which project is `$lib/models`.

**What was deliberately *not* reduced to a shared model**, and this list is the point of the record: lib
internal result models (internal ctors, immutable) · algorithm working types (they carry behaviour) · v3 DTOs
(frozen) · UIModule ViewModels (DataAnnotations + computed ID) · lib **internal** readonly-struct `Dimensions` /
`Coordinates` (value-type performance — they must stay structs).

**That list is settled, not an open question.** A later "reduce the duplication" pass that does not read it
will re-derive the same five answers from scratch, or take one of them the other way.

**TypeScript duplicates the model shapes on purpose.** TS is structurally typed, so the duplicates already
interoperate and nothing is broken; there is simply no single source. Worth revisiting only if the shapes
start to drift.

## Open

### O1 — what happens to `3.0.0-beta.1` and `3.0.0-beta.2` on Docker Hub

Both are still pullable. Under D4 no public surface names either, and under D3 neither passes the published
command — beta 1 was never signed, and beta 2 needs a string no page carries any more. So anyone who pulls one
gets a failure with nothing anywhere to explain it.

Deleting both tags once v3.0.0 is out is the clean end of it, and **it has a deadline**: Docker Hub tag
immutability is off today, and an immutable tag cannot be deleted. Not decided.
