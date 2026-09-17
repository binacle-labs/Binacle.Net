# admin

**Experimental. Local only.** A page to log in to a local Binacle.Net with the ServiceModule on and manage
its accounts and subscriptions. Nothing builds or deploys it: it has no build recipe and no workflow, and it
is not in the assets copy.

Once, in this folder: `bundle install`. Then, from the repo root:

```bash
just serve api S       # the API with the ServiceModule on, in one terminal
just serve admin       # this, on http://localhost:7198, in another
```

The API's address is `api_url` in `_config.yml`.
