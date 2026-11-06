import {DEFAULT_OPTIONS, mergeOptions} from "./options";
import type {ThemeStorage, ThemeSwitcherOptions} from "./options";
import {applyMode, currentMode, currentTheme, isTheme, nextMode, startingMode} from "./theme";
import type {DefaultTheme, Theme, ThemeMode} from "./theme";
import {createStorage} from "./storage";

export default class ThemeSwitcherButtonElement extends HTMLElement {
	private static options: ThemeSwitcherOptions = DEFAULT_OPTIONS;

	private storage: ThemeStorage | null = null;
	private button: HTMLButtonElement | null = null;
	private readonly onClick = (): void => {
		this.setMode(nextMode(currentMode()));
	};

	// Custom elements are built by the parser, so there is nowhere to pass options. Call this before
	// customElements.define.
	static configure(options: Partial<ThemeSwitcherOptions>): void {
		ThemeSwitcherButtonElement.options = mergeOptions(options);
	}

	connectedCallback(): void {
		const options = ThemeSwitcherButtonElement.options;
		this.storage = createStorage(options);
		this.button = renderButton(this, this.onClick);

		const mode = this.storage.read()
			?? startingMode(asDefaultTheme(this.dataset.defaultTheme) ?? options.defaultTheme);
		applyMode(mode);

		label(this.button, mode);
		swapAttributes(currentTheme());
	}

	disconnectedCallback(): void {
		this.button?.removeEventListener("click", this.onClick);
	}

	private setMode(mode: ThemeMode): void {
		applyMode(mode);
		if (mode === "auto") {
			this.storage?.clear();
		} else {
			this.storage?.write(mode);
		}
		if (this.button) {
			label(this.button, mode);
		}
		swapAttributes(currentTheme());
	}
}

function renderButton(host: ThemeSwitcherButtonElement, onClick: () => void): HTMLButtonElement {
	// A custom element takes no focus and answers no key. Without a real button the theme cannot be
	// changed from a keyboard or announced by a screen reader.
	const button = document.createElement("button");
	button.type = "button";
	button.className = host.dataset.buttonClass ?? "";
	button.addEventListener("click", onClick);
	host.appendChild(button);
	return button;
}

// The host's CSS draws the icon and hides this text from sight. A screen reader still reads it.
function label(button: HTMLButtonElement, mode: ThemeMode): void {
	button.textContent = `Theme: ${mode}`;
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
