---
permalink: /self-hosting/
title: Self-hosting
heading: Self-hosting Binacle.Net.
description: >-
  Binacle.Net is one image you run on your own machine. It turns on only what you ask for: the API is always
  there, and everything else is a module with its own files and switches.
---

# {{ page.heading }}
{: .band--split .split--text-left }

Binacle.Net is one image you run on your own machine. It turns on only what you ask for: the API is always there, and
everything else is a module with its own files and switches.

Your customers' dimensions never leave your network, and there is no per-call fee.
{: .muted }

{% include picture.html name="host" %}

{% include command.html id="run" %}

The quick way in. For a real deployment, start from one of the setups below.
{: .verdict }

## Six setups
{: .band--mirrored .band--tint .split--text-right data-label="Setups" }

Five for Docker and one for Kubernetes, from nothing switched on to everything. Each one is a folder of files to
download and edit.

**Most deployments want prod.** The docs explain the choice between prod and service.

- [Minimal](https://docs.binacle.net/samples/docker/minimal/) - The smallest configuration that still answers
  requests.
- [Quickstart](https://docs.binacle.net/samples/docker/quickstart/) - Trying Binacle.Net out, with the docs and the
  web UI.
- [Prod](https://docs.binacle.net/samples/docker/prod/) - The API behind your own backend.
- [Service](https://docs.binacle.net/samples/docker/service/) - Binacle.Net offered to callers you do not control.
- [Full](https://docs.binacle.net/samples/docker/full/) - Everything on at once, for a machine nobody else can reach.
- [Kubernetes](https://docs.binacle.net/samples/kubernetes/minimal/) - A minimal deployment on an existing cluster.
{: .doors }

## Presets.json is your bin set
{: .band--split .band--tint .split--text-left data-label="Your bin set" }

Every sample ships an example one. Replacing it with your own boxes, lockers or pallets is the first thing to do;
until then the answers describe someone else's packaging.

A request then names the preset instead of carrying the boxes.

```json
{
  "PresetOptions": {
    "Presets": {
      "custom-preset-1": {
        "Bins": [
          { "ID": "bin-1", "Length": 60, "Width": 40, "Height": 10 },
          { "ID": "bin-2", "Length": 60, "Width": 40, "Height": 20 }
        ]
      }
    }
  }
}
```

## Change a setting
{: .band--wide .band__body--code-left data-label="Configuration" }

Every configuration file lives under `/app/Config_Files`. There are four ways to change a setting, and the first one
wins:

- **Environment variables.** For secrets and anything that differs per deployment.
- **Production overrides.** A `<filename>.Production.json` holding only what you change.
- **Direct file edits.** Replace the file with a bind mount or a volume.
- **Connection string fallbacks.** One environment variable per connection string.
{: .band__list }

```
Settings__Enabled=True
Settings__Logs__Retention=5
```

An environment variable is named after the setting's path, with `__` between the levels.
{: .verdict }

## Check what you run
{: data-label="Releases" }

Every image from 3.0.0 onward is signed, and carries a software bill of materials and build provenance.
[Verify a release]({{ '/verify/' | relative_url }}) has the two commands.
