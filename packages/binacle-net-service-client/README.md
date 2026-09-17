# binacle-net-service-client

**Experimental. Local only.** A hand-written TypeScript client for the Binacle.Net service API - the token
route and the admin routes for accounts and subscriptions. Same shape as `binacle-net-client`, and
no dependency on it.

```ts
import {ServiceClient} from 'binacle-net-service-client';

const client = new ServiceClient({baseUrl: 'https://localhost:7194', token: () => sessionStorage.getItem('token')});
const login = await client.requestToken({username: 'admin', password: '...'});
const accounts = await client.listAccounts({page: 1});
```

| Folder or file | What it is |
|---|---|
| `src/client.ts` | `ServiceClient`. One method per route |
| `src/types.ts` | The wire contracts, one type per schema in the spec, named after it |
| `spec/service.json` | A copy of the API's service OpenAPI document. **Never edit it by hand** - `just openapi sync-all-copies` writes it |
| `tests/` | The contract test against the spec, and the client against a mocked `fetch` |

```bash
just test ts_binacle-net-service-client_unit
```
