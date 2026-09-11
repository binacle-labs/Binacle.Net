---
title: ViPaq Protocol
description: >-
  The ViPaq wire format as Binacle.Net writes and reads it. Strings from this version do not decode in
  v3.0.0 or later.
nav:
  order: 7
  icon: 🗜️
---

**ViPaq** packs one bin and its items into a single copy-pastable string. Binacle.Net's packing responses get
large with many items; ViPaq condenses one result - a single bin plus the items packed into it - into a string
that is small to store, cheap to send and easy to share. It carries the bin's dimensions and each item's
dimensions and position, which is everything needed to redraw the packing. This page is the wire format as
Binacle.Net {{ page.version_label }} writes and reads it.

> ⚠️ ViPaq is experimental and may change between versions.
>
> Strings produced by this version are not readable by Binacle.Net v3.0.0 or later, which uses a different format.
{: .block-warning}

## ⚙️ How It Works

ViPaq serializes the bin's dimensions along with each item's size and coordinates into a single encoded string, 
preserving all necessary data for visualization and decoding.

## 📌 Data Structure
```text
[Header] 
[Number of Items] 
[Bin: Length, Width, Height] 
[Item 1: Length, Width, Height, X, Y, Z]
[Item 2: Length, Width, Height, X, Y, Z] 
... 
[Item N: Length, Width, Height, X, Y, Z]
```

### 🛠️ Components

- **Header**: Decoding metadata
- **Number of Items**: Total encoded items
- **Bin**: Dimensions (Length, Width, Height)
- **Items**: Each with dimensions (L, W, H) and position coordinates (X, Y, Z)

### 🔑 Encoding & Compression Techniques

- **Base64 Encoding**: Converts binary data into a transfer-friendly string
- **Variable Length Encoding (VLE)**: Reduces storage by minimizing redundant data
- **Gzip Compression**: Automatically applied for larger data, enhancing compactness
