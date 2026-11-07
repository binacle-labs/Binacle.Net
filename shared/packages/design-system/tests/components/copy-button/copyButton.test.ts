import {initCopyButton} from "../../../components/copy-button/copyButton";

const writeText = jest.fn();

function button(): HTMLButtonElement {
	return document.querySelector("button") as HTMLButtonElement;
}

function giveClipboard(): void {
	Object.defineProperty(navigator, "clipboard", {value: {writeText}, configurable: true});
}

async function settle(): Promise<void> {
	await Promise.resolve();
	await Promise.resolve();
}

beforeEach(() => {
	writeText.mockReset();
	writeText.mockResolvedValue(undefined);
	giveClipboard();
});

afterEach(() => {
	Reflect.deleteProperty(navigator, "clipboard");
	jest.useRealTimers();
});

describe("what it copies", () => {
	test("the element it names", () => {
		document.body.innerHTML = '<p id="text"> the words </p><button data-copy="text" hidden></button>';
		initCopyButton();

		button().click();

		expect(writeText).toHaveBeenCalledWith("the words");
	});

	test("left empty, the code in the code block around it", () => {
		document.body.innerHTML = '<div data-code-block><button data-copy hidden></button><pre><code>\nnpm i\n</code></pre></div>';
		initCopyButton();

		button().click();

		expect(writeText).toHaveBeenCalledWith("npm i");
	});

	test("left empty, the code in the command around it", () => {
		document.body.innerHTML = '<div data-command><code>npm i</code><button data-copy hidden></button></div>';
		initCopyButton();

		button().click();

		expect(writeText).toHaveBeenCalledWith("npm i");
	});
});

describe("whether it shows", () => {
	const BLOCK = '<div data-code-block><button data-copy hidden></button><code>npm i</code></div>';

	test("with something to copy and a clipboard, it shows and has a title", () => {
		document.body.innerHTML = BLOCK;

		initCopyButton();

		expect(button().hidden).toBe(false);
		expect(button().title).toBe("Copy");
	});

	test("with no clipboard, it stays hidden", () => {
		Reflect.deleteProperty(navigator, "clipboard");
		document.body.innerHTML = BLOCK;

		initCopyButton();

		expect(button().hidden).toBe(true);
	});

	test("with nothing to copy, it stays hidden", () => {
		document.body.innerHTML = '<button data-copy="missing" hidden></button>';

		initCopyButton();

		expect(button().hidden).toBe(true);
	});
});

describe("the word on the button", () => {
	beforeEach(() => {
		jest.useFakeTimers();
		document.body.innerHTML = '<div data-code-block><button data-copy hidden>Copy</button><code>npm i</code></div>';
		initCopyButton();
	});

	test("says Copied once the text is on the clipboard", async () => {
		button().click();
		await settle();

		expect(button().textContent).toBe("Copied");
		expect(button().classList.contains("is-copied")).toBe(true);
	});

	test("goes back after a moment", async () => {
		button().click();
		await settle();

		jest.advanceTimersByTime(1500);

		expect(button().textContent).toBe("Copy");
		expect(button().classList.contains("is-copied")).toBe(false);
	});

	test("stays until the moment has passed", async () => {
		button().click();
		await settle();

		jest.advanceTimersByTime(1499);

		expect(button().textContent).toBe("Copied");
	});
});
