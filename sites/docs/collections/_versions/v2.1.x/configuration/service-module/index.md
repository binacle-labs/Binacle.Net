---
title: Service Module
description: >-
  The Service Module in Binacle.Net v2.1.x has no public documentation. What it is, and what it means if
  you self-host.
permalink: /version/v2.1.x/configuration/service-module/
nav:
  parent: Configuration
  order: 3
  icon: 🛡️
---


The **Service Module** of Binacle.Net has undergone a complete redesign and restructuring,  
introducing fundamental changes that break compatibility with all existing integrations.

⚠️ **Public documentation for the Service Module is no longer available.**

The module is built for the hosted service. It is not a general purpose feature of Binacle.Net, and it is not
documented, supported or version managed for public use.

## 🔄 Accounts and Subscriptions

Access is managed through accounts and subscriptions rather than anonymous and registered users.

### 👤 Anonymous Access
Anonymous users can still make requests to Binacle.Net and are subject  
to a fixed **global throttling limit** that applies collectively to all anonymous traffic.

### 🗝️ Accounts
The Admin issues accounts and subscriptions. To access the service beyond anonymous limits,
each account must have an active subscription associated with it.

## 📚 Documentation and Support

- 🚫 From v2.0.0 onward, **no public documentation is provided for the Service Module**.
- 🔕 Breaking changes are **not** documented publicly and do not trigger a major version increment.

## 🔓 Open Source Availability

The Service Module remains an open-source component within the Binacle.Net project. This allows anyone to:

- 🛠️ Clone and run their own instance of Binacle.Net.
- ⚙️ Utilize the Service Module fully within their self-managed environment.

However, since no official documentation or support is offered for the Service Module anymore, 
**self-hosted users must rely exclusively on the source code and community resources** for implementation and troubleshooting.
