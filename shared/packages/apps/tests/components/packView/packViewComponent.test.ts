import {packViewComponent, PackScene} from "../../../src/components/packView/packViewComponent";
import {createPackView} from "@binacle/design-system";

jest.mock("@binacle/design-system", () => ({createPackView: jest.fn()}));

const bin = {length: 10, width: 20, height: 30};
const items = [{length: 1, width: 2, height: 3, x: 0, y: 0, z: 0}];

function createComponent() {
	const view = {preview: jest.fn(), show: jest.fn(), wait: jest.fn(), highlight: jest.fn(), clear: jest.fn()};
	(createPackView as jest.Mock).mockReturnValue(view);

	const figure = document.createElement("figure");
	const $dispatch = jest.fn();
	const $logger = {error: jest.fn()};
	const component = Object.assign(packViewComponent(), {$el: figure, $dispatch, $logger});
	component.init();

	return {component, view, figure, $dispatch, $logger};
}

function answer(scene: PackScene | null) {
	return async () => scene;
}

describe("starting", () => {
	test("the view is built on the component's own element", () => {
		const {figure} = createComponent();

		expect(createPackView).toHaveBeenCalledWith(figure);
	});
});

describe("updating the scene", () => {
	test("the loader shows while the promise runs", async () => {
		const {component, view} = createComponent();
		let finish: (scene: PackScene) => void = () => undefined;

		const update = component.updateScene(() => new Promise<PackScene>(resolve => finish = resolve));

		expect(view.wait).toHaveBeenCalledWith(null);
		expect(view.show).not.toHaveBeenCalled();

		finish({bin, items});
		await update;

		expect(view.show).toHaveBeenCalledTimes(1);
	});

	test("the loader keeps the bin already drawn", async () => {
		const {component, view} = createComponent();
		await component.updateScene(answer({bin}));

		await component.updateScene(answer({bin, items}));

		expect(view.wait).toHaveBeenLastCalledWith(bin);
	});

	test("a bin and items is an answer", async () => {
		const {component, view} = createComponent();

		await component.updateScene(answer({bin, items}));

		expect(view.show).toHaveBeenCalledWith(bin, items, {note: undefined});
	});

	test("the note goes with the answer", async () => {
		const {component, view} = createComponent();

		await component.updateScene(answer({bin, items, note: "Note"}));

		expect(view.show).toHaveBeenCalledWith(bin, items, {note: "Note"});
	});

	test("a bin with no items draws the bin alone", async () => {
		const {component, view} = createComponent();

		await component.updateScene(answer({bin}));

		expect(view.preview).toHaveBeenCalledWith(bin);
		expect(view.show).not.toHaveBeenCalled();
	});

	test("an empty items list is an empty answer, not the bin alone", async () => {
		const {component, view} = createComponent();

		await component.updateScene(answer({bin, items: [], note: "Nothing fits"}));

		expect(view.show).toHaveBeenCalledWith(bin, [], {note: "Nothing fits"});
		expect(view.preview).not.toHaveBeenCalled();
	});

	test.each([
		["nothing", null],
		["no bin", {items}]
	])("%s clears the view", async (_name, scene) => {
		const {component, view} = createComponent();

		await component.updateScene(answer(scene));

		expect(view.clear).toHaveBeenCalledTimes(1);
	});
});

describe("a slow scene overtaken by a newer one", () => {
	function slow() {
		let finish: (scene: PackScene) => void = () => undefined;
		let fail: (reason: Error) => void = () => undefined;
		const promise = new Promise<PackScene>((resolve, reject) => {
			finish = resolve;
			fail = reject;
		});
		return {thunk: () => promise, finish: (scene: PackScene) => finish(scene), fail: (reason: Error) => fail(reason)};
	}

	test("does not draw over it", async () => {
		const {component, view} = createComponent();
		const old = slow();
		const update = component.updateScene(old.thunk);
		await component.updateScene(answer({bin}));

		old.finish({bin, items});
		await update;

		expect(view.show).not.toHaveBeenCalled();
	});

	test("does not clear it when it fails", async () => {
		const {component, view} = createComponent();
		const old = slow();
		const update = component.updateScene(old.thunk);
		await component.updateScene(answer({bin}));

		old.fail(new Error("failed"));
		await update;

		expect(view.clear).not.toHaveBeenCalled();
	});
});

describe("revealing the view", () => {
	function atWidth(stacked: boolean, reducedMotion = false) {
		window.matchMedia = jest.fn((query: string) => ({
			matches: query.includes("reduce") ? reducedMotion : stacked
		})) as unknown as typeof window.matchMedia;
	}

	test.each([
		["stacked", true, 1],
		["side by side", false, 0]
	])("%s scrolls %s", async (_name, stacked, times) => {
		const {component, figure} = createComponent();
		figure.scrollIntoView = jest.fn();
		atWidth(stacked);

		await component.updateScene(answer({bin, items, reveal: true}));

		expect(figure.scrollIntoView).toHaveBeenCalledTimes(times);
	});

	test("a scene that does not ask stays put", async () => {
		const {component, figure} = createComponent();
		figure.scrollIntoView = jest.fn();
		atWidth(true);

		await component.updateScene(answer({bin, items}));

		expect(figure.scrollIntoView).not.toHaveBeenCalled();
	});

	test("a reader who asks for less motion gets no smooth scroll", async () => {
		const {component, figure} = createComponent();
		figure.scrollIntoView = jest.fn();
		atWidth(true, true);

		await component.updateScene(answer({bin, items, reveal: true}));

		expect(figure.scrollIntoView).toHaveBeenCalledWith({behavior: "auto", block: "start"});
	});
});

describe("a failed update", () => {
	const failure = new Error("failed");

	test("reports the error", async () => {
		const {component, $dispatch, $logger} = createComponent();

		await component.updateScene(() => Promise.reject(failure));

		expect($logger.error).toHaveBeenCalledWith("[Binacle] Error while updating scene", failure);
		expect($dispatch).toHaveBeenCalledWith("error-occurred", ["Error while updating packing visualizer. Please try again later."]);
	});

	test("clears the view, or the loader would run on", async () => {
		const {component, view} = createComponent();

		await component.updateScene(() => Promise.reject(failure));

		expect(view.clear).toHaveBeenCalledTimes(1);
	});

	test("forgets the bin", async () => {
		const {component, view} = createComponent();
		await component.updateScene(answer({bin}));
		await component.updateScene(() => Promise.reject(failure));

		await component.updateScene(answer({bin}));

		expect(view.wait).toHaveBeenLastCalledWith(null);
	});
});

describe("highlighting an item type", () => {
	test("passes on to the view", () => {
		const {component, view} = createComponent();

		component.highlight("Shoebox");

		expect(view.highlight).toHaveBeenCalledWith("Shoebox");
	});
});
