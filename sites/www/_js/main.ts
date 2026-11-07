// The whole of this site's JavaScript. The theme's pre-paint read is a separate bundle loaded in the head.
import {initCodeWrap, initCopyButton, initDrawer, initThemeButton} from "@binacle/design-system";

initThemeButton();

document.addEventListener("DOMContentLoaded", () => {
	initDrawer();
	initCodeWrap();
	initCopyButton();
});
