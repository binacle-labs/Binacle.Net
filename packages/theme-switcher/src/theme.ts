export type Theme = "light" | "dark";
export type DefaultTheme = Theme | "system";
// Where the switcher stands. Only a Theme is ever stored: auto is having no choice.
export type ThemeMode = "auto" | Theme;

// The theme itself. data-swap on any other element is the attribute swap, and the two must not collide.
const ATTRIBUTE = "data-theme";

const ORDER: readonly ThemeMode[] = ["auto", "light", "dark"];

export function isTheme(value: string | null | undefined): value is Theme {
	return value === "light" || value === "dark";
}

export function systemTheme(): Theme {
	// Old browsers have no matchMedia at all. The typeof check is what tells TypeScript that.
	if (typeof window.matchMedia !== "function") {
		return "light";
	}
	return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export function startingMode(value: DefaultTheme): ThemeMode {
	return value === "system" ? "auto" : value;
}

export function nextMode(mode: ThemeMode): ThemeMode {
	return ORDER[(ORDER.indexOf(mode) + 1) % ORDER.length];
}

export function applyMode(mode: ThemeMode): void {
	// No attribute is how the stylesheet is told to follow the system.
	if (mode === "auto") {
		document.documentElement.removeAttribute(ATTRIBUTE);
	} else {
		document.documentElement.setAttribute(ATTRIBUTE, mode);
	}
}

export function currentMode(): ThemeMode {
	const set = document.documentElement.getAttribute(ATTRIBUTE);
	return isTheme(set) ? set : "auto";
}

// The theme on screen, which under auto is the system's.
export function currentTheme(): Theme {
	const mode = currentMode();
	return mode === "auto" ? systemTheme() : mode;
}
