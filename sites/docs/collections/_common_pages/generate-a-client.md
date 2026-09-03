---
title: Generate a Client
description: >-
  Generate a typed API client from Binacle.Net's published OpenAPI documents. hey-api for TypeScript, Kiota for
  C#, two commands each.
nav:
  order: 4
  icon: 🧰
---

Every documented version publishes its OpenAPI document. Point a generator at one and you get a typed client
in two commands.

There are no SDK packages to install. The published document is the deliverable, and the client is yours to
generate, read and keep in your own repository.

## 📡 Where the Document Is

Each version serves one document per API version, at:

```text
https://docs.binacle.net/version/<version>/swagger/<api>.json
```

For {{ site.data.versions.current }} those are:

- V3: [`https://docs.binacle.net/version/{{ site.data.versions.current }}/swagger/v3.json`]({{ '/version/' | append: site.data.versions.current | append: '/swagger/v3.json' | relative_url }})
- V4: [`https://docs.binacle.net/version/{{ site.data.versions.current }}/swagger/v4.json`]({{ '/version/' | append: site.data.versions.current | append: '/swagger/v4.json' | relative_url }})

Swap the version segment for the one you run. The [Versions]({% link _common_pages/version.html %}) page lists
them, and the [API reference for {{ site.data.versions.current }}]({{ '/version/' | append: site.data.versions.current | append: '/api/' | relative_url }})
describes the endpoints.

> ⚠️ V4 is **experimental and can change at any time**. A client generated from `v4.json` will need
> regenerating, and your calling code will need editing, when the contracts move. Use V3 for anything you keep.
{: .block-warning}

## 🌐 Set the Base URL Yourself

The documents carry one `servers` entry and it is the relative URL `/`, because Binacle.Net is self-hosted
and the document cannot know your host. A generated client therefore does not know where your instance is.
Set the base URL in your own code. Both examples below do.

Kiota is the one to watch: with nothing to go on it falls back to the host it downloaded the document from,
which is the docs site and not your API. Assigning `BaseUrl` before you construct the client replaces it.

## 🟦 TypeScript with hey-api

```bash
mkdir binacle-client && cd binacle-client
npm init -y
npm pkg set type=module
npm install --save-dev @hey-api/openapi-ts typescript@5
npx @hey-api/openapi-ts -i https://docs.binacle.net/version/{{ site.data.versions.current }}/swagger/v4.json -o src/client
```

That writes `src/client` with the request functions, the types and a fetch client.

> 💡 The `typescript@5` pin is deliberate. hey-api accepts a wider range, but npm resolves it to TypeScript 7,
> which the generator does not run on today.
{: .block-tip}

```typescript
import { client } from './src/client/client.gen';
import { packCustomBin } from './src/client';

client.setConfig({ baseUrl: 'http://localhost:8080' });

const { data, error } = await packCustomBin({
  body: {
    bin: { id: 'medium-box', length: 40, width: 30, height: 20 },
    items: [{ id: 'item-1', length: 10, width: 10, height: 10, quantity: 3 }],
    parameters: { algorithm: 'Best' },
  },
});

if (error) throw error;
console.log(data?.status, data?.packedItems?.length);
```

## 🟪 C# with Kiota

```bash
dotnet tool install --global Microsoft.OpenApi.Kiota
dotnet new console -o binacle-client
cd binacle-client
dotnet add package Microsoft.Kiota.Bundle
kiota generate -l CSharp -d https://docs.binacle.net/version/{{ site.data.versions.current }}/swagger/v4.json -c BinacleClient -n Binacle.Client -o ./Client
```

That writes `Client` with one request builder per path segment and the models under `Client/Models`.

```csharp
using Binacle.Client;
using Binacle.Client.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Bundle;

var adapter = new DefaultRequestAdapter(new AnonymousAuthenticationProvider())
{
    BaseUrl = "http://localhost:8080"
};
var client = new BinacleClient(adapter);

var response = await client.Api.V4.Pack.Bin.PostAsync(new PackCustomBinRequest
{
    Bin = new Bin { Id = "medium-box", Length = 40, Width = 30, Height = 20 },
    Items = [new Box { Id = "item-1", Length = 10, Width = 10, Height = 10, Quantity = 3 }],
    Parameters = new OperationParameters { Algorithm = Algorithm.Best }
});

Console.WriteLine($"{response?.Status} {response?.PackedItems?.Count}");
```

`AnonymousAuthenticationProvider` is right for a Binacle.Net instance with no authentication in front of it.
Put your own provider there if yours sits behind one.

## 🏷️ How the Method Names Come Out

The two generators name things differently, and neither is wrong.

- **hey-api names from `operationId`.** V4 uses dot notation, so `pack.customBin` becomes `packCustomBin()`.
  V3's flatter ids come through as `packByCustom()`, `fitByPreset()` and so on.
- **Kiota names from the URL path** and ignores `operationId` entirely, so `POST /api/v4/pack/bin` is
  `client.Api.V4.Pack.Bin.PostAsync()`.

Both group the operations the same way, because the grouping comes from the tags in the document: `Pack`,
`Fit` and `Presets`.
