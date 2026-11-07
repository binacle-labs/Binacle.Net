# binacle-components

Forwards to [`design-system`](../design-system), where the page behaviour the sites share now lives. `docs` and
`demo` still import from here; `www` imports from the design system.

## 🚀 How you use it

```ts
import {initCodeWrap, initCopyButton, initDrawer} from "binacle-components";
```

It exports the same init functions as the design system, but for `initThemeButton`. Their tests are there.
