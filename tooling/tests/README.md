# Tests

The three test runners behind `just test` - one per language. `tooling/tests.just` holds the names and the
lists; each name is a door onto one of these with the project, gem or package as its argument.

Nothing is written inside the `.just` file, because a recipe body cannot be run on its own and cannot be
handed to shellcheck. A `.sh` file is both.

## 📂 What is in it

| File | What it does |
|---|---|
| `dotnet.sh` | Runs one C# test project. Appends `DOTNET_TEST_ARGS`; takes a label for a project that runs more than once, so the two runs do not overwrite each other's files |
| `rspec.sh` | Runs one gem's specs from inside the gem folder, against the Jekyll `ruby/Gemfile` locks |
| `jest.sh` | Runs one TypeScript package by its jest `displayName` |
| `ruby-coverage.rb` | Not a script - what `rspec.sh` loads through `RUBYOPT` to start SimpleCov before the gem is required. Never loaded by a gem |

## 🚀 How you run one

Through the `test` module: `just test cs_binacle-lib_unit`, `just test rb_jekyll-filters_unit`,
`just test ts_cookies_unit`. `just test` with no argument prints every name.

Each runner has two modes. With `COVERAGE_FORMAT` unset it runs the tests and nothing else. With it set to
`cobertura` or `sonar` the same run also writes a test report to `artifacts/tests/` and a coverage file to
`artifacts/coverage/<format>/`, one flat file per project - `just coverage all` is what sets it.

## ⚠️ What will bite you

**Every path is written from the repo root**, and the module sets `working-directory` so a recipe starts
there. Run a script by hand from the root too.

**`expected.txt` is written before the run**, so `just coverage table` can tell "wrote no report" from
"never ran".

**`rspec.sh` runs without `-e` on purpose.** The exit code is kept, so the coverage file still gets moved out
of a failed run.

**Runner options for `dotnet test` go straight on the command line, never after a `--`.** `dotnet test`
hands anything it does not recognise to the test app, and a `--` makes it read the project path as junk.

**They must pass `shellcheck` clean.** `just check scripts` covers this folder.
