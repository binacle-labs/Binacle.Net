import {Cookies} from "cookies";
import type {CookieAttributes} from "cookies";

import {isTheme} from "./theme";
import type {Theme} from "./theme";
import type {ThemeStorage, ThemeSwitcherOptions} from "./options";

// The read half stands alone and touches nothing from the cookies package. The pre-paint script in <head>
// imports only this, and webpack drops everything below it - which is what keeps that bundle near a
// kilobyte. Route reading through createStorage and the whole write side rides along.
export function readStored(options: ThemeSwitcherOptions): Theme | undefined {
	if (typeof options.storage === "object") {
		return options.storage.read();
	}
	if (options.storage === "none") {
		return undefined;
	}
	const value = options.storage === "local"
		? readLocal(options.storageKey)
		: readCookie(options.storageKey);
	return isTheme(value) ? value : undefined;
}

function readCookie(key: string): string | undefined {
	// No decoding: isTheme lets nothing but the two literals through, and neither survives encoding.
	for (const pair of document.cookie.split("; ")) {
		const at = pair.indexOf("=");
		if (at > 0 && pair.slice(0, at) === key) {
			return pair.slice(at + 1);
		}
	}
	return undefined;
}

function readLocal(key: string): string | undefined {
	try {
		return localStorage.getItem(key) ?? undefined;
	} catch {
		// Private windows throw on read.
		return undefined;
	}
}

export function createStorage(options: ThemeSwitcherOptions): ThemeStorage {
	if (typeof options.storage === "object") {
		return options.storage;
	}
	return {
		read: () => readStored(options),
		write: writerFor(options)
	};
}

function writerFor(options: ThemeSwitcherOptions): (theme: Theme) => void {
	if (options.storage === "local") {
		return theme => writeLocal(options.storageKey, theme);
	}
	if (options.storage === "none") {
		return () => undefined;
	}
	return theme => writeCookie(options.storageKey, options.cookie, theme);
}

function writeCookie(key: string, attributes: CookieAttributes, theme: Theme): void {
	const secure = location.protocol === "https:";
	// A host-only cookie from before the shared domain is sent under the same name, and can win on read
	// forever. Removing it first is what stops the old choice sticking.
	if (attributes.domain) {
		Cookies.remove(key, {secure});
	}
	// Secure only where the browser will keep it. The cookies default is secure, and the API image is
	// commonly served over plain http on a LAN - there the cookie is dropped and the theme resets on every
	// page load.
	Cookies.set(key, theme, {...attributes, secure});
}

function writeLocal(key: string, theme: Theme): void {
	try {
		localStorage.setItem(key, theme);
	} catch {
		// Private windows throw on write. The choice still applies for this page load.
	}
}
