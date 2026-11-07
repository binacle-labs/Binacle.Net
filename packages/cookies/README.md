# cookies

Read, write and remove cookies in the browser. A copy of [js-cookie](https://github.com/js-cookie/js-cookie)
v3.0.5, changed a little, under the same MIT licence. `LICENSE` is upstream's.

## 📂 What is in it

| Folder or file | What it is |
|---|---|
| `src/api.ts` | `Cookies` - `get`, `set`, `remove` - and `CookieAttributes` |
| `src/assign.ts` | Merges the default attributes with the ones a call passes |
| `src/converter.ts` | Encodes and decodes a cookie's value |
| `index.ts` | The single entry point |
| `tests/` | The jest tests, on jsdom |

## 🚀 How you use it

```ts
import {Cookies} from "cookies";

Cookies.set("theme", "dark", {expires: 365, sameSite: "lax"});
Cookies.get("theme");
```

## 🧪 Tests

```bash
just test ts_cookies_unit
```

## ⚠️ What will bite you

**A cookie is `secure` by default.** On plain http the browser drops it. `theme-switcher` turns it off on http
itself; any other caller has to do the same.

**It is not the upstream package.** It has no `withConverter` and no `withAttributes`. Upgrading means applying
upstream's changes here by hand.
