---
id: api/api-client
description: "@binacle/api-client in api/packages/api-client - the hand-written TypeScript client for the v4 API, its committed copy of the OpenAPI document, and the contract test that holds the two together."
verified: 2026-11-09
check: The exported surface matches index.ts and src/; the route list matches the methods on BinacleClient; spec/v4.json is byte-identical to the docs site's copy for the current version; the ajv options and the schema names asserted still match tests/contract.test.ts; the recipes named here match tooling/openapi.just and tooling/tests.just
also_update:
  - packages
  - shared/apps
paths:
  - "api/packages/api-client/**"
---

# api-client

`@binacle/api-client`, in `api/packages/api-client/`. The TypeScript client the apps use to call the API; its
only importer is `@binacle/apps` (`$shared/apps`). Private npm workspace package, `AGPL-3.0-only`, **no runtime
dependencies** and **no build step** - `main` and `types` both point at `index.ts` and each host compiles it
from source, like every workspace package (`$packages`).

**It is written by hand and there is no generator anywhere in it.** Why, and what that trades away, is in
`$packages/decisions`.

`@binacle/service-client`, beside it, has the same shape for the service routes. It is experimental and local
only; `$packages` says where it is used.

## What it exposes

| Export | What it is |
|---|---|
| `BinacleClient` | The client. `new BinacleClient({baseUrl, fetch})`, both optional |
| `packCompareBins(request)` | `POST {baseUrl}/api/v4/pack/compare-bins`. The packing demo's call |
| `packPresetBin(preset, bin, request)` | `POST {baseUrl}/api/v4/pack/bin/{preset}/{bin}`, both names URI-encoded. *Everyday Items*' call |
| `packCompareBinsPath`, `packPresetBinPath(preset, bin)` | The route strings, for anything that needs to name them |
| `ApiResponse<T>`, `ApiSuccess<T>`, `ApiFailure`, `ApiProblem` | The result union |
| `hasValidationErrors(problem)` | Narrows a failure to the 422 shape that lists field errors |
| `readApiResponse(response)` | Turns a `Response` into an `ApiResponse<T>` |
| the v4 wire types | `PackCustomRequest`, `PackPresetRequest`, `PackCompareResponse`, `PackBinResponse`, `Bin`, `Box`, `PackedBox`, `UnpackedBox`, `OperationParameters`, `Algorithm`, `BinPackResultStatus`, `ProblemDetails`, `HttpValidationProblemDetails` |

**`baseUrl` empty means the browser resolves relative to the page it is on**, which is what the UI module
passes so a self-hosted instance calls itself.

**`fetch` is read on every call, not captured in the constructor**, so a test that swaps the global after
construction still works. `options.fetch` overrides it.

## Every call answers `ApiResponse<T>`

```ts
{ok: true,  status, data}              // 2xx
{ok: false, status, problem}           // non-2xx; problem is null when there was no body at all
```

**A 429 from the rate limiter carries no body.** `readApiResponse` reads `response.text()` first and never
calls `response.json()` unconditionally, so an empty body is `problem: null` rather than a parse throw.

**An absent body and an unparseable one are different faults and stay different.** A body that is present and
will not parse still throws. So does a 2xx with an empty body, rather than handing back `data` typed as `T`
with nothing in it.

## The spec copy and the contract test

`spec/v4.json` is a **committed copy** of the v4 OpenAPI document. Do not hand-edit it.

| Recipe | What it does | Runs in CI |
|---|---|---|
| `just openapi check-all-copies` | regenerates the documents, diffs every committed copy | yes, every pull request and release |
| `just openapi sync-all-copies` | regenerates, then writes every committed copy | **no** - a person runs it and commits the result |

`tests/contract.test.ts` loads that copy, registers the whole document with ajv under one name, and pulls a
validator per schema by JSON pointer so the internal `$ref`s resolve. Fixtures are annotated with this
package's own types, so a field renamed in the API changes the spec, fails the fixture, and names the field.

**Two ajv options are load-bearing.** `strict: false`, because an OpenAPI document carries keywords ajv's
strict mode rejects. `validateFormats: false`, because `int32` and `double` are OpenAPI formats rather than
JSON Schema ones and every compile otherwise floods stderr with `unknown format` lines. Neither weakens what
is checked: the enums, the required lists and the types are the assertion.

**What it catches and what it does not.** It catches a renamed field, a newly required one, and a value
outside an enum. It does not catch a field this package declares that the API never had, because the document
sets no `additionalProperties: false`.

## Adding an endpoint

`pack/smallest-bin` and `pack/best-bin` take the same request this package already has and answer a bare
`PackBinResponse`, which is also one entry of `PackCompareResponse`. So each is a method and no new type.

**The document has one schema per route, and the routes of a family share a shape.** The three custom pack
routes (`PackCustomCompareRequest`, `PackCustomSmallestBinRequest`, `PackCustomBestBinRequest`) are one
`PackCustomRequest` here, and the four preset pack routes (`PackPresetBinRequest`, `PackPresetCompareRequest`,
`PackPresetSmallestBinRequest`, `PackPresetBestBinRequest`) are one `PackPresetRequest`. The contract test
validates the same fixture against every name in a family, so the sharing is asserted rather than assumed.

## Tests

```bash
just test ts_binacle-api-client_unit
```

`tests/client.test.ts` mocks `fetch` and covers the URL, method, headers and body sent for both calls, the
escaping of a preset or bin name, and what comes back on 200, on a 4xx with a body, and on a 429 with none.
`tests/contract.test.ts` is the spec check above, and its `the check itself` group breaks a fixture on purpose
to prove the check can fail and names the field when it does.
