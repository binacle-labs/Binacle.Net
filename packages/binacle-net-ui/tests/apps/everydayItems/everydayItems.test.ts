import type {PackBinResponse} from "@binacle/api-client";

import {Logger} from "../../../src/components/logger/logger";
import type {PackScene} from "../../../src/components/packView/packViewComponent";
import {boxAsked, everydayItemsApp, sizeText} from "../../../src/apps/everydayItems/everydayItems";
import {BOXES, KINDS} from "../../../src/apps/everydayItems/kinds";

interface Dispatched {
	name: string;
	detail: any;
}

type SceneThunk = () => Promise<PackScene | null>;

type EverydayItems = ReturnType<typeof everydayItemsApp> & {
	$dispatch: (event: string, detail?: any) => void;
	$logger: Logger;
	$watch: (key: string, callback: () => void) => void;
	$nextTick: (callback: () => void) => void;
};

// Alpine's $watch is deep. The harness has plain objects, so it wraps the watched one to get the same reach.
function deepWatched<T extends object>(target: T, onChange: () => void): T {
	return new Proxy(target, {
		get(obj, key) {
			const value = Reflect.get(obj, key);
			return value && typeof value === "object" ? deepWatched(value, onChange) : value;
		},
		set(obj, key, value) {
			const written = Reflect.set(obj, key, value);
			onChange();
			return written;
		},
	});
}

function createApp() {
	const dispatched: Dispatched[] = [];
	const ticks: (() => void)[] = [];
	const logger = {info: jest.fn(), log: jest.fn(), warn: jest.fn(), error: jest.fn()};
	const app = everydayItemsApp({baseUrl: "https://api.example.com"}) as EverydayItems;
	app.$dispatch = (name: string, detail?: any) => {dispatched.push({name, detail});};
	app.$nextTick = (callback: () => void) => {ticks.push(callback);};
	app.$logger = logger as unknown as Logger;
	app.$watch = (key: string, callback: () => void) => {
		(app as any)[key] = deepWatched((app as any)[key], callback);
	};
	return {app, dispatched, ticks, logger};
}

function stubResponse(status: number, body?: unknown): Response {
	return {
		ok: status >= 200 && status < 300,
		status,
		text: () => Promise.resolve(body === undefined ? "" : JSON.stringify(body)),
	} as unknown as Response;
}

function mockFetch(response: Response) {
	const fetchMock = jest.fn().mockResolvedValue(response);
	globalThis.fetch = fetchMock as unknown as typeof fetch;
	return fetchMock;
}

function packed(overrides: Partial<PackBinResponse> = {}): PackBinResponse {
	return {
		status: "FullyPacked",
		bin: {id: "Large", length: 62, width: 45, height: 36},
		algorithmUsed: "BFD",
		packedItems: [
			{id: "Boxed kettle", length: 24, width: 20, height: 28, x: 0, y: 0, z: 0},
			{id: "Phone box", length: 17, width: 9, height: 5, x: 24, y: 0, z: 0},
		],
		unpackedItems: null,
		packedItemsVolumePercentage: 100,
		packedBinVolumePercentage: 14.5,
		viPaqData: null,
		...overrides,
	};
}

function lastScene(dispatched: Dispatched[]): Promise<PackScene | null> {
	const scenes = dispatched.filter(x => x.name === "update-scene");
	return (scenes[scenes.length - 1].detail as SceneThunk)();
}

function only(app: EverydayItems, counts: Record<number, number>) {
	app.model.counts = counts;
}

beforeEach(() => {
	window.history.replaceState(null, "", "/items/");
});

describe("the box asked for in the link", () => {
	test("a box's name picks it", () => {
		expect(boxAsked("?box=Small")).toBe("Small");
	});

	test("a name that is not a box is ignored", () => {
		expect(boxAsked("?box=Huge")).toBeNull();
		expect(boxAsked("")).toBeNull();
	});

	test("init opens on the box in the link", () => {
		window.history.replaceState(null, "", "/items/?box=Medium");
		const {app} = createApp();

		app.init();

		expect(app.model.box).toBe("Medium");
	});

	test("init opens on the large box without one", () => {
		const {app} = createApp();

		app.init();

		expect(app.model.box).toBe("Large");
	});
});

describe("opening the page", () => {
	test("packs once the view is listening", async () => {
		const fetchMock = mockFetch(stubResponse(200, packed()));
		const {app, ticks} = createApp();

		app.init();
		expect(fetchMock).not.toHaveBeenCalled();
		ticks.forEach(tick => tick());
		await app.packScene(false);

		expect(fetchMock).toHaveBeenCalled();
	});

	test("does not take the page to the view", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app, dispatched} = createApp();

		await app.pack(false);

		expect((await lastScene(dispatched))?.reveal).toBe(false);
	});
});

describe("the steppers", () => {
	test("add and take away one", () => {
		const {app} = createApp();
		const mug = KINDS[4];

		app.changeCount(mug, 1);
		expect(app.countOf(mug)).toBe(3);
		app.changeCount(mug, -1);
		expect(app.countOf(mug)).toBe(2);
	});

	test("stop at nought and at nine", () => {
		const {app} = createApp();
		const kettle = KINDS[0];

		app.changeCount(kettle, -1);
		expect(app.canRemove(kettle)).toBe(false);
		app.changeCount(kettle, -1);
		expect(app.countOf(kettle)).toBe(0);

		for (let i = 0; i < 12; i++) {
			app.changeCount(kettle, 1);
		}
		expect(app.countOf(kettle)).toBe(9);
		expect(app.canAdd(kettle)).toBe(false);
	});

	test("clear all leaves nothing", () => {
		const {app} = createApp();

		app.clearAll();

		expect(app.totalCount()).toBe(0);
	});
});

describe("the request", () => {
	test("goes to the large box of the sample preset", async () => {
		const fetchMock = mockFetch(stubResponse(200, packed()));
		const {app} = createApp();

		await app.onSubmit();

		expect(fetchMock.mock.calls[0][0]).toBe("https://api.example.com/api/v4/pack/bin/sample/Large");
	});

	test("asks for Best Fit Decreasing, with each kind once by its name", async () => {
		const fetchMock = mockFetch(stubResponse(200, packed()));
		const {app} = createApp();
		only(app, {1: 2, 6: 1});

		await app.onSubmit();

		expect(JSON.parse(fetchMock.mock.calls[0][1].body)).toEqual({
			parameters: {algorithm: "BFD"},
			items: [
				{id: "Boxed kettle", quantity: 2, length: 24, width: 20, height: 28},
				{id: "Phone box", quantity: 1, length: 17, width: 9, height: 5},
			],
		});
	});

	test("with nothing in it is not sent, and the status says why", async () => {
		const fetchMock = mockFetch(stubResponse(200, packed()));
		const {app} = createApp();
		app.clearAll();

		await app.onSubmit();

		expect(fetchMock).not.toHaveBeenCalled();
		expect(app.status).toBe("Add at least one item.");
		expect(app.statusIsError).toBe(true);
	});

	test("says Packing while it is out", () => {
		mockFetch(stubResponse(200, packed()));
		const {app} = createApp();

		void app.onSubmit();

		expect(app.status).toBe("Packing...");
		expect(app.submitting).toBe(true);
	});
});

describe("the answer", () => {
	test("draws the items coloured by kind, named by the kind list", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app, dispatched} = createApp();

		await app.onSubmit();
		const scene = await lastScene(dispatched);

		expect(scene?.bin).toEqual({id: "Large", length: 62, width: 45, height: 36});
		expect(scene?.items?.map(item => item.kind)).toEqual([1, 6]);
		expect(scene?.items?.[0]).not.toHaveProperty("id");
		expect(scene?.kindNames).toEqual(KINDS.map(kind => kind.name));
		expect(scene?.reveal).toBe(true);
	});

	test("when everything fits, says so", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app} = createApp();

		await app.onSubmit();

		expect(app.status).toBe("Everything fits in the large box.");
	});

	test("when some do not fit, counts them", async () => {
		mockFetch(stubResponse(200, packed({
			status: "PartiallyPacked",
			unpackedItems: [{id: "Boxed kettle", quantity: 1}],
		})));
		const {app} = createApp();
		only(app, {1: 2, 6: 1});

		await app.onSubmit();

		expect(app.status).toBe("2 of 3 items fit. 1 did not.");
		expect(app.kindResultText(KINDS[0])).toBe("1 of 2 packed - 1 did not fit");
		expect(app.unfitCountOf(KINDS[0])).toBe(1);
		expect(app.kindResultText(KINDS[5])).toBe("1 of 1 packed");
	});

	test("lists only the kinds asked for", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app} = createApp();
		only(app, {1: 2, 6: 1});

		await app.onSubmit();

		expect(app.resultKinds().map(kind => kind.name)).toEqual(["Boxed kettle", "Phone box"]);
	});

	test("the summary row reads the box, its size, the count and the fill", async () => {
		const result = packed();
		mockFetch(stubResponse(200, result));
		const {app} = createApp();

		await app.onSubmit();

		expect(app.resultTitle(result)).toBe("Large box");
		expect(app.resultBinSizeText(result)).toBe("62 x 45 x 36");
		expect(app.resultItemCountText(result)).toBe("2 items");
		expect(app.resultBinFillText(result)).toBe("14.50%");
	});

	test("an edit while it is out drops it, and the view keeps the box", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app, dispatched} = createApp();
		app.init();

		const submit = app.onSubmit();
		app.model.box = "Small";
		await submit;
		const scene = await (dispatched.filter(x => x.name === "update-scene")[0].detail as SceneThunk)();

		expect(app.result).toBeNull();
		expect(scene).toEqual({bin: {length: 62, width: 45, height: 8}});
	});
});

describe("an edit", () => {
	test("drops the answer and shows the box alone", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app, dispatched} = createApp();
		app.init();
		await app.onSubmit();

		app.model.box = "Medium";

		expect(app.result).toBeNull();
		expect(app.status).toBe("");
		expect(await lastScene(dispatched)).toEqual({bin: {length: 62, width: 45, height: 17}});
	});
});

describe("a failure", () => {
	test("opens the errors dialog with the API's words", async () => {
		mockFetch(stubResponse(429));
		const {app, dispatched} = createApp();

		await app.onSubmit();

		const error = dispatched.find(x => x.name === "error-occurred")!.detail;
		expect(error.title).toBe("Error: Too Many Requests");
		expect(app.status).toBe("");
		expect(app.submitting).toBe(false);
	});

	test("that throws is logged and shown", async () => {
		globalThis.fetch = jest.fn().mockRejectedValue(new Error("offline")) as unknown as typeof fetch;
		const {app, dispatched, logger} = createApp();

		await app.onSubmit();

		expect(logger.error).toHaveBeenCalled();
		expect(dispatched.find(x => x.name === "error-occurred")!.detail.errors).toEqual(["offline"]);
	});
});

describe("pressing a kind in the answer", () => {
	test("shows that kind alone, and a second press shows them all", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app, dispatched} = createApp();
		await app.onSubmit();

		app.toggleKind(KINDS[0]);
		expect(app.isLit(KINDS[0])).toBe(true);
		app.toggleKind(KINDS[0]);

		expect(dispatched.filter(x => x.name === "highlight-kind").map(x => x.detail)).toEqual([1, null]);
	});

	test("a new answer starts with none pressed", async () => {
		mockFetch(stubResponse(200, packed()));
		const {app} = createApp();
		await app.onSubmit();
		app.toggleKind(KINDS[0]);

		await app.onSubmit();

		expect(app.lit).toBeNull();
	});
});

describe("the words", () => {
	test("a size reads length by width by height", () => {
		expect(sizeText(BOXES[0])).toBe("62 x 45 x 8");
	});

	test("an item's size carries its unit", () => {
		const {app} = createApp();

		expect(app.kindSizeText(KINDS[0])).toBe("24 x 20 x 28 cm");
	});
});
