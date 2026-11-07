import {initNavFade} from "../../../layout/site-header/navFade";

function nav(): HTMLElement {
	return document.querySelector<HTMLElement>("[data-site-nav]") as HTMLElement;
}

function measure(scrollLeft: number, clientWidth: number, scrollWidth: number): void {
	nav().scrollLeft = scrollLeft;
	Object.defineProperty(nav(), "clientWidth", {value: clientWidth, configurable: true});
	Object.defineProperty(nav(), "scrollWidth", {value: scrollWidth, configurable: true});
}

function more(): string | null {
	return nav().getAttribute("data-more");
}

beforeEach(() => {
	document.body.innerHTML = "<nav data-site-nav></nav>";
});

describe("the side that has more", () => {
	test("at the start, the right", () => {
		measure(0, 100, 300);

		initNavFade();

		expect(more()).toBe("right");
	});

	test("in the middle, both", () => {
		measure(100, 100, 300);

		initNavFade();

		expect(more()).toBe("both");
	});

	test("at the end, the left", () => {
		measure(200, 100, 300);

		initNavFade();

		expect(more()).toBe("left");
	});

	test("a nav that fits has no mark", () => {
		nav().setAttribute("data-more", "both");
		measure(0, 100, 100);

		initNavFade();

		expect(more()).toBeNull();
	});

	test("one pixel either way does not count as more", () => {
		measure(1, 100, 102);

		initNavFade();

		expect(more()).toBeNull();
	});
});

describe("what updates it", () => {
	beforeEach(() => {
		measure(0, 100, 300);
		initNavFade();
	});

	test("scrolling the nav", () => {
		measure(200, 100, 300);

		nav().dispatchEvent(new Event("scroll"));

		expect(more()).toBe("left");
	});

	test("resizing the window", () => {
		measure(0, 300, 300);

		window.dispatchEvent(new Event("resize"));

		expect(more()).toBeNull();
	});
});

describe("fonts", () => {
	afterEach(() => {
		Reflect.deleteProperty(document, "fonts");
	});

	test("the mark is read again once they load", async () => {
		const ready = Promise.resolve();
		Object.defineProperty(document, "fonts", {value: {ready}, configurable: true});
		measure(0, 100, 100);
		initNavFade();
		measure(0, 100, 300);

		await ready;

		expect(more()).toBe("right");
	});
});
