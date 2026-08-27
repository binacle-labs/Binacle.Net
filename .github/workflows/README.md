# Workflows

Every GitHub Actions workflow, one file each. They call the same `just` recipes a maintainer runs, so a green
check and a local run mean the same thing.

**A step calls a recipe; it does not carry a script.** Anything longer than one command lives in
[`tooling/ci/`](../../tooling/ci/README.md), one file per operation, and the step is the one line that calls
it. That is what lets shellcheck read it and lets you run the same thing in a terminal.

Each file opens with a comment block explaining why it is shaped the way it is. **Read that before changing
one** - most of what looks odd here was deliberate.

## 🚦 What runs when

| Workflow | Fires on | What it does |
|---|---|---|
| `pull-request.yml` | Every pull request | Works out what changed, then runs the image tests, an image build, the site tests, the three site builds and the workflow lint. Its `gate` job is the only name branch protection holds |
| `release-docker-image.yml` | By hand, with the version typed in | The release: gate the version, tests, build and push to GHCR, smoke, copy to Docker Hub by digest, then the git tag, the GitHub release and the Docker Hub page |
| `deploy-docs-site.yml` | By hand | Runs the site tests, builds the docs site, checks its links offline, deploys to Cloudflare, tags the commit it published |
| `deploy-demo-site.yml` | By hand | The same for the demo site |
| `deploy-www-site.yml` | By hand | The same for the marketing site |
| `sonar-analysis.yml` | By hand | Coverage to SonarCloud. Keep Automatic Analysis off in the Sonar UI - the two fight |
| `codeql-analysis.yml` | Merge to `main`, weekly, by hand | Code scanning. Findings land in the Security tab, not on a check |

## 🔄 The `shared-` files

A `shared-` prefix means **another workflow calls this one**. They are not private - each keeps its own
manual trigger, because running one by hand is the point.

| Workflow | Called by | Also runnable by hand for |
|---|---|---|
| `shared-image-tests.yml` | The pull request gate, the release | Running every test the Docker image ships, plus the OpenAPI lint, against a branch |
| `shared-site-tests.yml` | The pull request gate, all three deploys | Running every test a Jekyll site ships - the ten gems and the javascript packages |
| `shared-smoke-image.yml` | The release | Smoking any published tag - it must test a **published** image, not a local build |
| `shared-dockerhub-overview.yml` | The release, as its last job | Fixing the wording on the Docker Hub page without cutting a release |

## ⚠️ Five that are easy to get wrong

- **The image build stays out of the test suite.** The release calls that file whole, so anything added there
  is paid for twice - once per pull request and again on every release.
- **A gem test never goes in the image suite.** The ten Jekyll plugins ship in the sites and never in the
  image, so they belong in `shared-site-tests.yml`.
- **The release makes its tag last, and nothing here fires on a tag.** The version is an input; the workflow
  tags the run's own commit once the image is published, so a run that goes red leaves nothing to delete.
- **Never rebuild the image between build and publish.** The copy to Docker Hub is by digest, which is what
  makes the published image the exact one the smoke suite passed.
- **The `publish` job never checks out**, so it is the one place a script still sits inline. Giving it a
  checkout would put repository code beside the Docker Hub credential, which is the trade this shape avoids.

The composite actions these call live next door in [`../actions`](../actions).
