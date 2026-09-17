---
description: "Integration tests that exercise the module set the image ships, not core modules only"
state: idea
waits-on: "nobody - it is an idea. horizon: near - chosen by an agent, strike it if wrong"
horizon: near
paths:
  - "api/**"
---

# Integration tests that cover the module set the image ships

The integration harnesses boot the app with core modules only, so every module combination the image actually
ships goes untested end to end. Turn the optional modules on - Diagnostics, Service and UI - and cover the
behaviour that exists only because an optional module registered something. That behaviour is where a silent
break lives: a core endpoint keeps passing in the harness and fails in the image.

## Done when

- [ ] The optional modules are on in the harnesses and the three TODOs are gone.
      `grep -rn "Run the tests with all modules enabled" api/test` returns nothing.
- [ ] One run with everything on, or a matrix over the combinations that ship, is decided and written here.
      **By eye.** The answer is in this file, not in someone's head.

## Research

### 2026-07-29 - the gap, verified

The harnesses boot with `WebApplicationFactory` and core modules only. Three `// TODO` comments say so:

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:35`
- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`

In the core harness the pre-build configuration dictionary is literally empty, so Diagnostics, Service and UI
are all off.

**Rate limiting is the worked example of the shape.** A core behaviour that exists only because an optional
module registered something. `api/test/Binacle.Net.ServiceModule.IntegrationTests/RateLimiting/` answers in
code what this file asks in prose.

**The shipped presets are replaced.** Both core harnesses swap in three test-only presets, so no in-process
test ever reads `Config_Files/Presets.json`. Leave that alone - proving the shipped presets load is the
container suite's job.

### 2026-07-29 - split out of the smoke-testing work, and the boundary

They are different jobs. **Behaviour goes here, in process. Packaging goes to the container suite.**

"Does rate limiting return 429 when the module is on" is behaviour, and a failure points at a line of C#. "Is
`RateLimiter.json` in the image at all" is packaging. "Does CORS echo a configured origin" is behaviour. "The
shipped image has no `Cors.json`, so it allows no browser origin" is packaging.

### 2026-08-07 - the two questions depend on each other

"One run with everything on, or a matrix?" cannot be answered until "how many more cross-module dependencies
are there?" is answered, and the second is real research. Anyone who settles the shape early so they can get
on with writing tests produces exactly the mistake this file exists to avoid.

The three questions a pickup session answers first:

- **One run or a matrix?** If a matrix, which combinations - the ones the samples ship, or something smaller?
- **How many more cross-module dependencies are there?** That list decides the answer above. The search is
  core code that only works because an optional module registered something: `RateLimited`, `RequireCors`,
  `RequireAuthorization`, and any middleware a module adds to the shared pipeline.
- **Does anything break when the modules go on?** If existing assertions have to change, which and why.

### 2026-09-11 - the CI half, folded in from the pull-request plan

**The integration suites already run on every pull request.** `shared-image-tests.yml` names five of them as
its own steps - the UI module's, the core one, and the Service Module's on Sqlite, Postgres and AzureStorage -
rather than calling a group recipe. True since `d0ba7823`. A separate plan claimed otherwise until it was
corrected and deleted on 2026-09-11; **the run was never the gap, the module set is.** So this file owns the
whole of what is left, and there is no CI work to do beyond whatever turning the modules on requires.

### 2026-08-27 - the maintainer agreed the split

His word was *"agreed"* on splitting the investigation from the build.

### Date not recorded - what will bite

- **A live rate limiter will make other tests flaky.** The anonymous partition key is a constant, so it is one
  bucket for every anonymous caller in a host, and it does not refill inside a run. Do not turn the limiter on
  in a shared harness. The rate limiting tests build their own host per test for this reason.
- **Test-host configuration goes through an env var the harness reads, never a `.runsettings` file.** The
  Microsoft Testing Platform runner ignores VSTest runsettings. `BINACLE_TEST_INFRA` already works this way.
- **Turning UI and Service on changes the route table**, so a test asserting a 404 for an unknown path may
  start hitting a real endpoint. Treat each one as a question rather than a fix - a 404 that becomes a 200
  might be the bug this work exists to find.
- **The ServiceModule harness disables the auth-token limiter on purpose.** Turn it back on and the auth tests
  need to account for it.
- **Whatever runs the all-modules suite must not be added to `shared-image-tests.yml`.** The release calls
  that file whole and takes no inputs, so a step added there is a step every release pays for. It belongs in
  `pull-request.yml`. This is the one trap the deleted pull-request plan carried that was not already here.
- **The Azure Storage provider has in-process coverage and nothing else.** `shared-image-tests.yml` runs the
  Service Module integration suite against Azurite, checked 2026-09-14. What it lacks: no smoke profile, and
  the sample carries it only as a commented block in `samples/docker/service/docker-compose.yml`. The
  ServiceModule plan says it goes.
