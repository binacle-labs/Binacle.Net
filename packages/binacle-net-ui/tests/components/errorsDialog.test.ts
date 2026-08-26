import type {Alpine as AlpineType} from "alpinejs";

import {errorsDialog, errorsDialogPlugin} from "../../src/core/errorsDialog";
import {Error as ErrorViewModel} from "../../src/viewModels";

const defaultTitle = "Something went wrong";

function createDialog() {
	return errorsDialog(defaultTitle);
}

// jsdom 20 has HTMLDialogElement without showModal or close, so the two calls the component makes are
// stubbed onto the element and open is driven through the attribute, which jsdom does reflect.
function createMountedDialog() {
	const root = document.createElement("div");
	const element = document.createElement("dialog") as HTMLDialogElement;
	root.append(element);

	const showModal = jest.fn(() => element.setAttribute("open", ""));
	const close = jest.fn(() => {
		element.removeAttribute("open");
		element.dispatchEvent(new Event("close"));
	});
	Object.assign(element, {showModal, close});

	const dialog = errorsDialog(defaultTitle) as ReturnType<typeof errorsDialog> & {$root: HTMLElement};
	dialog.$root = root;
	dialog.init();

	return {dialog, element, showModal, close};
}

describe("the starting state", () => {
	test("the title is the default", () => {
		const dialog = createDialog();

		const title = dialog.title;

		expect(title).toBe(defaultTitle);
	});

	test("there are no errors", () => {
		const dialog = createDialog();

		const hasErrors = dialog.hasErrors();

		expect(hasErrors).toBe(false);
	});
});

describe("a string array", () => {
	test("becomes the error list", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first", "second"]);

		expect(dialog.errors).toEqual(["first", "second"]);
	});

	test("leaves the title alone", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first"]);

		expect(dialog.title).toBe(defaultTitle);
	});

	test("makes the dialog report errors", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first"]);

		expect(dialog.hasErrors()).toBe(true);
	});

	test("an empty array leaves the dialog with nothing to show", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred([]);

		expect(dialog.hasErrors()).toBe(false);
	});
});

describe("an error view model", () => {
	test("its title replaces the default", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "Error: Bad Request", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.title).toBe("Error: Bad Request");
	});

	test("its errors become the error list", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "Error: Bad Request", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.errors).toEqual(["Bins is required"]);
	});

	test("an empty title leaves the default in place", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.title).toBe(defaultTitle);
	});

	test("an empty error list clears whatever was showing", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred(["stale"]);

		dialog.onErrorOccurred({title: "Error", errors: []} as ErrorViewModel);

		expect(dialog.errors).toEqual([]);
	});
});

describe("closing", () => {
	test("clears the errors", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred({title: "Error: Bad Request", errors: ["Bins is required"]} as ErrorViewModel);

		dialog.closeDialog();

		expect(dialog.errors).toEqual([]);
	});

	test("puts the default title back", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred({title: "Error: Bad Request", errors: ["Bins is required"]} as ErrorViewModel);

		dialog.closeDialog();

		expect(dialog.title).toBe(defaultTitle);
	});
});

describe("the plugin", () => {
	test("registers the factory under its x-data name", () => {
		const registered: Record<string, unknown> = {};
		const alpine = {data: (name: string, factory: unknown) => {registered[name] = factory;}} as unknown as AlpineType;

		errorsDialogPlugin(alpine);

		expect(registered).toEqual({errors_dialog: errorsDialog});
	});
});

describe("opening it as a real dialog", () => {
	test("an error opens it modally", () => {
		const {dialog, showModal} = createMountedDialog();

		dialog.onErrorOccurred(["first"]);

		expect(showModal).toHaveBeenCalledTimes(1);
	});

	test("a second error while it is open does not open it twice", () => {
		const {dialog, showModal} = createMountedDialog();
		dialog.onErrorOccurred(["first"]);

		dialog.onErrorOccurred(["second"]);

		expect(showModal).toHaveBeenCalledTimes(1);
	});

	test("an empty error list leaves it shut", () => {
		const {dialog, showModal} = createMountedDialog();

		dialog.onErrorOccurred([]);

		expect(showModal).not.toHaveBeenCalled();
	});

	test("closing it closes the element", () => {
		const {dialog, close} = createMountedDialog();
		dialog.onErrorOccurred(["first"]);

		dialog.closeDialog();

		expect(close).toHaveBeenCalledTimes(1);
	});

	// Escape closes a native dialog without going through closeDialog, which would leave the errors set.
	test("the element closing on its own clears the errors", () => {
		const {dialog, element} = createMountedDialog();
		dialog.onErrorOccurred(["first"]);

		element.dispatchEvent(new Event("close"));

		expect(dialog.errors).toEqual([]);
	});

	test("closing with nothing open does not call close", () => {
		const {dialog, close} = createMountedDialog();

		dialog.closeDialog();

		expect(close).not.toHaveBeenCalled();
	});
});

describe("with no dialog element in reach", () => {
	test("an error still lands", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first"]);

		expect(dialog.errors).toEqual(["first"]);
	});
});
