import * as fs from "node:fs";
import * as path from "node:path";
import {
	CATEGORIES,
	faceColors,
	isInColor,
	oklchToHex,
	parseOklch,
	printColor,
	RAMP_STEPS,
	rampStep
} from "../../../components/pack-view/colors";

const root = path.join(__dirname, "../../..");
const read = (file: string) => fs.readFileSync(path.join(root, file), "utf8");

function sassNumber(source: string, name: string): number {
	const match = source.match(new RegExp(`\\$${name}:\\s*(\\d+);`));
	if (!match) {
		throw new Error(`No $${name} found`);
	}
	return Number(match[1]);
}

function tokenNumbers(source: string, prefix: string): number[] {
	return [...source.matchAll(new RegExp(`--${prefix}-(\\d+):`, "g"))].map(match => Number(match[1]));
}

const oneTo = (count: number) => Array.from({length: count}, (_, index) => index + 1);

describe("parseOklch", () => {
	test("reads the three numbers of a token", () => {
		expect(parseOklch("oklch(0.88 0.08 247.2)")).toEqual([0.88, 0.08, 247.2]);
	});

	// An unset token reads as an empty string. It has to say so, not fail later inside three.js.
	test("throws on a value that is not oklch", () => {
		expect(() => parseOklch("")).toThrow("Expected an oklch() value");
	});
});

describe("oklchToHex", () => {
	test.each([
		[1, 0, 0, "#ffffff"],
		[0, 0, 0, "#000000"],
		[0.5999, 0, 0, "#808080"],
		[0.62796, 0.25768, 29.234, "#ff0000"],
		[0.45201, 0.31321, 264.052, "#0000ff"]
	])("oklch(%d %d %d) is %s", (l, c, h, hex) => {
		expect(oklchToHex(l, c, h)).toBe(hex);
	});

	test("clamps a colour outside sRGB instead of wrapping", () => {
		expect(oklchToHex(1.5, 0, 0)).toBe("#ffffff");
		expect(oklchToHex(-0.1, 0, 0)).toBe("#000000");
	});
});

describe("faceColors", () => {
	const token = "oklch(0.6 0.1 250)";

	// BoxGeometry's groups are +x, -x, +y, -y, +z, -z: the right pair, the top, the underside, the left pair.
	test("is in BoxGeometry's group order", () => {
		const right = oklchToHex(0.51, 0.1, 250);
		const top = oklchToHex(0.68, 0.1, 250);
		const under = oklchToHex(0.44, 0.1, 250);
		const left = oklchToHex(0.6, 0.1, 250);

		expect(faceColors(token)).toEqual([right, right, top, under, left, left]);
	});

	test("does not lift the top past white", () => {
		const [, , top] = faceColors("oklch(0.97 0 0)");

		expect(top).toBe("#ffffff");
	});
});

describe("printColor", () => {
	test("is the token, darker", () => {
		expect(printColor("oklch(0.62 0.095 250)")).toBe(oklchToHex(0.32, 0.095, 250));
	});
});

describe("rampStep", () => {
	test("a single item takes the first step", () => {
		expect(rampStep(0, 1)).toBe(0);
	});

	test("runs from the first step to the last whatever the count", () => {
		expect(rampStep(0, 13)).toBe(0);
		expect(rampStep(12, 13)).toBe(5);
		expect(rampStep(0, 2)).toBe(0);
		expect(rampStep(1, 2)).toBe(5);
	});

	test("spreads the steps evenly", () => {
		const steps = Array.from({length: 11}, (_, index) => rampStep(index, 11));

		expect(steps).toEqual([0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5]);
	});
});

describe("isInColor", () => {
	test("every item is in colour at rest", () => {
		expect(isInColor(0, undefined, 3, 3, null)).toBe(true);
		expect(isInColor(2, undefined, 3, 3, null)).toBe(true);
	});

	test("only the last item placed is in colour while stepping", () => {
		expect(isInColor(0, undefined, 2, 3, null)).toBe(false);
		expect(isInColor(1, undefined, 2, 3, null)).toBe(true);
	});

	test("a lit item type greys every other item type, at rest too", () => {
		expect(isInColor(0, "Shoebox", 3, 3, "Shoebox")).toBe(true);
		expect(isInColor(1, "Cereal box", 3, 3, "Shoebox")).toBe(false);
		expect(isInColor(2, undefined, 3, 3, "Shoebox")).toBe(false);
	});
});

// The tokens, the Sass loops and the view each hold how many there are. A colour missing from one gives an item
// no colour, with no error.
describe("the colour counts", () => {
	const counts = read("abstracts/_item-colors.scss");
	const [light, dark] = read("base/_tokens.scss").split("@mixin -dark");

	test.each([
		["category", "categories", CATEGORIES],
		["item", "ramp-steps", RAMP_STEPS],
	])("--%s- tokens, $%s and the view agree", (prefix, variable, count) => {
		expect(sassNumber(counts, variable)).toBe(count);
		expect(tokenNumbers(light, prefix)).toEqual(oneTo(count));
		expect(tokenNumbers(dark, prefix)).toEqual(oneTo(count));
	});
});
