import {ThemeSwitcherButtonElement, optionsFromDocument} from "theme-switcher";

// The settings come from the page, so the button and the pre-paint script cannot disagree.
export function initThemeButton(): void {
	ThemeSwitcherButtonElement.configure(optionsFromDocument());
	customElements.define("theme-switcher", ThemeSwitcherButtonElement);
}
