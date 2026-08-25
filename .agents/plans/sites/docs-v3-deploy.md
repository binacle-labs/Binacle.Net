---
description: The v3.0.x corrections the docs site needs and the deploy that publishes them - five pages describing configuration the image no longer ships, a worked example quoting a deleted tag, and two stale OpenAPI copies
state: ready
waits-on: "the v3.0.0 tag - three of its items quote the released tag, its date or its digest"
paths:
  - "sites/docs/**"
---

# The v3.0.x docs corrections and the deploy

**A docs session** - `sites/docs/` is published, so a coding session does not touch it.

**Steps 1, 2, 5 and 6 are done, 2026-08-25, and step 4's content half with them.** What is left needs the tag
to exist: the worked example in step 3, the release date and link in step 4, and the deploy. **Plus one
decision, at the bottom of this file.**

**The config half is already done on `main`** - `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`,
and the version sitemap restored. Verified 2026-08-14.

---

## 1. Read the rebuilt site before dispatching anything

The gem work changed pages in all four version folders - `v1.3.x`, `v2.0.x`, `v2.1.x` and `v3.0.x` - plus the
landing, the 404, the sitemaps, `robots.txt`, the typography and the code-block styles. **A deploy publishes
every one of those, not only the `v3.0.x` edits below.**

Four things a build settles and nothing else does:

- Every version's pages still carry a title, a description and a canonical.
- `/version/latest/` still redirects, is still `noindex`, and its canonical still points at
  `/version/<current>/` rather than at itself.
- The three sites' `robots.txt` files are byte identical to each other apart from the host in the `Sitemap:`
  lines, and each is byte identical to what it was before.
- The site serves and links a web app manifest, and the icon paths in it resolve. **Check it in a browser's
  application panel** - a wrong icon path fails with no console error.

## 2. The UI module reads no configuration, and five pages say it does

The rebuild deleted the `BinacleApi` connection string and `Config_Files/UiModule/` with it. **All five files
are wrong for 3.0.0, not merely stale.** Confirmed by grep on 2026-08-24.

- **`configuration/ui-module/index.md`** - delete the `## Configuration` block (the
  `/app/Config_Files/UiModule` sentence and the directory tree) and the whole configuration section at the
  bottom - the auto-detect paragraph, the `BinacleApi` JSON sample, and the link to *Configuration Basics >
  Connection String Fallbacks*. **What replaces them:** the module reads no configuration at all. Both demos
  run in the browser and call the API they are served from, over relative URLs, so there is nothing to point
  anywhere. `UI_MODULE=True` is the whole setup.
- **`configuration/index.md`** - drop the `UiModule` branch and its `ConnectionStrings.json` leaf from the
  directory tree, so it matches the image.
- **The three sample compose files** under `v3.0.x/samples/docker/` - `quickstart`, `full` and `service` -
  each set `BINACLEAPI_CONNECTION_STRING`. Delete that line, and in `service` the two comment lines above it.
  **Repo-root `samples/` has already had this done; copy from there.**

**Leave `v1.3.x`, `v2.0.x` and `v2.1.x` alone.** Those versions really did read the file. Only `v3.0.x` is
wrong.

## 3. Re-cut the worked example in `v3.0.x/verifying-a-release.md`

**It is broken, not merely stale.** The example verifies `3.0.0-beta.2`, which is deleted from Docker Hub and
was signed under the old owner anyway - so the block asks the reader to run a command that cannot succeed.
**This page must not deploy until this is done.**

Run `just image verify 3.0.0` and replace three things with what it prints: the Docker Hub digest, the package
count, and the provenance run URL. **It quotes real output, so it cannot be written before the tag exists.**

**The rule this came from, worth keeping: name a version where the version is the fact, never as a floor or an
example.** A floor or a sample tag goes stale on its own; a record of what was signed does not.

## 4. `v3.0.x/release-notes.md` - the date, the link, and what the changelog gained

- **The date and the release link.** The `## v3.0.0` section carries interim wording written before the tag
  existed. Swap the italic line for *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*,
  matching every other version folder.
- **Carry the additions from `CHANGELOG.md`.** Same notes in two places. **Decided 2026-08-14: this page stays
  hand-copied**, not generated, and the drift is the accepted cost - **which makes this list the control.**
  Run `just changelog extract 3.0.0` and read it rather than trusting the list below.

  The first four go in the `## v3.0.0` section, in the page's plain-ASCII style; the fifth is a migration step.

  - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the app
    user, so a volume mounted there is writable.
  - **Core Changes**, replacing *"The `Dockerfile` and existing environment variables are unchanged"* - which
    is false, the Dockerfile changed three times. Spell out the `/app/data` fix (docker created the mount point
    as root, the app does not run as root, so packing logs and the SQLite database could not be written to a
    fresh named volume); `libgssapi-krb5-2` now shipping, so Npgsql stops printing "Cannot load library
    libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI labels on the image; and
    only then "existing environment variables are unchanged".
  - **A Service Module section**, between Diagnostics and UI Module: the auth token rate limit partitions on
    the connection's remote address instead of a caller-supplied header, so varying the header no longer resets
    your own login throttle.
  - **A Diagnostics Module section**, carrying the `/_health` bullet. **It is bigger than "gained
    `ReservedPaths`".** At v2.1.1 that endpoint's `System` entry returned only `Processors`; `Version`,
    `Environment`, `StartedAt`, `Uptime`, `Features` and `ReservedPaths` are all new. **Copy the bullet from
    `CHANGELOG.md` rather than writing it again.**
  - **The whole UI Module section, plus one Overview bullet and one Core Changes bullet.** What the page needs
    is the rebuild itself (Blazor to Razor Pages, no SignalR circuit), the route changes `/PackingDemo` ->
    `/packing` and `/ProtocolDecoder` -> `/vipaq`, the Protocol Decoder renamed to ViPaq Decoder, the new
    `/instance` page, the randomizer, and that the module reads no configuration. The Core Changes bullet is
    the removal of `BINACLEAPI_CONNECTION_STRING`.
  - **Migration step 7, `Drop BINACLEAPI_CONNECTION_STRING`.** In `CHANGELOG.md` this pushed the `cosign
    verify` step to 8. **On the docs page nothing renumbers** - that page never carried the `cosign verify`
    step, so step 7 appends cleanly. Confirmed 2026-08-25.

## 5. Replace the two swagger documents

Copy `artifacts/openapi/Binacle.Net_v3.json` -> `v3.0.x/swagger/v3.json` and
`artifacts/openapi/Binacle.Net_v4.json` -> `v3.0.x/swagger/v4.json`. **The generator's file names differ from
what the site expects, so the rename is part of the copy.**

**Re-measured 2026-08-23.** The `servers` entry and the removal of the `429` responses are **already in the
committed files** - both carry `servers: [{"url": "/"}]` and zero `429`. Against a fresh `just openapi
generate`, sorted-key diffs show **one changed line each: `info.description`**, and it moved on purpose. Say
so wherever the update is described.

## 6. The ViPaq rename - done, and this step named the wrong file

**The docs landing never carried "Protocol Decoder"** - `git log -S` over it returns nothing. The only
`v3.0.x` page with the old name was `samples/docker/quickstart/index.md:40`, and that is where the rename
went, lowercase to match the page's own "packing demo".

**Open, and small:** if that page should use the canonical tool names, line 40 becomes "the Packing Demo and
the ViPaq Decoder" and line 47 goes with it.

## 7. Deploy

`workflow_dispatch` only.

**One deliberate 404, do not "fix" it.** The `v3.0.x` ViPaq page links the wire spec at a `blob/v3.0.0` path,
which 404s until that tag is pushed. **A versioned page should pin the spec it describes; do not repoint it at
`main`.**

## The one decision left, and it is not about v3.0.0

**The release notes page is behind `CHANGELOG.md` by more than this release added.** Found on 2026-08-25 while
carrying the six items across. **These were already in the changelog when the page was last written, so the
omission may be deliberate curation** - which is why it is a decision and not a defect.

Missing from the page:

- **Overview** - the image is signed and ships an SBOM and provenance; the image is about a third smaller; the
  project moved to a new organisation.
- **Core Changes** - the signing block with the `cosign verify` command, the moved signing identity, the image
  size drop, and that the description at the top of every API document changed.
- **Internal Work** - the rebuilt release pipeline, the `config/` -> `tooling/` and `build/` -> `artifacts/`
  renames, and the SHA-pinned actions.
- **Migration** - the changelog's step 8, updating a pinned `cosign verify` identity, has no counterpart.

**Two of them are not curation.** The `v3.0.x` folder ships a `verifying-a-release.md` page that assumes a
signed image, and a reader following a stale `cosign verify` identity gets a failure that reads as our bug.
**Signing and the identity should go on the page whatever is decided about the rest.**

## One heading icon differs from the changelog

The new Service Module heading uses the shield the rest of this page and `configuration/index.md` use.
`CHANGELOG.md` uses a plug. **Every other heading on the page matches the changelog, so this is the one
divergence** - the page was made consistent with itself rather than with the changelog. One character either
way.

## One byte, recorded so nobody chases it

`robots.txt` is **1367 bytes where it was 1368.** The gem's output no longer emits the leading blank line the
old hand-written file had, so the built file starts directly with `# As a condition`. **Content is otherwise
identical and the three sites still match each other byte for byte.** The gem is outside `sites/`. Harmless,
but the read-through check asks for a byte match against what was there before, and this is not one.

## What will bite

**Nothing watches the frozen OpenAPI copies.** `tooling/openapi.just` writes only to `artifacts/openapi`, and
`tooling/regen.just check` lists five globs, none of them the swagger folder. **This checklist is the only
control there is.**

**Nothing fails if the deploy is skipped.** The site just quietly keeps serving v2.1.x as current, and `main`
already says v3.0.x is current - so a skipped deploy presents an unreleased version as current and nothing
says so.

## Done when

- [x] No `v3.0.x` page describes UI module configuration. **Done 2026-08-25.**
      `grep -rln 'BINACLEAPI_CONNECTION_STRING\|Config_Files/UiModule' sites/docs/collections/_versions/v3.0.x/`
      returns `release-notes.md` and nothing else - **that page is meant to name the variable**, because it is
      where its removal is announced.
- [ ] The verifying-a-release example quotes the released image.
      `grep -n 'beta' sites/docs/collections/_versions/v3.0.x/verifying-a-release.md` returns nothing, and the
      command in it runs green from a clean shell.
- [x] The two swagger files match a fresh generate. **Done 2026-08-25 - both diffs are empty.**
      `diff <(jq -S . artifacts/openapi/Binacle.Net_v3.json) <(jq -S . sites/docs/collections/_versions/v3.0.x/swagger/v3.json)`
      and the same for v4.
- [ ] The release notes page carries the date, the release link and everything the changelog section has.
      **By eye**, against `just changelog extract 3.0.0`.
- [ ] The deployed site answers on v3.0.x and the four build checks in step 1 pass on the live host.
      **By eye**, plus `curl` on `/robots.txt` and `/version/latest/`.
