---
id: packages/binacle-net-client
description: packages/binacle-net-client — the hand-written TypeScript client for the v4 API, its committed copy of the OpenAPI document, and the contract test that holds the two together.
verified: 2026-09-10
check: The exported surface matches src/index.ts; the endpoint list matches the methods on BinacleClient; spec/v4.json is byte-identical to the docs site's copy for the current version; the ajv options and the schema names asserted still match tests/contract.test.ts; the recipes named here match tooling/openapi.just
also_update:
  - packages
  - packages/binacle-net-ui
paths:
  - "packages/**"
---

# binacle-net-client

The TypeScript client the demo uses to call the API. Private npm workspace package (`"private": true`),
`AGPL-3.0-only`, **no runtime dependencies** and **no build step** — `main` and `types` both point at
`src/index.ts` and each host compiles it from source, the same way it compiles `binacle-net-ui`.

**It is written by hand and there is no generator anywhere in it.** Why, and what that trades away, is in
`$packages/decisions`.

## What it exposes

| Export | What it is |
|---|---|
| `BinacleClient` | The client. `new BinacleClient({baseUrl, fetch})`, both optional |
| `packCompareBins(request)` | `POST {baseUrl}/api/v4/pack/compare-bins` |
| `packCompareBinsPath` | The route string, for anything that needs to name it |
| `ApiResponse<T>`, `ApiSuccess<T>`, `ApiFailure`, `ApiProblem` | The result union |
| `hasValidationErrors(problem)` | Narrows a failure to the 422 shape that lists field errors |
| `readApiResponse(response)` | Turns a `Response` into an `ApiResponse<T>` |
| the v4 wire types | `PackCustomRequest`, `PackCompareResponse`, `PackBinResponse`, `Bin`, `Box`, `PackedBox`, `UnpackedBox`, `OperationParameters`, `Algorithm`, `BinPackResultStatus`, `ProblemDetails`, `HttpValidationProblemDetails` |

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
| `just openapi sync-all-copies` | regenerates, then writes every committed copy | **no** — a person runs it and commits the result |

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

The three custom pack routes are separate but field-identical schemas in the document
(`PackCustomCompareRequest`, `PackCustomSmallestBinRequest`, `PackCustomBestBinRequest`); this package has one
`PackCustomRequest` and the contract test validates the same fixture against all three names, so the sharing
is asserted rather than assumed.

## Tests

```bash
just test ts_binacle-net-client_unit
```

`tests/client.test.ts` mocks `fetch` and covers the URL, method, headers and body sent, and what comes back on
200, on a 4xx with a body, and on a 429 with none. `tests/contract.test.ts` is the spec check above, and
includes two tests that break a fixture on purpose to prove the check can fail and names the field when it
does.
