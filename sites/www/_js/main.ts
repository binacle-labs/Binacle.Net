// The whole of this site's JavaScript beyond the theme. The theme is packages/theme-switcher, defined
// below; its pre-paint read is a separate bundle loaded in the head.
import {initCopyButton} from "@binacle/design-system";
import {initCodeWrap, initDrawer} from "binacle-components";
import {ThemeSwitcherButtonElement, optionsFromDocument} from "@binacle/theme-switcher";

ThemeSwitcherButtonElement.configure(optionsFromDocument());
customElements.define("theme-switcher", ThemeSwitcherButtonElement);

document.addEventListener("DOMContentLoaded", () => {
	initDrawer();
	initCodeWrap();
	initCopyButton();
});
