import {ThemeSwitcherButtonElement} from "@binacle/theme-switcher";
import {initThemeButton} from "../../../components/theme-button/themeButton";

describe("initThemeButton", () => {
	it("defines the element with the settings the page states", () => {
		document.documentElement.dataset.themeKey = "site-theme";
		const configure = jest.spyOn(ThemeSwitcherButtonElement, "configure");

		initThemeButton();

		expect(customElements.get("theme-switcher")).toBe(ThemeSwitcherButtonElement);
		expect(configure).toHaveBeenCalledWith(expect.objectContaining({storageKey: "site-theme"}));
	});
});
