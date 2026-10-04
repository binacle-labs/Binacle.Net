# ViPaq results

How small a ViPaq token is against the formats it replaces, and what encoding and decoding one costs.

[INDEX.md](INDEX.md) lists every file here and the question it answers.

## 📝 What the results say

| The question | The answer | Reads |
|---|---|---|
| Is the format worth having? | Yes. A token is 0.65× protobuf and 0.20× JSON stored as is, and 0.60× protobuf and 0.47× JSON as columnar deflate. It is never larger than protobuf on any pack. | `format-size.md`, `compressed-size.md` |
| Is encoding and decoding fast enough? | Yes. Encoding costs about 2× protobuf's time on the real packs and under half its memory. Decoding beats protobuf on most packs and always allocates less. | `encode-cost.md`, `decode-cost.md` |
| Row or columnar? | Columnar, once tokens are compressed. Uncompressed the two are identical character for character; compressed, columnar is 0.86× row and the gap widens with pack size. Decoding is the same either way and row encodes a little faster. | `compressed-size.md`, `encode-cost.md`, `decode-cost.md` |
| Compress, and with what? | Deflate. Gzip is never smaller on any pack. Compressing roughly triples the encode, and decompressing adds about half again to the decode. | `compressed-size.md`, `encode-cost.md`, `decode-cost.md` |

**What the win costs:** columnar deflate puts a pack on the wire at 0.60× protobuf and pays about 2.7× ViPaq's
own encode time for it.

Every number here is quoted from the file beside it. Nothing on this page is computed.

## 🛠️ How you use it

```
just measure vipaq    # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

Every size is a character count, not bytes: ViPaq and protobuf as base64, JSON and compact notation as text,
because text is their own stored form.
