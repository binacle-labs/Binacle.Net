import {Bin, Item} from "../viewModels";
import {getRandomInt} from "./getRandomInt";
import {sampleData} from "./sampleData";

export interface Sample {
	bins: Bin[];
	items: Item[];
}

// 30 is the floor because an item side is half a bin side and still has to clear 8.
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

// The bin everything is sized against: the largest by volume, so the set always fits at least one candidate
// and the smaller ones are the interesting result. That comparison is what the page is for.
export function largestBin(bins: Bin[]) {
	return bins.reduce((largest, bin) =>
		bin.length * bin.width * bin.height > largest.length * largest.width * largest.height ? bin : largest
	);
}

// Half the matching bin side, floored at 8. Eight of them fit before any packing thought, so nothing rolled
// here can be an item the bin will not hold.
function randomSide(binSide: number) {
	return getRandomInt(minItemSide, Math.max(minItemSide, Math.floor(binSide / 2)));
}

export function randomItemFor(bin: Bin, quantity: number) {
	return new Item(randomSide(bin.length), randomSide(bin.width), randomSide(bin.height), quantity);
}

export const sampleCount = sampleData.length;

// A fresh Bin and Item every call. The demo edits what it is handed, and the set has to survive that.
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
