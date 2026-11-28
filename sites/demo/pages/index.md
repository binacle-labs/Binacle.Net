---
title: Binacle.Net Demo
seo_title: Try the 3D bin packing API in your browser - Binacle.Net
heading: Pick a box to start
description: Try Binacle.Net in your browser. Put in your own box sizes and a list of items and watch them packed in 3D, or decode a ViPaq string. No signup.
permalink: /
excerpt: |
  Try the 3D bin packing API in your browser. Put in your own bins and items and watch them packed, or
  decode a ViPaq string into a 3D view.
boxes:
  - name: Small
    size: 62 x 45 x 8 cm
  - name: Medium
    size: 62 x 45 x 17 cm
  - name: Large
    size: 62 x 45 x 36 cm
---

# {{ page.heading }}
{: .hero }

Binacle.Net answers which box an order goes in. Pick one of three boxes, add some everyday things, and see where
each one goes.

{% include box-cards.html %}

[Pack a box]({{ '/items/' | relative_url }}){: .button .button--primary }
{: .actions }

{% include home-still.html %}

## More ways to try it
{: .band--tint }

- [Custom Sizes]({{ '/packing/' | relative_url }}) - Put in your own bins and items, pick an algorithm, and watch
  Binacle.Net pack them.
- [ViPaq Decoder]({{ '/vipaq/' | relative_url }}) - Decode, analyze, and visualize packing layouts with the ViPaq
  Protocol
{: .doors .doors--row }

## Run it yourself
{: .band--wide #run data-label="For developers" }

One command starts Binacle.Net on your machine. [The Quick Start](https://docs.binacle.net/quick-start/) has the rest.

{::nomarkdown}
<div class="command band__command" data-command>
    <pre class="command__pre"><span class="command__prompt">$ </span><code id="run-text">docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e SCALAR_UI=True -e UI_MODULE=True binacle/binacle-net:3</code></pre>
    {% include components/copy-button.html class="command__copy" copy="run-text" %}
</div>
{:/nomarkdown}
