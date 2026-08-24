---
description: No linter exists for any TypeScript or JavaScript in the repository - decide whether one lands, and what it gates
state: idea
waits-on: "a yes or a no, and it is not urgent - nothing is broken by the gap"
paths:
  - "packages/**"
  - "sites/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "vipaq/packages/**"
---

# No linter for the TypeScript

**There is no eslint, no prettier and no biome anywhere.** Style in the TypeScript is whatever the last
person typed, and the only thing holding it is `.editorconfig` and `strict` in six copies of
`tsconfig.json`.

**Ruby is not in the same position.** `ruby/.rubocop.yml` covers every gem and the style it enforces is
written down. What has never happened there is a run - `$plans/todos` carries that, and it is a different
gap from this one.

**Nothing is broken.** This is worth doing because the next person to touch a package cannot tell what the
house style is without reading four files and guessing, not because anything is wrong today.

## What the answer has to settle

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

- [ ] One tool is chosen and the reason is written down, or the answer is a recorded no.
      **By eye.** Either a config file exists at the root, or this plan is deleted and the no is a row
      in `$decisions`.
- [ ] The indentation conflict is settled in `.editorconfig`, not left for an editor to lose.
      `grep -n 'indent_style' .editorconfig` agrees with the tool's config.
- [ ] A `just` recipe runs it, and `just check` lists that recipe.
      `just check` shows it, and it exits non-zero on a seeded violation.
- [ ] Whether it gates a pull request is answered in the workflow, not only here.
      `grep -rn 'lint' .github/workflows/` - either a step names it, or nothing does on purpose.
