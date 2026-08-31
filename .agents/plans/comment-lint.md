---
description: A check that nothing outside the agent guidance directory points a reader into it
state: idea
waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
horizon: future
---

# Comment lint - stop code pointing at agent guidance

`CLAUDE.md` states the rule that nothing outside the agent guidance directory may point a reader into it, and
nothing checks it, which is how 27 sites accumulated. All 27 were fixed, so a check lands green on day one.
**Do not go looking for sites to fix** - confirm it catches nothing, then keep it that way.

## Research

### 2026-08-13 - measured, so it is not re-derived

- **Three arms, each blind to the other two.** Of the 27 sites, only a few named the directory: 14 named a
  guidance file with the path filed off, 2 used the `$` scheme, and **11 named no file at all** - a bare ref
  code. Build all three or it ships with a whole file type falling through.
- **Derive every list, never hardcode one.** Deriving from all guidance basenames first measured 94 hits and
  was dismissed; re-measured, the noise was `README.md` and the sites linking their own pages. **Derive the
  exclusions too** - drop any basename that also exists outside the guidance directory - and it comes out at
  77 names, 13 files, 14 lines, every one real. **Assert the derived lists are non-empty, loudly.** An empty
  list makes the check report clean forever, which is how it dies.
- **Scan comment text, never whole lines.** In C#, `:D16` is a number format. In shell, `"$packages"` is a
  variable and `packages` is a declared id. Restricting to commentary removes all four with no allow-list. Use
  `awk` per comment family, not `sed` - the C block form needs state across lines, and a violation inside
  `/* */` is exactly the hole a one-line `sed` leaves.
- **A `just` recipe, not Semgrep.** The job is regex over comment text. Adopt Semgrep the day a rule appears
  that a regex genuinely cannot express.
- **No `xargs`.** It returns 123 when a grep batch matches nothing, and six algorithm folders have spaces in
  their names. `grep` searching a directory directly has neither problem.
- **`CLAUDE.md` is exempt as a file**, and scan every tracked file type - `Directory.Packages.props` carried
  one.

### 2026-08-13 - all 27 sites were fixed

That is why the check lands green.
