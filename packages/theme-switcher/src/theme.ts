export type Theme = "light" | "dark";
export type DefaultTheme = Theme | "system";

// The theme itself. data-swap on any other element is the attribute swap, and the two must not collide.
const ATTRIBUTE = "data-theme";

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

export function resolveDefault(value: DefaultTheme): Theme {
	return value === "system" ? systemTheme() : value;
}

export function applyTheme(theme: Theme): void {
	document.documentElement.setAttribute(ATTRIBUTE, theme);
}

export function currentTheme(): Theme {
	const set = document.documentElement.getAttribute(ATTRIBUTE);
	return isTheme(set) ? set : "light";
}
