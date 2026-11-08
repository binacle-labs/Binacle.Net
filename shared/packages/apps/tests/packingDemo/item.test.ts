import Item from "../../src/packingDemo/item";

describe("getDimensions", () => {
	test("adds quantity as a fourth dimension", () => {
		const item = new Item(10, 20, 30, 4);

		const dimensions = item.getDimensions();

		expect(dimensions).toEqual([
			{field: "length", label: "Length", value: 10},
			{field: "width", label: "Width", value: 20},
			{field: "height", label: "Height", value: 30},
			{field: "quantity", label: "Quantity", value: 4},
		]);
	});
});

describe("validation", () => {
	test("a valid item reports no errors", () => {
		const item = new Item(10, 20, 30, 4);

		const errors = item.allErrorMessages;

		expect(errors).toEqual([]);
	});

	test("quantity is checked by the same rules as the sides", () => {
		const item = new Item(10, 20, 30, 0);

		const errors = item.allErrorMessages;

		expect(errors).toEqual(["Quantity must be between 1 and 65535"]);
	});

	test("a fractional quantity is not an integer", () => {
		const item = new Item(10, 20, 30, 1.5);

		const errors = item.allErrorMessages;

		expect(errors).toEqual(["Quantity must be an integer"]);
	});

	test("isFieldValid answers for quantity too", () => {
		const item = new Item(10, 20, 30, 0);

		const valid = item.isFieldValid("quantity");

		expect(valid).toBe(false);
	});
});
