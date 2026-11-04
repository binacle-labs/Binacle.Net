# binacle-components

The page behaviour the sites share. Each part is one init function that finds its elements by `data-` attribute
and does nothing on a page that has none, so a site's entry calls only the parts it uses. The look comes from
[`design/`](../../design); the theme toggle is [`theme-switcher`](../theme-switcher).

## 📂 What is in it

| Part | Init | What it does |
|---|---|---|
| `src/drawer.ts` | `initDrawer` | The menu drawer. Builds one from the header's links, and opens and closes every `[data-drawer]` |
| `src/navFade.ts` | `initNavFade` | Marks the sides of the phone nav that have more, so the CSS can fade them |
| `src/stickySidebar.ts` | `initStickySidebar` | Shortens a sticky menu as the footer comes into the window |
| `src/copyButton.ts` | `initCopyButton` | Shows the copy buttons and makes them work |
| `src/codeWrap.ts` | `initCodeWrap` | Sets where commands, inline code and code lines may break |
| `src/tabs.ts` | `initTabs` | One panel per tab, in order |
| `src/lightbox.ts` | `initLightbox` | Opens a still in the dialog it names |

## 🚀 How you use it

```ts
import {initCodeWrap, initCopyButton, initDrawer} from "binacle-components";

document.addEventListener("DOMContentLoaded", () => {
	initDrawer();
	initCopyButton();
	initCodeWrap();
});
```

From the repo root:

```
just test ts_binacle-components_unit
```

## ⚠️ What will bite you

**Call each init once, after the page is parsed.** None of them waits for the page or guards against a second
call, and `initDrawer` listens on `document` and `window`.

**A site that writes its own drawer must write it before `initDrawer` runs.** The drawer and its buttons are
wired when it is called, not later.
