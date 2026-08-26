import {Bin, Item} from "../../src/viewModels";
import {largestBin, nextSampleIndex, randomBin, randomItemFor, sampleAt, sampleCount} from "../../src/utils/samples";
import {sampleData} from "../../src/utils/sampleData";

// The rolled numbers, restated here so a change to samples.ts has to be a deliberate change to the test.
const minBinSide = 30;
const maxBinSide = 60;

// Enough rolls that a one-in-a-thousand break shows up, few enough that the file stays under a second.
const rolls = 5000;

const everyIndex = Array.from({length: sampleCount}, (_, index) => index);

function duplicates(ids: string[]) {
	return ids.filter((id, index) => ids.indexOf(id) !== index);
}

describe("randomBin", () => {
	test("every side is a whole number inside the bin range", () => {
		const bins = Array.from({length: rolls}, () => randomBin());

		const sides = bins.flatMap(bin => [bin.length, bin.width, bin.height]);

		const outOfRange = sides.filter(side => !Number.isInteger(side) || side < minBinSide || side > maxBinSide);
		expect(outOfRange).toEqual([]);
	});
});

describe("largestBin", () => {
	test("picks the bin with the greatest volume", () => {
		const bins = [new Bin(30, 30, 30), new Bin(60, 50, 40), new Bin(40, 40, 40)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[1]);
	});

	test("keeps the first bin when two share the greatest volume", () => {
		const bins = [new Bin(40, 30, 20), new Bin(20, 30, 40)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[0]);
	});

	test("returns the only bin of a single-bin set", () => {
		const bins = [new Bin(31, 32, 33)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[0]);
	});
});

describe("randomItemFor", () => {
	test("no side is more than half the matching bin side", () => {
		const bin = new Bin(60, 45, 31);

		const items = Array.from({length: rolls}, () => randomItemFor(bin, 1));

		const oversized = items.filter(
			item =>
				item.length > Math.floor(bin.length / 2) ||
				item.width > Math.floor(bin.width / 2) ||
				item.height > Math.floor(bin.height / 2)
		);
		expect(oversized).toEqual([]);
	});

	test("keeps the quantity it was given", () => {
		const bin = new Bin(50, 50, 50);

		const item = randomItemFor(bin, 7);

		expect(item.quantity).toBe(7);
	});
});

// The set is generated from shared/data/demo-samples, so a bad file there is a 422 on the demo page and
// nothing else here would catch it.
describe("the sample set", () => {
	test("holds one sample per source file", () => {
		expect(sampleCount).toBe(sampleData.length);
		expect(sampleCount).toBeGreaterThanOrEqual(10);
	});

	// The demo opens on the first, so the order the generator writes has to be the order of the file names.
	test("keeps the source file order", () => {
		const names = sampleData.map(sample => sample.name);

		expect(names).toEqual([...names].sort());
		expect(names[0]).toBe("01-opening-set");
	});

	test("every sample has at least one bin", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const empty = samples.filter(sample => sample.bins.length < 1);

		expect(empty).toEqual([]);
	});

	test("every sample has at least one item", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const empty = samples.filter(sample => sample.items.length < 1);

		expect(empty).toEqual([]);
	});

	test("no sample repeats a bin id", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const repeated = samples.flatMap(sample => duplicates(sample.bins.map(bin => bin.id)));

		expect(repeated).toEqual([]);
	});

	test("no sample repeats an item id", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const repeated = samples.flatMap(sample => duplicates(sample.items.map(item => item.id)));

		expect(repeated).toEqual([]);
	});

	test("every dimension is one the API accepts", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const boxes = samples.flatMap(sample => [...sample.bins, ...sample.items] as (Bin | Item)[]);

		expect(boxes.filter(box => box.hasErrors())).toEqual([]);
	});

	test("every quantity is a whole number of at least one", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const quantities = samples.flatMap(sample => sample.items.map(item => item.quantity));

		expect(quantities.filter(quantity => !Number.isInteger(quantity) || quantity < 1)).toEqual([]);
	});

	test("covers bin counts from one to five", () => {
		const samples = everyIndex.map(index => sampleAt(index));

		const binCounts = new Set(samples.map(sample => sample.bins.length));

		expect([...binCounts].sort()).toEqual(expect.arrayContaining([1, 2, 3, 4, 5]));
	});
});

describe("sampleAt", () => {
	test("sample zero is the same set every call", () => {
		const first = sampleAt(0);
		const second = sampleAt(0);

		expect(second.bins.map(bin => bin.id)).toEqual(first.bins.map(bin => bin.id));
		expect(second.items.map(item => item.id)).toEqual(first.items.map(item => item.id));
	});

	test("hands back a new Bin and Item every call", () => {
		const first = sampleAt(0);
		const second = sampleAt(0);

		expect(second.bins[0]).not.toBe(first.bins[0]);
		expect(second.items[0]).not.toBe(first.items[0]);
	});

	test("sample zero opens on more than one bin", () => {
		const sample = sampleAt(0);

		expect(sample.bins.length).toBeGreaterThan(1);
	});
});

describe("nextSampleIndex", () => {
	test("never returns the index it was given", () => {
		const picks = everyIndex.flatMap(index =>
			Array.from({length: rolls / sampleCount}, () => ({index, next: nextSampleIndex(index)}))
		);

		expect(picks.filter(pick => pick.next === pick.index)).toEqual([]);
	});

	test("stays inside the set", () => {
		const picks = everyIndex.flatMap(index =>
			Array.from({length: rolls / sampleCount}, () => nextSampleIndex(index))
		);

		expect(picks.filter(next => !Number.isInteger(next) || next < 0 || next >= sampleCount)).toEqual([]);
	});

	test("reaches every other sample", () => {
		const picks = new Set(Array.from({length: rolls}, () => nextSampleIndex(0)));

		expect([...picks].sort((a, b) => a - b)).toEqual(everyIndex.slice(1));
	});
});
