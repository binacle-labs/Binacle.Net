import Bin from "./bin";
import Item from "./item";
import {getRandomInt} from "./getRandomInt";
import {sampleData} from "./sampleData";

export interface Sample {
	bins: Bin[];
	items: Item[];
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
