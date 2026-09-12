---
title: CORS
description: >-
  Let a browser page call the Binacle.Net API directly: the Cors.json file, the origins it names, and why no
  origin is allowed until you add it.
nav:
  parent: Core
  order: 3
  icon: 🌍
---

CORS only matters when a **browser** calls the API directly - a page on your site sending the request itself.
A server calling Binacle.Net is never affected.

Until you allow an origin, the browser blocks every cross-origin call. The API still answers; the page never
sees the answer.

## 🛠️ Configuration
Allowed origins are configured in `Cors.json`, which is **not in the image** - you supply it. The list is
read at startup, so a change needs a restart.

```json
{
  "Cors": {
    "CoreApi": {
      "AllowedOrigins": [ "https://your-site.example" ]
    }
  }
}
```

- 📁 **Location**: `/app/Config_Files`
- 📌 **Full Path**: `/app/Config_Files/Cors.json`

The same value can come from an environment variable - `Cors__CoreApi__AllowedOrigins__0=https://your-site.example` -
as the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page describes.

## 🔧 Configuration Options

- `CoreApi.AllowedOrigins` (_array_): the origins allowed to call the API from a browser.

An origin is a scheme, a host and an optional port, with nothing after it: `https://example.com`,
`http://localhost:5173`. The browser compares it as an exact string, so a trailing slash or a path never
matches, and Binacle.Net refuses to start on one. `*` allows any origin.

Every allowed origin may use any method and any header.

## 🔍 Is it CORS?

A blocked call shows in the browser console, not in the API's logs - the request reached the API and was
answered. If the console names a CORS error, the origin the page is served from is not in the list, or is
spelled differently from it.
