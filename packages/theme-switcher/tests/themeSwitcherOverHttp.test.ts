/**
 * @jest-environment jsdom
 * @jest-environment-options {"url": "http://localhost/"}
 */
import {Cookies} from "cookies";

import ThemeSwitcherButtonElement from "../src/themeSwitcher";
import {DEFAULT_OPTIONS} from "../src/options";

// Its own file because the origin is set per file, and the rest of the suite runs on https. The API image is
// commonly served over plain http on a LAN, and a secure cookie is dropped there - so without the protocol
// check in the cookie storage the theme resets on every page load and every assertion below reads back
// nothing.
beforeAll(() => {
	customElements.define("theme-switcher", ThemeSwitcherButtonElement);
});

function createSwitcher(defaultTheme: string): ThemeSwitcherButtonElement {
	const element = document.createElement("theme-switcher") as ThemeSwitcherButtonElement;
	element.dataset.defaultTheme = defaultTheme;
	return element;
}

function click(element: ThemeSwitcherButtonElement): void {
	element.querySelector("button")?.click();
}

// Without this the previous test's theme is still in the jar, so the next switcher connects dark and the
// click under test toggles the other way.
function clearCookies(): void {
	for (const cookie of document.cookie.split("; ")) {
		const name = cookie.split("=")[0];
		if (name) {
			document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
		}
	}
}

beforeEach(() => {
	clearCookies();
	document.body.innerHTML = "";
	document.documentElement.removeAttribute("data-theme");
	ThemeSwitcherButtonElement.configure({...DEFAULT_OPTIONS, defaultTheme: "light"});
});

test("the origin really is insecure, or the rest of this file proves nothing", () => {
	expect(location.protocol).toBe("http:");
});

test("the chosen theme is written to a cookie", () => {
	const element = createSwitcher("light");
	document.body.appendChild(element);

	click(element);

	expect(Cookies.get("theme")).toBe("dark");
});

test("the cookie is still there for the next page load", () => {
	const element = createSwitcher("light");
	document.body.appendChild(element);
	click(element);

	document.body.innerHTML = "";
	document.documentElement.removeAttribute("data-theme");
	document.body.appendChild(createSwitcher("light"));

	expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
});
