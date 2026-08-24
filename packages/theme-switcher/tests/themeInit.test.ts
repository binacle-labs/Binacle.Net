import {Cookies} from "cookies";

import initTheme from "../src/themeInit";
import {optionsFromDocument} from "../src/options";
import {createStorage} from "../src/storage";
import {mergeOptions} from "../src/options";

function clearCookies(): void {
	for (const cookie of document.cookie.split("; ")) {
		const name = cookie.split("=")[0];
		if (name) {
			document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
		}
	}
}

function theme(): string | null {
	return document.documentElement.getAttribute("data-theme");
}

beforeEach(() => {
	jest.restoreAllMocks();
	clearCookies();
	localStorage.clear();
	const root = document.documentElement;
	for (const name of ["data-theme", "data-default-theme", "data-theme-storage", "data-theme-key", "data-theme-domain"]) {
		root.removeAttribute(name);
	}
});

describe("the settings the host declares on the html element", () => {
	test("an unknown storage falls back to the cookie", () => {
		document.documentElement.dataset.themeStorage = "carrier pigeon";

		expect(optionsFromDocument().storage).toBe("cookie");
	});

	test("an unknown default falls back to system", () => {
		document.documentElement.dataset.defaultTheme = "drak";

		expect(optionsFromDocument().defaultTheme).toBe("system");
	});

	test("no domain is set when the host declares none", () => {
		expect(optionsFromDocument().cookie.domain).toBeUndefined();
	});

	test("the declared domain is carried into the cookie attributes", () => {
		document.documentElement.dataset.themeDomain = ".binacle.net";

		expect(optionsFromDocument().cookie.domain).toBe(".binacle.net");
	});
});

describe("the pre-paint read", () => {
	test("a stored cookie is on the html element before anything else runs", () => {
		Cookies.set("theme", "dark");

		initTheme();

		expect(theme()).toBe("dark");
	});

	test("it reads local storage when the host asks for it", () => {
		document.documentElement.dataset.themeStorage = "local";
		localStorage.setItem("theme", "dark");

		initTheme();

		expect(theme()).toBe("dark");
	});

	test("it reads the key the host declared", () => {
		document.documentElement.dataset.themeKey = "binacle-theme";
		Cookies.set("binacle-theme", "dark");

		initTheme();

		expect(theme()).toBe("dark");
	});

	test("nothing stored falls back to the declared default", () => {
		document.documentElement.dataset.defaultTheme = "dark";

		initTheme();

		expect(theme()).toBe("dark");
	});

	test("storage of none never reads anything back", () => {
		document.documentElement.dataset.themeStorage = "none";
		document.documentElement.dataset.defaultTheme = "light";
		Cookies.set("theme", "dark");

		initTheme();

		expect(theme()).toBe("light");
	});
});

// jsdom keeps one cookie per name, so it cannot reproduce a host-only and a domain cookie coexisting -
// which is the whole failure this guards. What is pinned here is the call: with a domain, the host-only
// cookie is removed first; without one, nothing is removed. The effect itself is a by-eye check.
describe("the shared cookie", () => {
	test("a write with a domain removes the host-only cookie first", () => {
		const remove = jest.spyOn(Cookies, "remove");
		document.documentElement.dataset.themeDomain = "localhost";

		createStorage(optionsFromDocument()).write("dark");

		expect(remove).toHaveBeenCalledWith("theme", expect.anything());
	});

	test("a write with no domain removes nothing", () => {
		const remove = jest.spyOn(Cookies, "remove");

		createStorage(optionsFromDocument()).write("dark");

		expect(remove).not.toHaveBeenCalled();
	});

	test("the value written is the new theme", () => {
		document.documentElement.dataset.themeDomain = "localhost";

		createStorage(optionsFromDocument()).write("dark");

		expect(Cookies.get("theme")).toBe("dark");
	});
});

describe("merging settings over the defaults", () => {
	test("a cookie domain on its own keeps the year-long expiry", () => {
		const options = mergeOptions({cookie: {domain: ".binacle.net"}});

		expect(options.cookie.expires).toBe(365);
	});

	test("and keeps the domain", () => {
		const options = mergeOptions({cookie: {domain: ".binacle.net"}});

		expect(options.cookie.domain).toBe(".binacle.net");
	});

	test("the settings read off the document keep it too", () => {
		document.documentElement.dataset.themeDomain = ".binacle.net";

		expect(optionsFromDocument().cookie.expires).toBe(365);
	});
});
