import {Cookies} from "cookies";

import ThemeSwitcherButtonElement from "../src/themeSwitcher";
import {DEFAULT_OPTIONS} from "../src/options";

// jsdom gives us a real customElements registry, so connectedCallback fires on appendChild. The tag can
// only be defined once per file - a second define throws.
beforeAll(() => {
	customElements.define("theme-switcher", ThemeSwitcherButtonElement);
});

function clearCookies(): void {
	for (const cookie of document.cookie.split("; ")) {
		const name = cookie.split("=")[0];
		if (name) {
			document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
		}
	}
}

function createSwitcher(defaultTheme?: string): ThemeSwitcherButtonElement {
	const element = document.createElement("theme-switcher") as ThemeSwitcherButtonElement;
	if (defaultTheme) {
		element.dataset.defaultTheme = defaultTheme;
	}
	return element;
}

function theme(): string | null {
	return document.documentElement.getAttribute("data-theme");
}

function button(element: ThemeSwitcherButtonElement): HTMLButtonElement {
	const found = element.querySelector("button");
	if (!found) {
		throw new Error("the switcher rendered no button");
	}
	return found;
}

beforeEach(() => {
	clearCookies();
	document.body.innerHTML = "";
	document.documentElement.removeAttribute("data-theme");
	document.documentElement.removeAttribute("data-theme-storage");
	ThemeSwitcherButtonElement.configure({...DEFAULT_OPTIONS, defaultTheme: "light"});
});

describe("picking the theme on connect", () => {
	test("data-default-theme of dark puts the html element in dark", () => {
		document.body.appendChild(createSwitcher("dark"));

		expect(theme()).toBe("dark");
	});

	test("data-default-theme of light puts the html element in light", () => {
		document.body.appendChild(createSwitcher("light"));

		expect(theme()).toBe("light");
	});

	test("no data-default-theme falls back to the configured default", () => {
		ThemeSwitcherButtonElement.configure({...DEFAULT_OPTIONS, defaultTheme: "dark"});

		document.body.appendChild(createSwitcher());

		expect(theme()).toBe("dark");
	});

	test("an unknown data-default-theme falls back to light rather than being written through", () => {
		document.body.appendChild(createSwitcher("drak"));

		expect(theme()).toBe("light");
	});

	test("an existing theme cookie beats the default", () => {
		Cookies.set("theme", "light");

		document.body.appendChild(createSwitcher("dark"));

		expect(theme()).toBe("light");
	});

	test("an existing theme cookie is not overwritten on connect", () => {
		Cookies.set("theme", "light");

		document.body.appendChild(createSwitcher("dark"));

		expect(Cookies.get("theme")).toBe("light");
	});

	test("the default theme is not written to storage", () => {
		document.body.appendChild(createSwitcher("dark"));

		expect(Cookies.get("theme")).toBeUndefined();
	});

	test("a stored value that is not a theme is ignored", () => {
		Cookies.set("theme", "purple");

		document.body.appendChild(createSwitcher("dark"));

		expect(theme()).toBe("dark");
	});
});

describe("system as the default", () => {
	function withSystemDark(dark: boolean): void {
		window.matchMedia = ((query: string) => ({
			matches: dark,
			media: query,
			addEventListener: () => undefined,
			removeEventListener: () => undefined
		})) as unknown as typeof window.matchMedia;
	}

	test("a dark machine gets dark", () => {
		withSystemDark(true);

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBe("dark");
	});

	test("a light machine gets light", () => {
		withSystemDark(false);

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBe("light");
	});

	test("a stored choice still beats the machine", () => {
		withSystemDark(true);
		Cookies.set("theme", "light");

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBe("light");
	});
});

describe("clicking", () => {
	test("light becomes dark", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(theme()).toBe("dark");
	});

	test("dark becomes light", () => {
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		button(element).click();

		expect(theme()).toBe("light");
	});

	test("the new theme is written to storage", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(Cookies.get("theme")).toBe("dark");
	});

	test("a second click writes the theme back", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();
		button(element).click();

		expect(Cookies.get("theme")).toBe("light");
	});

	test("a themeChanged event carries the new theme", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);
		const listener = jest.fn();
		window.addEventListener("themeChanged", listener);

		button(element).click();

		expect(listener.mock.calls[0][0].detail).toEqual({theme: "dark"});
	});
});

describe("the control", () => {
	test("it renders a real button, so a keyboard can reach it", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).type).toBe("button");
	});

	test("the button says what it does", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).getAttribute("aria-label")).toBe("Switch to the dark theme");
	});

	test("the label follows the theme", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(button(element).getAttribute("aria-label")).toBe("Switch to the light theme");
	});

	test("aria-pressed follows the theme", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(button(element).getAttribute("aria-pressed")).toBe("true");
	});

	test("a light theme renders the icon that offers dark", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(element.querySelector("i")?.textContent).toBe("dark_mode");
	});

	test("a dark theme renders the icon that offers light", () => {
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(element.querySelector("i")?.textContent).toBe("light_mode");
	});

	test("a host label is used instead of an icon, and kept in step", () => {
		const element = createSwitcher("light");
		element.innerHTML = "<span data-theme-label>Theme</span>";
		document.body.appendChild(element);

		expect(element.querySelector("[data-theme-label]")?.textContent).toBe("Dark");
	});

	test("a host label moves inside the button, so the whole control is clickable", () => {
		const element = createSwitcher("light");
		element.innerHTML = "<span data-theme-label>Theme</span>";
		document.body.appendChild(element);

		expect(button(element).querySelector("[data-theme-label]")).not.toBeNull();
	});

	test("a host label means no material icon is rendered", () => {
		const element = createSwitcher("light");
		element.innerHTML = "<span data-theme-label>Theme</span>";
		document.body.appendChild(element);

		expect(element.querySelector("i")).toBeNull();
	});

	test("the host label follows a click", () => {
		const element = createSwitcher("light");
		element.innerHTML = "<span data-theme-label>Theme</span>";
		document.body.appendChild(element);

		button(element).click();

		expect(element.querySelector("[data-theme-label]")?.textContent).toBe("Light");
	});

	test("data-button-class is put on the rendered button, so the host can style it", () => {
		const element = createSwitcher("light");
		element.dataset.buttonClass = "theme-toggle";
		document.body.appendChild(element);

		expect(button(element).className).toBe("theme-toggle");
	});

	test("the icon swaps when the theme is clicked", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(element.querySelector("i")?.textContent).toBe("light_mode");
	});
});

describe("elements marked with data-swap", () => {
	test("a dark theme gets the data-darktheme value", () => {
		document.body.innerHTML =
			"<img data-swap=\"src\" data-darktheme=\"/dark.png\" data-lighttheme=\"/light.png\">";

		document.body.appendChild(createSwitcher("dark"));

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/dark.png");
	});

	test("a click moves them to the other theme", () => {
		document.body.innerHTML =
			"<img data-swap=\"src\" data-darktheme=\"/dark.png\" data-lighttheme=\"/light.png\">";
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		button(element).click();

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/light.png");
	});

	test("a missing value skips the element rather than writing the string undefined", () => {
		document.body.innerHTML = "<img data-swap=\"src\">";

		document.body.appendChild(createSwitcher("dark"));

		expect(document.querySelector("img")?.hasAttribute("src")).toBe(false);
	});

	test("the html element is not treated as a swap target", () => {
		document.body.appendChild(createSwitcher("dark"));

		expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
	});
});

describe("disconnecting", () => {
	test("the browser lifecycle hook is implemented", () => {
		const element = createSwitcher("light");

		expect(typeof element.disconnectedCallback).toBe("function");
	});

	test("the click listener does not outlive the element", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);
		const control = button(element);

		element.remove();
		control.click();

		expect(theme()).toBe("light");
	});
});
