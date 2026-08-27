# Agent index generator

What `just agents all` runs. It rewrites the `_index.md` manifest for each guidance directory - a list of
every markdown file under it, grouped by area, with the description from each file's front matter.

Nothing in CI calls this. It is a laptop command, which is why it is python with a real config parser rather
than shell.

## 📂 What is in it

| File | What it is |
|---|---|
| `generate-index.py` | The generator. Reads `indexes.toml`, walks the directory, writes the manifest |
| `indexes.toml` | What it writes - one table per manifest, plus the group names and the front matter keys |

## 🚀 Running it

From the repo root:

```bash
just agents all                      # rewrite every manifest
just agents generate-index plans     # rewrite one
```

Run it after adding, renaming, moving or re-describing any file in one of those directories.

## ⚠️ What will bite you

**A directory is indexed only if it has a table in `indexes.toml`.** Add one there and it joins the set;
without one it is skipped in silence.

**Entries sort by byte order, so capitals come first.** That is deliberate: the old shell version used the
machine's `sort`, which follows the locale, and two people regenerating on differently configured machines
got two different files.

**The generator needs no packages.** It reads TOML with `tomllib`, which is in the python standard library
from 3.11.
