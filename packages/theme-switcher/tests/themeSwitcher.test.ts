import {Cookies} from "cookies";

import ThemeSwitcherButtonElement from "../src/themeSwitcher";
import {DEFAULT_OPTIONS} from "../src/options";
import {systemTheme} from "../src/theme";

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

function withSystemDark(dark: boolean): void {
	window.matchMedia = ((query: string) => ({
		matches: dark,
		media: query,
		addEventListener: () => undefined,
		removeEventListener: () => undefined
	})) as unknown as typeof window.matchMedia;
}

beforeEach(() => {
	clearCookies();
	withSystemDark(false);
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
	test("no attribute is set, so the tokens follow the machine", () => {
		withSystemDark(true);

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBeNull();
	});

	test("an attribute already there is removed", () => {
		document.documentElement.setAttribute("data-theme", "dark");

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBeNull();
	});

	test("the button starts on auto", () => {
		const element = createSwitcher("system");

		document.body.appendChild(element);

		expect(button(element).textContent).toBe("Theme: auto");
	});

	test("a stored choice still beats the machine", () => {
		withSystemDark(true);
		Cookies.set("theme", "light");

		document.body.appendChild(createSwitcher("system"));

		expect(theme()).toBe("light");
	});

	test("a browser with no matchMedia counts as light", () => {
		window.matchMedia = undefined as unknown as typeof window.matchMedia;

		expect(systemTheme()).toBe("light");
	});
});

describe("clicking", () => {
	test("auto becomes light", () => {
		const element = createSwitcher("system");
		document.body.appendChild(element);

		button(element).click();

		expect(theme()).toBe("light");
	});

	test("light becomes dark", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(theme()).toBe("dark");
	});

	test("dark becomes auto, with the attribute gone", () => {
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		button(element).click();

		expect(theme()).toBeNull();
	});

	test("auto on a dark machine still goes to light first", () => {
		withSystemDark(true);
		const element = createSwitcher("system");
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

	test("going to auto clears the stored choice", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();
		button(element).click();

		expect(Cookies.get("theme")).toBeUndefined();
	});

	test("going on from auto stores a choice again", () => {
		Cookies.set("theme", "dark");
		const element = createSwitcher("system");
		document.body.appendChild(element);

		button(element).click();
		button(element).click();

		expect(Cookies.get("theme")).toBe("light");
	});

	test("going to auto calls clear on a host's own storage", () => {
		const storage = {read: () => "dark" as const, write: jest.fn(), clear: jest.fn()};
		ThemeSwitcherButtonElement.configure({storage});
		const element = createSwitcher();
		document.body.appendChild(element);

		button(element).click();

		expect(storage.clear).toHaveBeenCalledTimes(1);
	});

	test("a themeChanged event carries the new theme", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);
		const listener = jest.fn();
		window.addEventListener("themeChanged", listener);

		button(element).click();

		expect(listener.mock.calls[0][0].detail).toEqual({theme: "dark"});
	});

	test("under auto the event carries the machine's theme, for a listener that knows only two", () => {
		withSystemDark(true);
		const element = createSwitcher("dark");
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

	test("the button says where it is", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).textContent).toBe("Theme: light");
	});

	test("the text follows a click", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(button(element).textContent).toBe("Theme: dark");
	});

	test("the text says auto after the last click of the round", () => {
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		button(element).click();

		expect(button(element).textContent).toBe("Theme: auto");
	});

	test("nothing overrides the text for a screen reader", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).hasAttribute("aria-label")).toBe(false);
	});

	test("it is not announced as a two-way toggle", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).hasAttribute("aria-pressed")).toBe(false);
	});

	test("it draws no icon element", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(element.querySelector("i")).toBeNull();
	});

	test("data-button-class is put on the rendered button, so the host can style it", () => {
		const element = createSwitcher("light");
		element.dataset.buttonClass = "theme-button";
		document.body.appendChild(element);

		expect(button(element).className).toBe("theme-button");
	});

	test("a host that names no class gets none", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(button(element).className).toBe("");
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
		const element = createSwitcher("light");
		document.body.appendChild(element);

		button(element).click();

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/dark.png");
	});

	test("under auto they follow the machine", () => {
		withSystemDark(true);
		document.body.innerHTML =
			"<img data-swap=\"src\" data-darktheme=\"/dark.png\" data-lighttheme=\"/light.png\">";

		document.body.appendChild(createSwitcher("system"));

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/dark.png");
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
