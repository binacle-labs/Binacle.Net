# Quickstart

**A first look at Binacle.Net.** The same API as [minimal](../minimal), with the interactive docs and the
packing demo switched on so you can click around before writing any code against it.

Those three pages are on because looking around is the point. For something you keep running, [prod](../prod)
has nothing browsable at all, and [service](../service) keeps the docs but adds accounts and rate limiting.

## 🚀 Run it

```bash
docker compose up -d
```

| What | Where |
|---|---|
| Packing demo (web UI) | http://localhost:8080/ |
| Swagger UI | http://localhost:8080/swagger/ |
| Scalar UI | http://localhost:8080/scalar/ |

The API itself is under `/api/v3` and `/api/v4` and needs none of them. Swagger and Scalar both send real
requests, so the quickest first call is a `fit` from either page.

## ✏️ Change this first

**`Presets.json` is your bin set.** The shipped presets are examples, and until you replace them the answers
describe someone else's packaging. Edit the file, then restart to pick it up:

```bash
docker compose down
docker compose up -d
```

## 📂 The `./data` folder

Logs are written to `/app/data`, bind-mounted to `./data`. The folder has to be writable by the container's
non-root user before the first run - the [minimal](../minimal) sample has the two commands.

## ✅ Tested on every release

This configuration is smoke-tested against the image on every release, so it is a shape that is checked rather
than one nobody runs.
