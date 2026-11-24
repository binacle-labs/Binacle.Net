import {samplesPanel} from "../../src/protocolDecoder/samplesPanel";

const sample = {name: "Five boxes", encoded: "AAAF"};

interface Dialog {
	showModal: jest.Mock;
	close: jest.Mock;
}

// The component nests in the decoder's scope, so the parent's members arrive through `this`.
function createPanel() {
	const dialog: Dialog = {showModal: jest.fn(), close: jest.fn()};
	const parent = {model: {result: null as string | null}, addResult: jest.fn()};
	const panel = Object.assign(samplesPanel(), parent, {$refs: {panel: dialog}});

	return {panel, dialog, parent};
}

function setClipboard(value: unknown) {
	Object.defineProperty(navigator, "clipboard", {value, configurable: true});
}

afterEach(() => {
	jest.useRealTimers();
	setClipboard(undefined);
});

describe("samplesPanel", () => {
	test("starts with nothing copied", () => {
		expect(createPanel().panel.copied).toBeNull();
	});

	test("show opens the dialog as a modal", () => {
		const {panel, dialog} = createPanel();

		panel.show();

		expect(dialog.showModal).toHaveBeenCalledTimes(1);
	});

	test("hide closes the dialog", () => {
		const {panel, dialog} = createPanel();

		panel.hide();

		expect(dialog.close).toHaveBeenCalledTimes(1);
	});

	test("does not define the parent's members", () => {
		const own = samplesPanel() as unknown as Record<string, unknown>;

		expect(own.model).toBeUndefined();
		expect(own.addResult).toBeUndefined();
	});
});

describe("canCopy", () => {
	test("is true when the clipboard API exists", () => {
		setClipboard({writeText: jest.fn()});

		expect(createPanel().panel.canCopy()).toBe(true);
	});

	test("is false when it does not", () => {
		setClipboard(undefined);

		expect(createPanel().panel.canCopy()).toBe(false);
	});
});

describe("copy", () => {
	test("writes the encoded string, marks the sample as copied, and clears it after 1500 ms", async () => {
		jest.useFakeTimers();
		const writeText = jest.fn().mockResolvedValue(undefined);
		setClipboard({writeText});
		const {panel} = createPanel();

		await panel.copy(sample);

		expect(writeText).toHaveBeenCalledWith(sample.encoded);
		expect(panel.copied).toBe(sample.name);

		jest.advanceTimersByTime(1499);
		expect(panel.copied).toBe(sample.name);

		jest.advanceTimersByTime(1);
		expect(panel.copied).toBeNull();
	});
});

describe("use", () => {
	test("sets the parent's result, closes the drawer and adds the result, in that order", () => {
		const {panel, dialog, parent} = createPanel();
		const order: string[] = [];
		dialog.close.mockImplementation(() => order.push("close"));
		parent.addResult.mockImplementation(() => order.push(`add:${parent.model.result}`));

		panel.use(sample);

		expect(parent.model.result).toBe(sample.encoded);
		expect(order).toEqual(["close", `add:${sample.encoded}`]);
	});
});

describe("fitToText", () => {
	function textarea(scrollHeight: number, clientHeight: number, offsetHeight: number) {
		return {style: {height: "99px"}, scrollHeight, clientHeight, offsetHeight} as unknown as HTMLTextAreaElement;
	}

	test("leaves a short box on its rows", () => {
		const box = textarea(40, 40, 42);

		createPanel().panel.fitToText(box);

		expect(box.style.height).toBe("");
	});

	test("gives a long box its content height plus the border", () => {
		const box = textarea(120, 40, 42);

		createPanel().panel.fitToText(box);

		expect(box.style.height).toBe("122px");
	});
});
