import {DEFAULT_OPTIONS, mergeOptions} from "./options";
import type {ThemeStorage, ThemeSwitcherOptions} from "./options";
import {applyTheme, currentTheme, isTheme, resolveDefault} from "./theme";
import type {DefaultTheme, Theme} from "./theme";
import {createStorage} from "./storage";

// What render built. Held as one thing so nothing has to guard three fields that are only ever null
// together, before render has run.
interface Control {
	button: HTMLButtonElement;
	icon: HTMLElement | null;
	text: HTMLElement | null;
}

export default class ThemeSwitcherButtonElement extends HTMLElement {
	private static options: ThemeSwitcherOptions = DEFAULT_OPTIONS;

	private storage: ThemeStorage | null = null;
	private control: Control | null = null;
	private readonly onClick = (): void => {
		this.setTheme(currentTheme() === "dark" ? "light" : "dark");
	};

	// Custom elements are built by the parser, so there is nowhere to pass options. Call this before
	// customElements.define.
	static configure(options: Partial<ThemeSwitcherOptions>): void {
		ThemeSwitcherButtonElement.options = mergeOptions(options);
	}

	connectedCallback(): void {
		const options = ThemeSwitcherButtonElement.options;
		this.storage = createStorage(options);
		this.control = renderControl(this, this.onClick);

		const chosen = this.storage.read()
			?? resolveDefault(asDefaultTheme(this.dataset.defaultTheme) ?? options.defaultTheme);
		applyTheme(chosen);

		label(this.control, chosen);
		swapAttributes(chosen);
	}

	disconnectedCallback(): void {
		this.control?.button.removeEventListener("click", this.onClick);
	}

	private setTheme(theme: Theme): void {
		applyTheme(theme);
		this.storage?.write(theme);
		if (this.control) {
			label(this.control, theme);
		}
		swapAttributes(theme);
		window.dispatchEvent(new CustomEvent("themeChanged", {detail: {theme}}));
	}
}

function renderControl(host: ThemeSwitcherButtonElement, onClick: () => void): Control {
	// A custom element takes no focus and answers no key. Without a real button the theme cannot be
	// changed from a keyboard or announced by a screen reader.
	const button = document.createElement("button");
	button.type = "button";
	button.className = host.dataset.buttonClass ?? "";

	// A host wanting words rather than an icon puts its own element in and gets its text kept in step.
	// An empty switcher gets the material ligature the BeerCSS hosts expect.
	const text = host.querySelector<HTMLElement>("[data-theme-label]");
	let icon: HTMLElement | null = null;
	if (text) {
		button.appendChild(text);
	} else {
		icon = document.createElement("i");
		icon.classList.add("page", "top", "active");
		button.appendChild(icon);
	}

	button.addEventListener("click", onClick);
	host.appendChild(button);
	return {button, icon, text};
}

function label(control: Control, theme: Theme): void {
	const dark = theme === "dark";
	const offers = dark ? "light" : "dark";
	if (control.icon) {
		control.icon.textContent = `${offers}_mode`;
	}
	if (control.text) {
		control.text.textContent = dark ? "Light" : "Dark";
	}
	control.button.setAttribute("aria-pressed", String(dark));
	control.button.setAttribute("aria-label", `Switch to the ${offers} theme`);
}

// Makes something CSS cannot reach - an img src, say - follow the theme.
function swapAttributes(theme: Theme): void {
	document.querySelectorAll<HTMLElement>("[data-swap]").forEach(element => {
		const attribute = element.dataset.swap;
		const value = theme === "dark" ? element.dataset.darktheme : element.dataset.lighttheme;
		if (attribute && value !== undefined) {
			element.setAttribute(attribute, value);
		}
	});
}

function asDefaultTheme(value: string | undefined): DefaultTheme | undefined {
	return value === "system" || isTheme(value) ? value : undefined;
}
