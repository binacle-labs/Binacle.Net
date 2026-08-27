---
description: Answered no - linting is one decision for the whole repository, not a TypeScript one. Every language gets the same treatment or none does.
state: deferred
waits-on: "a decision to lint every language in this repository to the same standard - TypeScript alone is not the question"
paths:
  - "packages/**"
  - "sites/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "vipaq/packages/**"
---

# Linting is a whole-repository decision

**Answered no on 2026-08-27, and the reason is the shape, not the tool.** The maintainer's words: *"then no
we don't do. There is no linter for C#, there is for Ruby, but no - not yet, all need same treatment."*

**Do not reopen this as a TypeScript question.** Adopting a linter for one of three languages leaves the
repository with three different answers to the same question. **What revives this file is a decision to lint
every language here to the same standard.** Then the material below is the TypeScript half of it.

## Where each language actually stands - checked 2026-08-27

- **TypeScript and JavaScript: nothing.** No eslint, no prettier, no biome anywhere. Style is whatever the
  last person typed, held only by `.editorconfig` and `strict` in six copies of `tsconfig.json`.
- **Ruby: a config, never run.** `ruby/.rubocop.yml` covers every gem and writes down the style it enforces.
  rubocop is in `ruby/Gemfile`, and nothing calls it - no `just` recipe and no pipeline step. It lands red
  before it lands green.
- **C#: no in-build linter, but not nothing.** There are no analyzer packages, no `EnforceCodeStyleInBuild`,
  no `AnalysisMode` and no `TreatWarningsAsErrors`. The single root `.editorconfig` carries formatting rules
  and exactly one `dotnet_diagnostic` line, and it is a suppression (`IDE0130`, scoped to the ViPaq unit
  tests). **What C# does have is SonarCloud**, run by `.github/workflows/sonar-analysis.yml` on merge, with
  its rule set tuned in `Directory.Build.props` via `SonarQubeTestProject`. CodeQL runs beside it, security
  only. **So the C# gap is a local, in-build style linter - not quality analysis, which already runs.**

**Nothing is broken.** The cost is that the next person to touch a package cannot tell what the house style is
without reading four files and guessing.

## What the TypeScript half would have to settle

**Kept because it is measured work, not because the question is open.**

- **Which tool.** `prettier` plus `typescript-eslint` is what most of the world runs. `biome` is one binary
  for both and much faster. Either is defensible; running both is not.
- **What it covers.** Four packages, three sites, the UI module and the ViPaq mirror - eight roots, all
  written at different times.
- **The indentation conflict, and it is real.** `.editorconfig` sets tabs at width 4 for every `*.ts`.
  Prettier's default is two spaces, and biome's is tabs. **An editor obeys `.editorconfig`, so the two
  fight on every keystroke until one of them gives.** Either the config changes, or the tool is told to
  match it - and matching it means giving up the defaults, which is most of the reason to adopt a tool.
- **Whether it gates.** A gate on a red codebase blocks every pull request from the day it lands. The
  honest order is: land it reporting only, fix in slices, then gate.
- **What the first run costs.** Nobody has measured it. **Run the tool once before deciding anything above**
  - if it comes back with four findings the decision is small, and if it comes back with four hundred it is
  a project.

## Done when

**None of these open while the file is deferred.** They describe what landing a linter across the repository
would look like, so that reviving it does not start from a blank page.

- [ ] Every language here has an answer, and it is the same answer.
      **By eye.** TypeScript, Ruby and C# each either run a linter or are written down as deliberately not
      linted. One language settled and two not is the state this file exists to refuse.
- [ ] The indentation conflict is settled in `.editorconfig`, not left for an editor to lose.
      `grep -n 'indent_style' .editorconfig` agrees with whatever tool config lands beside it.
- [ ] A `just` recipe runs each linter, and `just check` lists them.
      `just check` shows them, and each exits non-zero on a seeded violation.
- [ ] Whether they gate a pull request is answered in the workflow, not only here.
      `grep -rn 'lint' .github/workflows/` - either a step names it, or nothing does on purpose.
