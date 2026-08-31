---
title: Service Module
description: >-
  The Service Module turns Binacle.Net into a service with accounts, authentication and rate limiting.
  No public documentation is provided for it.
permalink: /version/v3.0.x/configuration/service-module/
nav:
  parent: Configuration
  order: 3
  icon: 🛡️
---


The **Service Module** turns Binacle.Net into a service for callers you do not control: accounts, JWT
authentication, per-caller rate limiting, and a database behind them. It is built for the hosted service.

⚠️ **No public documentation is provided for the Service Module.**

## 📌 What that means

- There is no reference documentation for its configuration, its endpoints or its data.
- No support is offered for it.
- **Changes to it are listed in the release notes like anything else, but no migration steps are given.**
- **A breaking change to it does not force a major version.** A **minor** release can break it. A **patch**
  will not.

If you self-host with the Service Module enabled, read every minor release before upgrading.

## 🔓 It is still open source

The module ships in the source and in the image under the same licence as the rest of Binacle.Net. You can run
it, read it and change it.

## 🐳 What does exist

The [Service sample]({% vlink /samples/docker/service/index.md %}) is a working compose file that starts it:
the secrets you have to change, the database choice, and how to get a token. It is a starting point, not
documentation.

Beyond that, the source is the reference.
