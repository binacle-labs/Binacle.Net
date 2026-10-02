---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs: the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## General

```yaml
- file: architecture-graph.md
  description: "Why the repo's dependency graph is generated rather than declared, what no generator can see, what an InternalsVisibleTo grant means for the graph, and the heavier architecture tools that were surveyed and not taken."
  paths: ["**/*.csproj", "Directory.Packages.props"]
- file: decisions.md
  description: "General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact and that no docs page quotes a figure that expires, why the licence file keeps its name and why the root holds only one of them, why only the current docs version is indexable and old ones are bug-fix only, how the agent reference layer is kept honest against the code, and what was deliberately not reduced to a shared model, and the four project folders and what each may reference, and why measured numbers live in the slice - deterministic ones tracked and diffed, timing ones kept by hand."
  paths: ["NOTICE", "README.md", "SECURITY.md", "CHANGELOG.md", "Dockerfile", "CONTENT-TERMS.md", "sites/docs/**", "shared/src/Binacle.Packing/**"]
- file: sonar-accepted-findings.md
  description: "The Sonar findings answered with a reason rather than a code change, why each one stands, and why this register has to live in the repository rather than in the SonarCloud UI."
  paths: ["lib/**", "packages/cookies/**", "api/src/Binacle.Net.Kernel/OpenApi/**"]
```

## API

```yaml
- file: api/decisions.md
  description: "API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, why the API sends no HSTS header, why the DiagnosticsModule alone is registered unconditionally, why an unknown enum answers with the same error a missing one does, why the shipped image calls the experimental v4 API, and why the instance page renders its presets from a startup snapshot rather than a live provider."
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/decisions/README.md
  description: "CI/CD decisions ledger - one file per decision; the index lists them"
- file: ci-cd/decisions/d1-release-dispatched-with-version.md
  description: "a release is dispatched with a version, and the tag and the release are its last job"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", "tooling/ci/check-release-ref.sh", "tooling/ci/check-version.sh", "tooling/ci/check-release-tag.sh", "tooling/ci/github-release.sh"]
- file: ci-cd/decisions/d10-npm-ci-ignore-scripts.md
  description: "npm ci --ignore-scripts"
  status: pending
  paths: [".github/workflows/**", ".github/actions/**"]
- file: ci-cd/decisions/d11-actions-pinned-by-sha.md
  description: "every action is pinned by commit SHA, and Dependabot keeps the pins moving"
  status: pending
  paths: [".github/workflows/**", ".github/actions/**", ".github/dependabot.yml"]
- file: ci-cd/decisions/d12-framework-dependent-image.md
  description: "the image is framework-dependent, and the publish flag is spelled out"
  status: pending
  paths: ["tooling/build.just", "Dockerfile"]
- file: ci-cd/decisions/d13-oci-labels-at-build-time.md
  description: "per-build OCI labels are applied at build time, never as LABEL fed by ARG"
  status: pending
  paths: ["Dockerfile", "tooling/build.just", ".github/workflows/release-docker-image.yml"]
- file: ci-cd/decisions/d14-ghcr-is-staging.md
  description: "GHCR is staging, and only the release workflow touches it"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", ".github/workflows/shared-smoke-image.yml"]
- file: ci-cd/decisions/d15-sbom-provenance-keyless-signing.md
  description: "the image carries an SBOM and provenance, and is signed keyless"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", "tooling/image/verify-signature.sh", "SECURITY.md"]
- file: ci-cd/decisions/d16-lychee-pinned-binary.md
  description: "lychee is installed as a pinned binary, not through its own action"
  status: pending
  paths: [".github/actions/install-lychee/**", "tooling/ci/install-lychee.sh", "tooling/check.just", "tooling/check.lychee.toml"]
- file: ci-cd/decisions/d17-site-deploys-by-hand.md
  description: "the site deploys are published by hand, and never on a push"
  status: pending
  paths: [".github/workflows/deploy-site.yml"]
- file: ci-cd/decisions/d18-two-test-suites-split.md
  description: "two test suites, split by what ships"
  status: pending
  paths: [".github/workflows/shared-image-tests.yml", ".github/workflows/shared-site-tests.yml", "tooling/tests.just", "tooling/ci/changed-paths.sh"]
- file: ci-cd/decisions/d19-coverage-drops-test-support.md
  description: "the merged coverage report drops the test-support assemblies"
  status: pending
  paths: [".netconfig", "tooling/coverage.just"]
- file: ci-cd/decisions/d2-build-once-copy-by-digest.md
  description: "build once, smoke the registry copy, then copy by digest"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", "tooling/ci/copy-tags.sh"]
- file: ci-cd/decisions/d20-codeql-buildless-merge-only.md
  description: "CodeQL runs buildless, on merge only, and reports nothing on a check"
  status: pending
  paths: [".github/workflows/codeql-analysis.yml", "tooling/ci/codeql-summary.sh"]
- file: ci-cd/decisions/d21-image-verify-user-run.md
  description: "just image verify is what a user runs, and it must stay that way"
  status: pending
  paths: ["tooling/image.just", "tooling/image/**"]
- file: ci-cd/decisions/d22-gems-reach-sonar-built-project.md
  description: "the gems reach Sonar through a built project, and the project type is what decides that"
  status: pending
  paths: ["ruby/ruby.rbproj", "Binacle.Net.slnx", "tooling/ci/sonar-analysis.xml"]
- file: ci-cd/decisions/d23-one-required-check.md
  description: "one required check, and the maintainer bypasses it"
  status: decided
  paths: [".github/workflows/pull-request.yml", "tooling/ci/gate.sh"]
- file: ci-cd/decisions/d24-release-tags-immutable.md
  description: "release tags cannot be moved or deleted, and nobody bypasses that"
  status: pending
  paths: ["tooling/ci/github-release.sh", "tooling/ci/create-tag.sh"]
- file: ci-cd/decisions/d25-moving-tags-proven-on-release.md
  description: "the moving tags were proven on the release itself, not on a scratch repository"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", "tooling/ci/moving-tags.sh"]
- file: ci-cd/decisions/d26-dockerhub-tag-immutability-off.md
  description: "Docker Hub tag immutability stays off"
  status: decided
  paths: [".github/workflows/release-docker-image.yml", "tooling/ci/moving-tags.sh"]
- file: ci-cd/decisions/d27-released-version-never-deleted.md
  description: "a released version is never deleted, and prereleases stop reaching the public repository"
  status: pending
  paths: [".github/workflows/release-docker-image.yml", "tooling/image/verify-tags.sh", ".github/dockerhub-overview.md"]
- file: ci-cd/decisions/d28-sonar-pr-parallel-non-blocking.md
  description: "Sonar runs on every pull request that can carry the token, parallel and non-blocking"
  status: decided
  paths: [".github/workflows/sonar-analysis.yml", ".github/workflows/pull-request.yml", "tooling/ci/sonar-analysis.xml", "tooling/ci/sonar-summary.sh"]
- file: ci-cd/decisions/d29-dockerhub-credential-no-environment.md
  description: "the Docker Hub credential is not scoped to an environment"
  status: decided
  paths: [".github/workflows/release-docker-image.yml", ".github/workflows/shared-dockerhub-overview.yml"]
- file: ci-cd/decisions/d3-prerelease-stays-on-ghcr.md
  description: "a prerelease stays on GHCR, and still gets its tag and a GitHub prerelease, through two job conditions"
  status: decided
  paths: [".github/workflows/release-docker-image.yml", "tooling/ci/check-release-ref.sh", "tooling/ci/github-release.sh", "tooling/ci/changelog-section.sh", "tooling/ci/release-summary.sh"]
- file: ci-cd/decisions/d30-no-git-credential-after-checkout.md
  description: "no job holds a git credential after checkout"
  status: decided
  paths: [".github/workflows/**", "tooling/ci/create-tag.sh"]
- file: ci-cd/decisions/d31-path-filter-names-github-files.md
  description: "the site half of the path filter names the .github/ files a site depends on"
  status: decided
  paths: ["tooling/ci/changed-paths.sh"]
- file: ci-cd/decisions/d32-one-site-deploy-workflow.md
  description: "the three site deploys are one workflow with a site chosen at dispatch"
  status: decided
  paths: [".github/workflows/deploy-site.yml"]
- file: ci-cd/decisions/d33-dockerhub-oidc-login.md
  description: "the release logs into Docker Hub with the run's OIDC token"
  status: decided
  paths: [".github/workflows/release-docker-image.yml", ".github/workflows/shared-dockerhub-overview.yml"]
- file: ci-cd/decisions/d4-step-calls-just-recipe.md
  description: "a workflow step calls a just recipe, it does not inline the command"
  status: pending
  paths: [".github/workflows/**", ".github/actions/**", "tooling/ci.just", "tooling/ci/**"]
- file: ci-cd/decisions/d5-no-duplicate-repo-variable.md
  description: "a repo variable may not duplicate a fact that lives in the repo"
  status: pending
  paths: [".github/workflows/**"]
- file: ci-cd/decisions/d6-smoke-pins-ubuntu-24-04.md
  description: "shared-smoke-image.yml pins ubuntu-24.04, everything else takes ubuntu-latest"
  status: pending
  paths: [".github/workflows/shared-smoke-image.yml"]
- file: ci-cd/decisions/d7-changelog-single-source-notes.md
  description: "CHANGELOG.md is the single source of release notes, and a missing section is fatal"
  status: pending
  paths: ["CHANGELOG.md", "tooling/changelog.just", "tooling/changelog.*.sh", "tooling/ci/changelog-section.sh", "tooling/ci/github-release.sh"]
- file: ci-cd/decisions/d8-sonar-ci-run-automatic-off.md
  description: "Sonar analysis is a CI run, Automatic Analysis stays off, and the published sites are in scope"
  status: pending
  paths: [".github/workflows/sonar-analysis.yml", "tooling/ci/sonar-analysis.xml"]
- file: ci-cd/decisions/d9-postgres-service-no-password.md
  description: "the Postgres service in shared-image-tests.yml carries no password"
  status: pending
  paths: [".github/workflows/shared-image-tests.yml"]
- file: ci-cd/decisions/o2-pull-request-gate-proof.md
  description: "open question: how much the pull-request gate should prove"
  status: open
  paths: [".github/workflows/pull-request.yml", "tooling/ci/sonar-analysis.xml"]
- file: ci-cd/decisions/o3-multi-arch-absent.md
  description: "open question: multi-arch is still absent"
  status: open
  paths: [".github/workflows/release-docker-image.yml", "Dockerfile"]
- file: ci-cd/github-surface.md
  description: "What GitHub offers a repository, what this one uses, and the ten Actions gotchas that fail quietly"
  paths: [".github/**"]
```

## Lib

```yaml
- file: lib/decisions.md
  description: "Lib decisions ledger — why Algorithm.Best races a different set per path, where the packing vocabulary lives, why there are two data hubs, why old algorithm versions are kept, the parallel bench shape (a class per thing raced, one grid per scenario kind, why the line classes measure an expected nothing), how a kept baseline run is removed or replaced rather than dated and what a skip mark carries, and the open parallelization question."
  paths: ["lib/**"]
- file: lib/findings.md
  description: "Lib findings — the measured evidence (algorithm racing cost, parallel algorithm racing, parallel bin processing) behind the decisions."
  paths: ["lib/**"]
```

## Packages

```yaml
- file: packages/decisions.md
  description: "Packages decisions ledger - why the v4 client is hand-written rather than generated, why the visualizer owns its own internals instead of sharing a utils folder, and why the request panel is the UI module's and the component only hands over the request."
  paths: ["packages/**"]
```

## Ruby

```yaml
- file: ruby/decisions.md
  description: "Ruby gem decisions ledger — why one computation feeds every gem, the one key whose generator order is load-bearing, how a gem's config names are chosen, what the gem name says about portability, why a portable gem may not name the product, and what decides a tag against a generator."
  paths: ["ruby/**"]
```

## Sites

```yaml
- file: sites/decisions.md
  description: "Decisions behind the demo and documentation sites — the link-preview pair, title order, what the demo host calls itself, why the demo has no collections, the two footer calls, and why the docs site keeps one folder per major with the current line at the root. What a review would otherwise re-litigate."
  paths: ["sites/demo/**", "sites/docs/**"]
- file: sites/demo-and-image-boundary.md
  description: "The two demo tools ship on two hosts from one implementation - what is shared, what diverges freely, and the test that keeps the line where it is"
  paths: ["sites/demo/**", "packages/binacle-net-ui/**", "api/src/Binacle.Net.UIModule/**"]
- file: sites/docs-and-demo.md
  description: "Why the docs and demo templates are shaped this way - the beercss and Alpine traps, the contrast measurements behind the component overrides, and the asset budget."
  paths: ["sites/demo/**", "sites/docs/**"]
- file: sites/packing-demo-set.md
  description: "Why the packing demo sizes its items against the largest bin, and how sizingBin and addBin relate - the reasoning behind the numbers a visitor arrives to"
  paths: ["packages/binacle-net-ui/**"]
- file: sites/site-build.md
  description: "The webpack and sass machinery behind the three sites - the clean rule that keeps a watch alive, the chunk split and the asset budget, and the cache that lies about a clean build."
  paths: ["sites/www/webpack.config.js", "sites/docs/webpack.config.js", "sites/demo/webpack.config.js", "sites/*/package.json"]
- file: sites/site-config-and-data.md
  description: "What the three sites' _config.yml and _data files carry that a reader cannot recover - the version_tag trap, the cookie domain, the organisation block, and how the www exchange payloads are re-run."
  paths: ["sites/www/_config.yml", "sites/docs/_config.yml", "sites/demo/_config.yml", "sites/*/_config.prod.yml", "sites/*/_data/**"]
- file: sites/site-theming-css.md
  description: "The CSS side of the theme on all three sites - why the attribute is on html, why every dark block also lands on body, and the two rules that keep the switcher element from being a box or a dead control."
  paths: ["sites/www/_sass/**", "sites/docs/_sass/**", "sites/demo/_sass/**"]
- file: sites/webmanifest.md
  description: "Why the three sites ship a web app manifest rather than dropping the two android icons, where its colours come from, and why the UI module gets neither."
  paths: ["sites/**", "gulpfile.js"]
- file: sites/www-stylesheets.md
  description: "The www stylesheet system - the palette and its measured contrast numbers, the separation rhythm, the one-column hero and why the two-column version was dropped, and the exchange card's scroll behaviour."
  paths: ["sites/www/_sass/**", "sites/www/_js/**"]
- file: sites/www.md
  description: "Why the www site's templates are shaped the way they are - the traps that bite silently, and the constraints a rewrite would break without noticing."
  paths: ["sites/www/**"]
```

## ViPaq

```yaml
- file: vipaq/decisions.md
  description: "ViPaq decisions ledger — the decisions and their reasons, plus the open questions."
  paths: ["vipaq/**"]
- file: vipaq/findings.md
  description: "ViPaq findings — the measured evidence (base64 size, encode/decode time) behind the decisions."
  paths: ["vipaq/**"]
- file: vipaq/history.md
  description: "ViPaq design history — superseded throwaway-prototype measurements (2026-07-05), the earlier framings of decisions that were later amended or reversed, and where the test files the v2 rebuild deleted ended up. Reference only, not current truth."
```
