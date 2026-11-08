import {largestPair, pathSegments, printPositions} from "../../../components/pack-view/printPositions";
import {PRINTS} from "../../../components/pack-view/prints";

function points(positions: number[]): number[][] {
	const all: number[][] = [];
	for (let index = 0; index < positions.length; index += 3) {
		all.push(positions.slice(index, index + 3));
	}
	return all;
}

describe("pathSegments", () => {
	test("a line is one segment", () => {
		expect(pathSegments("M1 2 L3 4")).toEqual([[[1, 2], [3, 4]]]);
	});

	test("Z closes back to the start", () => {
		const segments = pathSegments("M0 0 L2 0 L2 2 Z");

		expect(segments).toHaveLength(3);
		expect(segments[2]).toEqual([[2, 2], [0, 0]]);
	});

	test("a curve is flattened and ends on its end point", () => {
		const segments = pathSegments("M0 0 C0 1 1 1 1 0");

		expect(segments).toHaveLength(8);
		expect(segments[0][0]).toEqual([0, 0]);
		expect(segments[7][1]).toEqual([1, 0]);
	});

	test("every path in the prints file reads", () => {
		for (const print of Object.values(PRINTS)) {
			for (const path of print.paths) {
				const flat = pathSegments(path).reduce<number[]>((all, [from, to]) => all.concat(from, to), []);

				expect(flat.length).toBeGreaterThan(0);
				expect(flat.every(Number.isFinite)).toBe(true);
			}
		}
	});
});

describe("largestPair", () => {
	const face = (width: number, height: number) =>
		({center: [0, 0, 0], right: [1, 0, 0], up: [0, 1, 0], normal: [0, 0, 1], width, height} as Parameters<typeof largestPair>[0][number]);

	test("picks the opposite pair with the largest area", () => {
		const faces = [face(1, 1), face(1, 1), face(5, 5), face(5, 5), face(2, 2), face(2, 2)];

		expect(largestPair(faces)).toEqual([faces[2], faces[3]]);
	});
});

describe("printPositions", () => {
	test("an item type with no picture has nothing to draw", () => {
		expect(printPositions("bicycle", 10, 10, 10)).toBeNull();
	});

	test("every Everyday Items item type has a picture", () => {
		const itemTypes = Object.keys(PRINTS).sort();

		expect(itemTypes).toEqual(["book", "cereal", "kettle", "mug", "phone", "shoe"]);
		for (const itemType of itemTypes) {
			expect(printPositions(itemType, 10, 10, 10)).not.toBeNull();
		}
	});

	// A wide flat item: the top and the underside are the largest faces.
	test("lies on the largest pair of faces, just off them", () => {
		const positions = printPositions("kettle", 20, 2, 10)!;
		const heights = points(positions).map(([, y]) => Math.abs(y));

		expect(heights.every(height => Math.abs(height - 1.02) < 1e-9)).toBe(true);
	});

	test("stays inside the face", () => {
		const positions = printPositions("kettle", 20, 2, 10)!;

		for (const [x, , z] of points(positions)) {
			expect(Math.abs(x)).toBeLessThan(10);
			expect(Math.abs(z)).toBeLessThan(5);
		}
	});
});
