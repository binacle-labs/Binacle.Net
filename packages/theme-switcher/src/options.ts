import {isTheme} from "./theme";
import type {DefaultTheme, Theme} from "./theme";
import type {CookieAttributes} from "cookies";

export interface ThemeStorage {
	read(): Theme | undefined;
	write(theme: Theme): void;
}

export type StorageKind = "cookie" | "local" | "none";

export interface ThemeSwitcherOptions {
	// data-default-theme on the element wins over this.
	defaultTheme: DefaultTheme;
	storage: StorageKind | ThemeStorage;
	storageKey: string;
	// Read only when storage is "cookie".
	cookie: CookieAttributes;
}

export const DEFAULT_OPTIONS: ThemeSwitcherOptions = {
	defaultTheme: "system",
	storage: "cookie",
	storageKey: "theme",
	cookie: {expires: 365}
};

function asDefaultTheme(value: string | undefined): DefaultTheme {
	return value === "system" || isTheme(value) ? value : DEFAULT_OPTIONS.defaultTheme;
}

function asStorageKind(value: string | undefined): StorageKind {
	return value === "local" || value === "none" || value === "cookie" ? value : "cookie";
}

// cookie is merged rather than replaced: a caller passing only a domain would otherwise drop expires and
// get the cookies package's 90 days instead of a year.
export function mergeOptions(over: Partial<ThemeSwitcherOptions>): ThemeSwitcherOptions {
	return {...DEFAULT_OPTIONS, ...over, cookie: {...DEFAULT_OPTIONS.cookie, ...over.cookie}};
}

// One place a host declares its settings, so the pre-paint read and the switcher cannot disagree. A Jekyll
// site writes these from _config.yml, the UI module from Razor.
export function optionsFromDocument(): ThemeSwitcherOptions {
	const data = document.documentElement.dataset;
	return mergeOptions({
		defaultTheme: asDefaultTheme(data.defaultTheme),
		storage: asStorageKind(data.themeStorage),
		storageKey: data.themeKey || DEFAULT_OPTIONS.storageKey,
		// Absent everywhere the host is not on a shared parent domain - the image especially, where a
		// domain the host does not match makes the browser drop the cookie outright.
		cookie: {domain: data.themeDomain}
	});
}
