import ErrorCollection from "../../src/packingDemo/errorCollection";

describe("push and hasError", () => {
	test("a field is found by the name it was pushed under", () => {
		const errors = new ErrorCollection();

		errors.push("length", "Length is required");

		expect(errors.hasError("length")).toBe(true);
	});

	test("the name must match exactly, case included", () => {
		const errors = new ErrorCollection();

		errors.push("length", "Length is required");

		expect(errors.hasError("Length")).toBe(false);
	});

	test("two messages on one field share one bucket", () => {
		const errors = new ErrorCollection();

		errors.push("length", "first");
		errors.push("length", "second");

		expect(errors.errorMessages).toEqual(["first", "second"]);
	});

	test("an untouched field is false", () => {
		const errors = new ErrorCollection();

		const result = errors.hasError("width");

		expect(result).toBe(false);
	});
});

describe("errorMessages", () => {
	test("is empty for a new collection", () => {
		const errors = new ErrorCollection();

		const messages = errors.errorMessages;

		expect(messages).toEqual([]);
	});

	test("returns every field's messages in the order the fields were first seen", () => {
		const errors = new ErrorCollection();

		errors.push("width", "width one");
		errors.push("length", "length one");
		errors.push("width", "width two");

		expect(errors.errorMessages).toEqual(["width one", "width two", "length one"]);
	});
});

describe("hasErrors", () => {
	test("is false for a new collection", () => {
		const errors = new ErrorCollection();

		const result = errors.hasErrors();

		expect(result).toBe(false);
	});

	test("is true once any field has a message", () => {
		const errors = new ErrorCollection();

		errors.push("height", "Height must be an integer");

		expect(errors.hasErrors()).toBe(true);
	});
});
