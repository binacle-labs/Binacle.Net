---
description: TODOs
state: ready
waits-on: "nothing"
---

# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## CI

**The OpenAPI lint moved out on 2026-08-17** and into the v3.0.0 release plan, which owns it whole along with
the `--fail-severity=warn` flag it needs. It had been written in both files and the two copies had already
started to differ.

## Agent docs

- **`.agents/docs/sites/demo.md` has not been read against the tree since 2026-08-24.** Run its own `check:`
  line: collections, JS bundles and plugin list against `sites/demo/_config.yml` and `sites/demo/js/`; no
  `seo.html` in `_includes/` and `pages/index.html` still printing `item.summary`; the demo/prefetch script
  split against `_data/includes.yml`; the `sitemaps:` block writing one file and `/sitemap.xml` listing three
  pages; `artifacts/demo/lib/` after `just build demo` holding exactly the vendor folders listed, with
  `gulpfile.js`'s IGNORE map explaining what is missing. Fix what has moved, then date it.

## Sites

- **Old-register prose left on the docs site.** Found 2026-08-25, checked again 2026-08-27.
  `sites/docs/collections/_versions/v3.0.x/configuration/ui-module/index.md:20-28` still reads "allows users
  to interact", "Users can navigate" and "enables users to decode"; `sites/docs/pages/index.md:29` still says
  "the algorithms and real-time strategies that power Binacle.Net's packing solutions". **A site session.**

## The shared UI package

- **The submit button can stick.** `packages/binacle-net-ui/src/core/packingDemo.ts:183` sets
  `submitting = true` in `onSubmit` and only the `finally` at `:225` clears it - and that `finally` is inside
  the thunk handed to `$dispatch('update-scene', ...)`, which nothing runs unless a visualizer is listening.
  On a page without one the button stays disabled and the status stays "Packing..." with no way back.
  **Latent, not live** - both packing pages include the visualizer today. Found independently by two reviews.
  **The fix is a behaviour decision, not a typo**: what the button should wait on. Deliberately not changed
  before the v3.0.0 tag.

- **An import that reads wrong.** `packages/binacle-net-ui/src/core/packingDemo.ts:170` tests
  `error instanceof Error` while `:11` imports `Error` from `../viewModels`. `viewModels/error.ts` declares an
  interface, so the import is erased and the branch tests the global `Error` - correct today. **If that
  interface ever becomes a class it breaks silently.**

## Kernel

- **The nullable-enum converter never rejects a bad value.** Found 2026-08-27.
  `api/src/Binacle.Net.Kernel/Serialization/JsonStringNullableEnumConverter.cs:104` -
  `TryParseEnumFromString` has five returns and every one of them is `return true`, including the
  fall-through at `:129` where nothing matched. An unknown enum string is read as `default!` rather than
  refused, so validation is what has to catch it. Two consequences:
  - The `throw new JsonException(...)` at `:78` in `ReadAsPropertyName` is unreachable. Nothing can make
    `TryParseEnumFromString` return `false`.
  - **Not a live defect where the value is required.** `default!` on a nullable enum is null, and the
    validators reject null - `v3/Contracts/Algorithm.cs:26` and the v4 one both `NotNull()`, so a bogus
    algorithm is a 422 either way.
  - **It does change behaviour on a patch.** `ServiceModule/v0/Contracts/Admin/SubscriptionPatchRequest.cs`
    only requires that one of `Type` and `Status` has a value, so a request naming a bogus `Type` and a good
    `Status` passes and the bad field is silently dropped instead of refused.

  **The lenient parse is worth a deliberate decision rather than an accident**, and the dead throw says the
  opposite of what the code does.

## ServiceModule

**Both are `// TODO` comments in the code, and both ride with the ServiceModule decision.** The maintainer
said on 2026-08-27 that ServiceModule work is taken as one piece, not row by row - so neither of these is
picked up on its own.

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:32` - the comment reads
  **"Review json config for default policies"**. It sits at the top of `GetPartition`, which reads
  `ApiUsageAnonymous` and the tier configurations out of `RateLimiterConfiguration`.

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57` - the comment reads
  **"Make Response"**. It is on the `this.request is null` path, which returns a bare `ProblemDetails`
  ("Malformed Request", 400) where every other path in the file returns a typed result.
