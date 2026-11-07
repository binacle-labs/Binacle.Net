import {initStickySidebar} from "../../../layout/sidebar/stickySidebar";

function menu(): HTMLElement {
	return document.querySelector<HTMLElement>("[data-sidebar-inner]") as HTMLElement;
}

function footerAt(top: number): void {
	const footer = document.querySelector("[data-site-footer]") as Element;
	jest.spyOn(footer, "getBoundingClientRect").mockReturnValue({top} as DOMRect);
}

function below(): string {
	return menu().style.getPropertyValue("--below");
}

beforeEach(() => {
	jest.restoreAllMocks();
	document.body.innerHTML = "<div data-sidebar-inner></div><footer data-site-footer></footer>";
});

test("a footer past the bottom of the window takes nothing off the menu", () => {
	footerAt(window.innerHeight + 200);

	initStickySidebar();

	expect(below()).toBe("0px");
});

test("a footer in the window takes its height in view off the menu", () => {
	footerAt(window.innerHeight - 100);

	initStickySidebar();

	expect(below()).toBe("100px");
});

test("scrolling updates it", () => {
	footerAt(window.innerHeight);
	initStickySidebar();
	footerAt(window.innerHeight - 40);

	window.dispatchEvent(new Event("scroll"));

	expect(below()).toBe("40px");
});

test("resizing updates it", () => {
	footerAt(window.innerHeight);
	initStickySidebar();
	footerAt(window.innerHeight - 60);

	window.dispatchEvent(new Event("resize"));

	expect(below()).toBe("60px");
});

test("a page with no footer leaves the menu alone", () => {
	document.querySelector("[data-site-footer]")?.remove();

	initStickySidebar();

	expect(below()).toBe("");
});
