# theme-switcher

A theme button - auto, light and dark - as a `<theme-switcher>` custom element, and
the small script that sets the theme before the page first paints. MIT.

It knows nothing about Binacle. The tag name, the storage key and the cookie come from the host, so any site
can use it. The theme is set as `data-theme` on `<html>`; with no attribute, the stylesheet follows the system.

## 📂 What is in it

| Folder or file | What it is |
|---|---|
| `src/themeSwitcher.ts` | `ThemeSwitcherButtonElement`, the button. Each press goes auto, light, dark |
| `src/themeInit.ts` | `initTheme`, the pre-paint script. It reads the stored theme and sets it |
| `src/options.ts` | The options, their defaults, and `optionsFromDocument`, which reads them from `<html>` |
| `src/storage.ts` | Where the choice is kept: a cookie, through [`cookies`](../cookies), `localStorage`, or nowhere |
| `src/theme.ts` | The theme names, and the theme the system asks for |
| `index.ts` | The single entry point |
| `tests/` | The jest tests, on jsdom, in the same folders as the code |

## 🚀 How you use it

The host sets its options as `data-` attributes on `<html>`, so the pre-paint script and the button read the
same ones:

```html
<html data-default-theme="system" data-theme-storage="cookie" data-theme-key="theme">
```

`data-theme-domain` adds a cookie domain, for sites that share a parent domain.

The pre-paint script is its own bundle, loaded in `<head>`:

```ts
import {initTheme} from "theme-switcher";

initTheme();
```

The button goes in the main bundle:

```ts
import {ThemeSwitcherButtonElement, optionsFromDocument} from "theme-switcher";

ThemeSwitcherButtonElement.configure(optionsFromDocument());
customElements.define("theme-switcher", ThemeSwitcherButtonElement);
```

## 🧪 Tests

```bash
just test ts_theme-switcher_unit
```

## ⚠️ What will bite you

**The pre-paint script must stay small.** It blocks the first paint. `initTheme` reaches only the read half of
`src/storage.ts`, so webpack leaves `cookies` out of that bundle. Import the write side into it and `cookies`
comes along.

**A cookie on plain http is not kept if it is marked secure.** The button drops `secure` when the page is on
http, so the theme holds when the API image is served on a local network.
