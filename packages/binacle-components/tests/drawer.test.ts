import {initDrawer} from "../src/drawer";

const PAGE = `
<header data-site-header>
	<div id="shell">
		<nav data-site-nav>
			<a class="site-header__link" href="#one" aria-current="page">One</a>
			<a class="site-header__link" href="#two">Two</a>
			<a class="site-header__link" href="#docs" data-other-site>Docs</a>
		</nav>
	</div>
</header>
<main id="main"></main>
<script id="script"></script>`;

// initDrawer listens on document and window, which outlive a test. A listener left from an earlier test
// would close the drawer first, with its own opener.
const added: [EventTarget, string, EventListenerOrEventListenerObject][] = [];

function trackListeners(target: EventTarget): void {
	const add = target.addEventListener.bind(target);
	jest.spyOn(target, "addEventListener").mockImplementation((type, listener, options) => {
		added.push([target, type, listener as EventListenerOrEventListenerObject]);
		add(type, listener, options);
	});
}

function element<T extends HTMLElement = HTMLElement>(selector: string): T {
	return document.querySelector<T>(selector) as T;
}

function burger(): HTMLElement {
	return element("[data-drawer-open]");
}

function panel(): HTMLElement {
	return element("#site-drawer");
}

function everyElementShows(shows: boolean): void {
	const rects = (shows ? [{}] : []) as unknown as DOMRectList;
	jest.spyOn(Element.prototype, "getClientRects").mockReturnValue(rects);
}

beforeEach(() => {
	trackListeners(document);
	trackListeners(window);
	document.body.innerHTML = PAGE;
});

afterEach(() => {
	added.splice(0).forEach(([target, type, listener]) => target.removeEventListener(type, listener));
	jest.restoreAllMocks();
});

describe("the drawer built from the header", () => {
	test("the burger opens the shell and the drawer follows the header", () => {
		initDrawer();

		expect(element("#shell").firstElementChild).toBe(burger());
		expect(element("[data-site-header]").nextElementSibling).toBe(panel());
		expect(panel().nextElementSibling).toBe(element(".drawer-backdrop"));
	});

	test("the site's pages and the other sites go in their own lists, with the drawer's class", () => {
		initDrawer();

		const pages = Array.from(document.querySelectorAll(".drawer__pages a"));
		const sites = Array.from(document.querySelectorAll(".drawer__sites a"));
		expect(pages.map((link) => link.textContent)).toEqual(["One", "Two"]);
		expect(sites.map((link) => link.textContent)).toEqual(["Docs"]);
		expect(pages.every((link) => link.className === "drawer__page-link")).toBe(true);
		expect(sites.every((link) => link.className === "drawer__site-link")).toBe(true);
	});

	test("a copy keeps the link's other attributes", () => {
		initDrawer();

		expect(element(".drawer__pages a").getAttribute("aria-current")).toBe("page");
	});

	test("a header marked data-nav-row gets no drawer", () => {
		element("[data-site-header]").setAttribute("data-nav-row", "");

		initDrawer();

		expect(panel()).toBeNull();
		expect(burger()).toBeNull();
	});

	test("a header with its own drawer button gets no second drawer", () => {
		element("#shell").insertAdjacentHTML("beforeend", '<button data-drawer-open="own"></button>');

		initDrawer();

		expect(panel()).toBeNull();
	});
});

describe("opening", () => {
	test("marks the drawer open and makes it a dialog", () => {
		initDrawer();

		burger().click();

		expect(panel().classList.contains("is-open")).toBe(true);
		expect(burger().getAttribute("aria-expanded")).toBe("true");
		expect(panel().getAttribute("role")).toBe("dialog");
		expect(panel().getAttribute("aria-modal")).toBe("true");
		expect(panel().getAttribute("aria-label")).toBe("Menu");
	});

	test("makes the rest of the page inert, but not the backdrop or a script", () => {
		initDrawer();

		burger().click();

		expect(element("#main").inert).toBe(true);
		expect(element("[data-site-header]").inert).toBe(true);
		expect(element(".drawer-backdrop").inert).toBeFalsy();
		expect(element("#script").inert).toBeFalsy();
	});

	test("reaches the elements beside each parent, up to the body", () => {
		document.body.innerHTML = `
			<aside id="beside"></aside>
			<div id="wrap">
				<button data-drawer-open="own"></button>
				<div id="own" data-drawer=""><button data-drawer-close></button></div>
			</div>`;
		initDrawer();

		burger().click();

		expect(element("#beside").inert).toBe(true);
		expect(element("[data-drawer-open]").inert).toBe(true);
		expect(element("#wrap").inert).toBeFalsy();
	});

	test("moves focus to the close button", () => {
		initDrawer();

		burger().click();

		expect(document.activeElement).toBe(element(".drawer__close"));
	});

	test("scrolls to the current page, leaving room above it", () => {
		initDrawer();
		Object.defineProperty(element('.drawer__pages [aria-current="page"]'), "offsetTop", {value: 500});

		burger().click();

		expect(panel().scrollTop).toBe(380);
	});

	test("a current page near the top leaves the drawer at the top", () => {
		initDrawer();
		Object.defineProperty(element('.drawer__pages [aria-current="page"]'), "offsetTop", {value: 50});
		panel().scrollTop = 200;

		burger().click();

		expect(panel().scrollTop).toBe(0);
	});
});

describe("closing", () => {
	function expectClosed(): void {
		expect(panel().classList.contains("is-open")).toBe(false);
		expect(burger().getAttribute("aria-expanded")).toBe("false");
		expect(panel().hasAttribute("role")).toBe(false);
		expect(panel().hasAttribute("aria-modal")).toBe(false);
		expect(panel().hasAttribute("aria-label")).toBe(false);
		expect(element("#main").inert).toBe(false);
	}

	beforeEach(() => {
		everyElementShows(true);
		initDrawer();
		burger().click();
	});

	test("the close button closes it and gives focus back", () => {
		element(".drawer__close").click();

		expectClosed();
		expect(document.activeElement).toBe(burger());
	});

	test("the backdrop closes it and gives focus back", () => {
		element(".drawer-backdrop").click();

		expectClosed();
		expect(document.activeElement).toBe(burger());
	});

	test("Escape closes it and gives focus back", () => {
		document.dispatchEvent(new KeyboardEvent("keydown", {key: "Escape"}));

		expectClosed();
		expect(document.activeElement).toBe(burger());
	});

	test("another key leaves it open", () => {
		document.dispatchEvent(new KeyboardEvent("keydown", {key: "Enter"}));

		expect(panel().classList.contains("is-open")).toBe(true);
	});

	test("a link in it closes it and leaves focus alone", () => {
		element(".drawer__pages a").click();

		expectClosed();
		expect(document.activeElement).not.toBe(burger());
	});

	test("a click on anything else in it leaves it open", () => {
		element(".drawer__inner").click();

		expect(panel().classList.contains("is-open")).toBe(true);
	});

	test("focus does not go back to a button that is gone from view", () => {
		everyElementShows(false);

		element(".drawer__close").click();

		expectClosed();
		expect(document.activeElement).not.toBe(burger());
	});

	test("a resize that hides its button closes it", () => {
		everyElementShows(false);

		window.dispatchEvent(new Event("resize"));

		expectClosed();
	});

	test("a resize that keeps its button leaves it open", () => {
		window.dispatchEvent(new Event("resize"));

		expect(panel().classList.contains("is-open")).toBe(true);
	});
});
