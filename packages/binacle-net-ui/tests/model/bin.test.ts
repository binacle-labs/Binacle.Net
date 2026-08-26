import Bin from "../../src/viewModels/bin";
import Box from "../../src/viewModels/box";

test("a bin is a box", () => {
	const bin = new Bin(10, 20, 30);

	const isBox = bin instanceof Box;

	expect(isBox).toBe(true);
});

test("a bin carries the box id", () => {
	const bin = new Bin(10, 20, 30);

	const id = bin.id;

	expect(id).toBe("10x20x30");
});

test("a bin carries the box validation", () => {
	const bin = new Bin(10, 20, 0);

	const errors = bin.allErrorMessages;

	expect(errors).toEqual(["Height must be between 1 and 65535"]);
});

test("a copy of a bin keeps the footprint", () => {
	const bin = new Bin(10, 20, 30);

	const copy = Bin.copyOf([bin], bin);

	expect([copy.length, copy.width, copy.height]).toEqual([10, 20, 30]);
});

test("a copy of a bin does not take its id", () => {
	const bin = new Bin(10, 20, 30);

	const copy = Bin.copyOf([bin], bin);

	expect(copy.id).toBe("10x20x30 (1)");
});

test("a second copy takes the next number", () => {
	const bin = new Bin(10, 20, 30);
	const first = Bin.copyOf([bin], bin);

	const second = Bin.copyOf([bin, first], bin);

	expect(second.id).toBe("10x20x30 (2)");
});

test("a copy ignores bins with another footprint", () => {
	const bin = new Bin(10, 20, 30);
	const other = new Bin(40, 50, 60, 7);

	const copy = Bin.copyOf([bin, other], bin);

	expect(copy.id).toBe("10x20x30 (1)");
});
