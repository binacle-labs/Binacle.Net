# Design

The settled design *behind* the docs — the **decisions** (why we built it this way) and the **findings**
(the measured evidence). Permanent and maintained like docs, with the same `verified:` / `check:` front matter.

Design is **not** documentation. Docs say *what the code is and how to use it*; design says *why it is shaped
this way and what proved it*. Keep the two apart: a description of current behaviour is a doc; a decision record
or a benchmark result is design. Design also owns the **history** — superseded evidence and reversed decisions —
which can go in a dedicated `<slice>/history.md`. Like everything here, group by slice — `design/vipaq/`, etc.

## Anchors

Decision and option headings carry short labels (`D1`, `O1`) that double as section anchors, so a section can be
cited as `$vipaq/decisions#D16`. Give every file a stable `id:` in its front matter so there is something to cite.

**The id before the `#` is the file that holds the anchor, and it is easy to get wrong.** `$vipaq` is the ViPaq
*doc*; the decisions live in `$vipaq/decisions`. A citation like `$vipaq#D16` resolves to a real file and then
to an anchor that is not in it, so it reads as valid and lands nowhere.

## A ledger that grows long becomes a folder

The maintainer, 2026-09-29: "deffinately over 1000 lines". Past that, a slice's `decisions.md` becomes a
folder, `decisions/`, with one file per decision:

- `decisions/README.md` keeps `id: <slice>/decisions` and the ledger's opening text.
- Each entry is `d<n>-<slug>.md` (`o<n>-` for an open question) with `id: <slice>/decisions/D<n>`, its own
  `description:`, `check:` and `paths:`, and a `status:` of `decided`, `pending` or `open` in place of the
  `## Decided` / `## Pending` / `## Open` sections.
- It is cited as `$ci-cd/decisions/D16`, and a section inside it as `$ci-cd/decisions/D8#sites-in-scope`.
- The generated index is the list of entries. Do not keep a second one by hand.

`design/ci-cd/decisions/` is the one ledger split this way.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate with `just agents all` after adding,
renaming or re-describing a file.
