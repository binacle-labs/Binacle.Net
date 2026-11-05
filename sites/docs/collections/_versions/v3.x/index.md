---
title       : Welcome to Binacle.Net Docs!
seo_title   : Open source 3D bin packing API - Binacle.Net Docs
breadcrumbs : false
home        : true
description: >-
  Documentation for Binacle.Net: quick start, the HTTP API, configuration, Docker and Kubernetes samples,
  and the release notes.
menu_title  : v3.x
nav:
  exclude: true
  order: 1
  icon: 🏠
---

<div class="split hero split--centred" markdown="0">
  <div class="hero__copy">
    <h1 class="split__title">{{ page.title }}</h1>
    <p class="doc__meta label"><span class="doc__value">{{ page.version_label }}</span><span>Docker tag {{ page.version_tag }}</span></p>
    <div class="prose prose--marketing">
      <p>Binacle.Net answers which box an order goes in, in milliseconds. Give it your box sizes and a list of items
      and it returns the smallest box that holds them, and where every item sits. It is a free and open source 3D
      bin packing API that you run yourself, shipped as a Docker image.</p>
    </div>
    <p>These docs cover the HTTP API, configuration, deployment, and the ViPaq result format. If you just want to
    see it work, start with the quick start.</p>
    <p class="actions"><a class="button" href="{% vlink /quick-start.md %}">Quick Start</a></p>
  </div>
  <figure class="pack-view">
    <button type="button" class="still" data-lightbox="home-still" aria-label="Open the picture larger">
      {% include drawings/home-still.svg %}
    </button>
    <figcaption class="pack-view__strip">
      <span class="dimension-line dimension-line--small dimension-line--strip"><span class="dimension-line__tick"></span><span class="dimension-line__number">45 x 30 x 25</span><span class="dimension-line__tick"></span></span>
      <span>13 items, the default sample</span>
    </figcaption>
  </figure>
  <dialog class="lightbox" id="home-still" aria-label="13 items, the default sample, larger">
    {% include drawings/home-still.svg class="lightbox__still" %}
    <div class="lightbox__bar"><span>The default sample: 13 items packed in a 45&nbsp;x&nbsp;30&nbsp;x&nbsp;25 bin</span><button type="button" class="button button--link" data-close>Close</button></div>
  </dialog>
</div>

<div class="doc__band" markdown="0">
  <h2>Contents</h2>
  <nav class="doors doors--grid" aria-label="Sections">
    <a class="doors__link" href="{% vlink /quick-start.md %}"><h3 class="doors__title">🚀 Quick Start</h3><p class="doors__text">One <code>docker run</code>, then a request. Start here if you have not run it yet.</p></a>
    <a class="doors__link" href="{% vlink release-notes.md %}"><h3 class="doors__title">🛠️ Release Notes</h3><p class="doors__text">What changed in {{ page.version_label }}, and how to move from the version before.</p></a>
    <a class="doors__link" href="{% vlink /core-concepts.md %}"><h3 class="doors__title">🔍 Core Concepts</h3><p class="doors__text">What a dimension must be, fit against pack, and what each algorithm does.</p></a>
    <a class="doors__link" href="{% vlink /api/index.md %}"><h3 class="doors__title">📡 API</h3><p class="doors__text">The endpoints and their shapes. V3 is stable; V4 is experimental and adds the <code>Best</code> algorithm.</p></a>
    <a class="doors__link" href="{% vlink /generate-a-client.md %}"><h3 class="doors__title">🧰 Generate a Client</h3><p class="doors__text">A typed client from the published OpenAPI documents - hey-api for TypeScript, Kiota for C#.</p></a>
    <a class="doors__link" href="{% vlink /configuration/index.md %}"><h3 class="doors__title">🔧 Configuration</h3><p class="doors__text">The files under <code>/app/Config_Files</code>, how to override a setting, and one page per module.</p></a>
    <a class="doors__link" href="{% vlink /samples/index.md %}"><h3 class="doors__title">📦 Samples</h3><p class="doors__text">Docker Compose and Kubernetes setups to copy, including one for running behind your own backend.</p></a>
    <a class="doors__link" href="{% vlink vipaq-protocol.md %}"><h3 class="doors__title">🗜️ ViPaq Protocol</h3><p class="doors__text">The compact format the packing endpoints return. Stable from v3.0.0.</p></a>
    <a class="doors__link" href="{% vlink /verifying-a-release.md %}"><h3 class="doors__title">🔏 Verifying a Release</h3><p class="doors__text">Check the signature and read the bill of materials of a published image.</p></a>
  </nav>
</div>
