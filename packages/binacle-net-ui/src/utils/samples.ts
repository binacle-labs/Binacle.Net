import {Bin, Item} from "../viewModels";
import {getRandomInt} from "./getRandomInt";
import {sampleData} from "./sampleData";

export interface Sample {
	bins: Bin[];
	items: Item[];
}

// 30 is the floor: an item side is half a bin side and still has to clear 8.
const minBinSide = 30;
const maxBinSide = 60;
const minItemSide = 8;

export function randomBin() {
	return new Bin(
		getRandomInt(minBinSide, maxBinSide),
		getRandomInt(minBinSide, maxBinSide),
		getRandomInt(minBinSide, maxBinSide)
	);
}

// Items are sized against this bin, so the set always fits at least one candidate.
// Seeded with bins[0] after the guard. A seedless reduce throws its own message on an empty array, and
// this is exported, so a caller outside the demo gets told what it did wrong.
export function largestBin(bins: Bin[]) {
	if (bins.length === 0) {
		throw new Error("largestBin needs at least one bin");
	}

	return bins.reduce(
		(largest, bin) =>
			bin.length * bin.width * bin.height > largest.length * largest.width * largest.height ? bin : largest,
		bins[0]
	);
}

// Eight of these fit the bin before any packing thought, so nothing rolled here is an item it will not hold.
function randomSide(binSide: number) {
	return getRandomInt(minItemSide, Math.max(minItemSide, Math.floor(binSide / 2)));
}

export function randomItemFor(bin: Bin, quantity: number) {
	return new Item(randomSide(bin.length), randomSide(bin.width), randomSide(bin.height), quantity);
}

export const sampleCount = sampleData.length;

// A fresh Bin and Item every call - the demo edits what it is handed.
export function sampleAt(index: number): Sample {
	const data = sampleData[index];
	return {
		bins: data.bins.map(([length, width, height]) => new Bin(length, width, height)),
		items: data.items.map(([length, width, height, quantity]) => new Item(length, width, height, quantity))
	};
}

// A step of at least one, so it can never hand back the sample already on screen.
export function nextSampleIndex(current: number): number {
	return (current + getRandomInt(1, sampleCount - 1)) % sampleCount;
}
