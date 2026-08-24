// The whole of this site's JavaScript beyond the theme. The theme is packages/theme-switcher, defined
// below; its pre-paint read is a separate bundle loaded in the head.
import {ThemeSwitcherButtonElement, optionsFromDocument} from "theme-switcher";

// The docker run line is the primary conversion on three of the four pages, so one click instead of a
// drag-select is the highest-value JavaScript on the site. A button marks what it copies with
// data-copy="<id>"; with JavaScript off the text is still selectable, which is the fallback.
function wireCopyButtons(): void {
	document.querySelectorAll<HTMLButtonElement>("[data-copy]").forEach((button) => {
		const target = document.getElementById(button.dataset.copy || "");
		if (!target || !navigator.clipboard) return;
		button.hidden = false;
		const original = button.textContent;
		button.addEventListener("click", async () => {
			try {
				await navigator.clipboard.writeText(target.textContent?.trim() || "");
				button.textContent = "Copied";
				setTimeout(() => { button.textContent = original; }, 2000);
			} catch {
				// Clipboard is permission-gated and refuses outside a secure context. Leave the label alone
				// rather than claiming a copy that did not happen.
			}
		});
	});
}

ThemeSwitcherButtonElement.configure(optionsFromDocument());
customElements.define("theme-switcher", ThemeSwitcherButtonElement);
wireCopyButtons();
