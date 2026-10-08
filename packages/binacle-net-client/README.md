# binacle-net-client

A TypeScript client for the Binacle.Net **v4** API. It sends the request, hands back the parsed body, and
stops there - nothing in here knows what a screen looks like.

It is written by hand. There is no generator and no codegen step, and there is not meant to be one. What a
generator would have given us - types that cannot quietly drift away from the API - comes instead from the
contract test in `tests/`, which validates fixtures written against these types against a committed copy of
the API's own OpenAPI spec.

```ts
import {BinacleClient} from 'binacle-net-client';

const client = new BinacleClient({baseUrl: 'https://api.binacle.net'});

const response = await client.packCompareBins({
	parameters: {algorithm: 'Best', includeViPaqData: true},
	bins: [{id: 'bin_1', length: 10, width: 40, height: 60}],
	items: [{id: 'box_1', quantity: 2, length: 2, width: 5, height: 10}],
});

if (response.ok)
	console.log(response.data.results);
else
	console.log(response.status, response.problem);
```

Leave `baseUrl` out and requests go to whatever host served the page.

## 📂 What is in it

| Folder or file | What it is |
|---|---|
| `src/client.ts` | `BinacleClient`. One method per route, today just `packCompareBins` for `POST /api/v4/pack/compare-bins` |
| `src/types.ts` | The wire contracts, one type per schema in the spec, named after it |
| `src/response.ts` | `ApiResponse<T>` - the success-or-problem answer every call returns - and how a response body is read |
| `src/index.ts` | The single entry point. Everything above is re-exported from here |
| `spec/v4.json` | A copy of the API's OpenAPI spec. **Never edit it by hand**, see below |
| `tests/contract.test.ts` | Fixtures typed by `src/types.ts`, validated against `spec/v4.json` with ajv |
| `tests/client.test.ts` | The client itself, against a mocked `fetch` |

Every call answers an `ApiResponse<T>`: `{ok: true, status, data}` when the API answered 2xx, and
`{ok: false, status, problem}` when it did not. `problem` is the RFC 7807 body the API sends on a 4xx or 5xx,
or `null` when there was no body at all.

## 🚀 How you use it

The package is an npm workspace. Install once from the repo root:

```bash
npm install
```

Then import it by name - `binacle-net-client` - from any workspace that depends on it. There is nothing to
build; the host compiles the TypeScript from source.

## 🧪 Tests

```bash
just test ts_binacle-net-client_unit
```

The contract test is the reason this package exists. It loads `spec/v4.json`, pulls each schema out by JSON
pointer and checks a fixture against it. The fixtures are annotated with the types in `src/types.ts`, so a
field renamed in the API changes the spec, does not change our type, and fails the test by name.

## ⚠️ What will bite you

**`spec/v4.json` is a copy and nothing here writes it.** A just recipe keeps it in step with the API:
`just openapi check-all-copies` tells you whether every copy still matches, `just openapi sync-all-copies`
rewrites them. Hand-editing the file makes the contract test pass against a spec the API never published,
which is the one failure this package is built to prevent.

**There is no build step.** Every host bundles the TypeScript from source with its own webpack, so a change
here is only proven once those hosts build. Green tests in this folder say the types agree with the spec;
they do not say the hosts still compile.

**The client does not retry, throttle or cache.** A 429 comes back as a plain failure with no body, because
the rate limiter sends none. What to do about it is the caller's decision.
