# ViPaq results

How small a ViPaq token is against the formats it replaces, and what encoding and decoding one costs.

[INDEX.md](INDEX.md) lists every file here and the question it answers.

## 📝 What the results say

Every number here is quoted from the file beside it. Nothing on this page is computed.

| The question | The answer | Read |
|---|---|---|
| How long is a token, stored as is? | 0.65 of protobuf, 0.20 of JSON and 0.57 of compact notation, median per pack. Never longer than protobuf; longer than compact notation only on empty packs, 12 characters against 8. | `format-size.md` |
| Row or columnar? | Columnar, once compressed: 0.86 of row with deflate, median per pack. The gain shrinks as item types grow, from 0.71 at 3 types to 0.96 at 20. | `compressed-size.md` |
| Deflate or gzip? | Deflate. Gzip is never shorter, and gzip makes small packs longer than no compression at all; deflate never does. | `compressed-size.md` |
| How long is a compressed token? | Columnar with deflate is 0.59 of protobuf, 0.48 of JSON and 0.68 of compact notation, each also with deflate, median per pack. | `compressed-size.md` |
| What do encoding and decoding cost? | In the row layout, encoding takes 1.73 to 2.13x protobuf's time up to 365 items, and less than protobuf on most cases from 1000 items. Decoding takes 0.56 to 1.42x and always allocates less. | `encode-decode-cost.md` |
| What does compressing add? | Deflate makes an encode 2.67 to 2.79x slower and a decode 1.49 to 1.76x, row layout. | `encode-decode-cost.md` |

## 🛠️ How you use it

```
just measure vipaq    # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

Every size is a character count, not bytes: ViPaq and protobuf as base64, JSON and compact notation as text,
because text is their own stored form.
