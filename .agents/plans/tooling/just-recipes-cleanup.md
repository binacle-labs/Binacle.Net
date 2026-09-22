---
description: "The just modules get the fixes the bench module got - recipes listed in file order, a word checked by just before anything runs, short scripts folded back into their recipe, one way to name a private recipe"
state: proposed
waits-on: "a yes from the maintainer, item by item. State picked by an agent to make the file legible; strike it if wrong"
horizon: undecided
paths: ["tooling/*.just", "tooling/*.sh", "tooling/image/*.sh", "justfile"]
---

# Just recipes cleanup

The bench module was reworked on 2026-09-22. A review of the other modules found the same faults. Nothing
here was run; every line comes from reading the files.

The rule the work follows: a short body - set a variable or two, then run one command - stays in the recipe.
Loops, `case` tables and many steps go in a `.sh` file, so shellcheck reads them. No script is better than a
short one. Scripts under `tooling/ci/` stay: the workflows reach them through `just ci`, and the four
`ci/install-*.sh` are called by path from `.github/actions/`.

## 1. List recipes in file order

`just <module>` lists alphabetically, so a module's order is lost. Add `--unsorted` to `default` in every
module and the root `justfile`. `coverage.just` has no `default`, so a bare `just coverage` runs `all`;
it gets one. `smoke.just:37-38` has "# List the recipes" twice.

## 2. Check a word with `[arg(..., pattern=...)]`

just rejects a word that does not match before anything runs, with its own message. Today these check by
hand, or not at all:

| Recipe | Today | With a pattern |
|---|---|---|
| `smoke test`, `up`, `down` | `_check`, a `case` recipe called first | `pattern='minimal\|quickstart\|prod\|service\|full'`; `_check` goes |
| `tests` service-module integration | a `case` in the body | `pattern='\|Sqlite\|Postgres\|AzureStorage'` |
| `coverage all`, `run` | a `case` in `coverage.run.sh:13-19` | `pattern='cobertura\|sonar'` |
| `image verify` | a `case` in `image/verify.sh:15-24` | `pattern='all\|tags\|signature\|attestations\|metadata'` |
| `image up` | `just image up -d` takes `-d` as the stack name | a pattern on `name` |
| `serve api` | a `case` with aliases N, S, U, All | the four profile names; the aliases go |
| `agents generate-index` | none | `pattern='rules\|docs\|design\|plans\|memory'` |
| `check links`, the site word | none; fails late with "No artifacts/x" | `pattern='\|docs\|demo\|www'` |

## 3. Fold short scripts back in

- `changelog.check.sh` (18 lines) - calls the extract script, fails on an empty result, prints a count.
  About five lines inline.
- `image/dockerhub-overview.sh` (24 lines) - once item 2 checks the version, what is left is a `sed` and a
  guard. Borderline: the body has `{{`, which just needs escaped.

## 4. Bodies too long to stay inline

- `serve.just` `api` - about 38 lines, two `case` tables. Item 2 removes the first; what is left is short.
- `smoke.just` `all` - build or pull, then a loop over profiles, about 20 lines. A script, or dependencies
  that carry the build step.

## 5. Defaults nobody can type

The list shows a variable name where a value should be: `repo=hub_repo` (`image verify`),
`dir=openapi_dir` (`openapi`), `image=local_image` (`smoke`). `image verify` has four positional words, so
passing the last means typing all four.

## 6. One way to name a private recipe

Four styles today: `_lychee`, `_build-site`, `_dotnet_test`, and bench's `lib-algorithms-run`. Pick one.
Bench's follows the maintainer's call of 2026-09-22: every recipe starts with its slice.

## 7. Two silent failures

- `image/verify-metadata.sh` has no `set -e`; a failed `docker image inspect | jq` is ignored.
- `openapi.just:47` reads a folder name with `sed` when the file loads. If `versions.yml` changes shape it
  comes back empty and `check-all-copies` diffs the wrong path, without an error.

## What will bite

- The recipe name is what CI calls (`just ci <x>`, `just smoke test`, `just coverage all`, ...). Renaming a
  public recipe means changing the workflow in the same step.
- `[arg]` patterns are anchored and must match the whole word; an optional word needs the empty case (`'|a|b'`).
- Tested only on just 1.45.
