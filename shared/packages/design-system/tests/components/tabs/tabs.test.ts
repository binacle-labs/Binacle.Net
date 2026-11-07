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
