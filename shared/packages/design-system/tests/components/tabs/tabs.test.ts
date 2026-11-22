import {initTabs} from "../../../components/tabs/tabs";

function tabs(): HTMLElement[] {
	return Array.from(document.querySelectorAll<HTMLElement>('[role="tab"]'));
}

function panels(): HTMLElement[] {
	return Array.from(document.querySelectorAll<HTMLElement>('[role="tabpanel"]'));
}

beforeEach(() => {
	document.body.innerHTML = `
		<div data-tabs>
			<button role="tab" aria-selected="true"></button>
			<button role="tab" aria-selected="false"></button>
			<div role="tabpanel"></div>
			<div role="tabpanel" hidden></div>
		</div>
		<div data-tabs>
			<button role="tab" aria-selected="true"></button>
			<div role="tabpanel"></div>
		</div>`;
	initTabs();
});

test("a click selects its tab and no other", () => {
	tabs()[1].click();

	expect(tabs()[0].getAttribute("aria-selected")).toBe("false");
	expect(tabs()[1].getAttribute("aria-selected")).toBe("true");
});

test("a click shows the panel in the same place and hides the rest", () => {
	tabs()[1].click();

	expect(panels()[0].hidden).toBe(true);
	expect(panels()[1].hidden).toBe(false);
});

test("a click can go back", () => {
	tabs()[1].click();
	tabs()[0].click();

	expect(tabs()[0].getAttribute("aria-selected")).toBe("true");
	expect(panels()[0].hidden).toBe(false);
	expect(panels()[1].hidden).toBe(true);
});

test("a click leaves another set of tabs alone", () => {
	tabs()[1].click();

	expect(tabs()[2].getAttribute("aria-selected")).toBe("true");
	expect(panels()[2].hidden).toBe(false);
});

function press(tab: HTMLElement, key: string): KeyboardEvent {
	const event = new KeyboardEvent("keydown", {key, bubbles: true, cancelable: true});
	tab.dispatchEvent(event);
	return event;
}

test("each tab and its panel point at each other", () => {
	tabs().forEach((tab, index) => {
		const panel = panels()[index];
		expect(tab.getAttribute("aria-controls")).toBe(panel.id);
		expect(panel.getAttribute("aria-labelledby")).toBe(tab.id);
	});
});

test("every id is unique on the page", () => {
	const ids = [...tabs(), ...panels()].map((element) => element.id);

	expect(new Set(ids).size).toBe(ids.length);
});

test("an id the markup gives is kept", () => {
	document.body.innerHTML = `
		<div data-tabs>
			<button role="tab" id="own-tab" aria-selected="true"></button>
			<div role="tabpanel" id="own-panel"></div>
		</div>`;
	initTabs();

	expect(tabs()[0].getAttribute("aria-controls")).toBe("own-panel");
	expect(panels()[0].getAttribute("aria-labelledby")).toBe("own-tab");
});

test("only the selected tab is a Tab stop", () => {
	expect(tabs()[0].tabIndex).toBe(0);
	expect(tabs()[1].tabIndex).toBe(-1);

	tabs()[1].click();

	expect(tabs()[0].tabIndex).toBe(-1);
	expect(tabs()[1].tabIndex).toBe(0);
});

test("every panel is a Tab stop", () => {
	panels().forEach((panel) => expect(panel.tabIndex).toBe(0));
});

test("the right arrow selects and focuses the next tab", () => {
	const event = press(tabs()[0], "ArrowRight");

	expect(event.defaultPrevented).toBe(true);
	expect(tabs()[1].getAttribute("aria-selected")).toBe("true");
	expect(document.activeElement).toBe(tabs()[1]);
	expect(panels()[1].hidden).toBe(false);
	expect(panels()[0].hidden).toBe(true);
});

test("the arrows wrap round at both ends", () => {
	press(tabs()[1], "ArrowRight");
	expect(document.activeElement).toBe(tabs()[0]);

	press(tabs()[0], "ArrowLeft");
	expect(document.activeElement).toBe(tabs()[1]);
});

test("Home and End go to the first and last tab", () => {
	press(tabs()[0], "End");
	expect(tabs()[1].getAttribute("aria-selected")).toBe("true");

	press(tabs()[1], "Home");
	expect(tabs()[0].getAttribute("aria-selected")).toBe("true");
});

test("any other key does nothing", () => {
	const event = press(tabs()[0], "a");

	expect(event.defaultPrevented).toBe(false);
	expect(tabs()[0].getAttribute("aria-selected")).toBe("true");
});

test("a key leaves another set of tabs alone", () => {
	press(tabs()[0], "ArrowRight");

	expect(tabs()[2].getAttribute("aria-selected")).toBe("true");
	expect(panels()[2].hidden).toBe(false);
});
