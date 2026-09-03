# Minimal

**The smallest thing that answers.** The API on port 8080 with your own bin set, and nothing else switched on:
no interactive docs, no web UI, no health checks, no accounts.

Take it when you want the API with nothing in front of it. [quickstart](../quickstart) is the same thing with
the docs and the packing demo on; [prod](../prod) is the one to run for real, with health checks and packing
logs.

## 🚀 Run it

```bash
docker compose up -d
```

There is nothing to browse to. Ask it which of the two example bins holds a 30x20x15 item:

```bash
curl -X POST http://localhost:8080/api/v3/fit/by-preset/custom-preset-1 \
  -H 'Content-Type: application/json' \
  -d '{"parameters":{"algorithm":"FFD"},"items":[{"id":"item-1","quantity":1,"length":30,"width":20,"height":15}]}'
```

One result per bin in the preset. `bin-1` is 10 deep, so the item does not go in; `bin-2` is 20 deep and takes
it:

```json
{"result":"Success","data":[
  {"result":"NotAllItemsFit","bin":{"id":"bin-1","length":60,"width":40,"height":10},
   "fittedItems":[],"unfittedItems":[{"id":"item-1","quantity":1}],
   "fittedBinVolumePercentage":0,"fittedItemsVolumePercentage":0},
  {"result":"AllItemsFit","bin":{"id":"bin-2","length":60,"width":40,"height":20},
   "fittedItems":[{"id":"item-1","length":30,"width":20,"height":15}],"unfittedItems":[],
   "fittedBinVolumePercentage":18.75,"fittedItemsVolumePercentage":100}]}
```

## ✏️ Change this first

**`Presets.json` is your bin set.** `custom-preset-1` and `custom-preset-2` are examples, and until you
replace them the answers describe someone else's packaging. Edit the file, then restart to pick it up:

```bash
docker compose down
docker compose up -d
```

## 📂 The `./data` folder

Logs are written to `/app/data`, which is bind-mounted to `./data`. The image runs as a non-root user, uid
1654, and docker does not change the owner of a bind mount, so create the folder and hand it over before the
first run:

```bash
mkdir -p ./data
sudo chown -R 1654:1654 ./data
```

`sudo chmod -R 777 ./data` also works and is what older copies of this sample said. It gives every user on the
host full access to the folder; the chown above is the narrow version of the same fix.

## ✅ Tested on every release

This configuration is smoke-tested against the image on every release, so it is a shape that is checked rather
than one nobody runs.
