import type {Dimensions} from "@binacle/design-system";

// The API's built-in preset. Its bins are the three boxes, and each name is the bin's id in it.
export const PRESET = "sample";

export interface BoxSize extends Dimensions {
	name: string;
}

export const BOXES: BoxSize[] = [
	{name: "Small", length: 62, width: 45, height: 8},
	{name: "Medium", length: 62, width: 45, height: 17},
	{name: "Large", length: 62, width: 45, height: 36},
];

// The id is the item's id in the request, so the answer names it back, and the key of its picture. The name is
// only shown. `category` is its --category- colour.
export interface ItemType extends Dimensions {
	id: string;
	name: string;
	category: number;
	openingCount: number;
}

export const ITEM_TYPES: ItemType[] = [
	{id: "kettle", name: "Boxed kettle", category: 1, openingCount: 1, length: 24, width: 20, height: 28},
	{id: "shoe", name: "Shoebox", category: 2, openingCount: 1, length: 33, width: 22, height: 13},
	{id: "cereal", name: "Cereal box", category: 3, openingCount: 2, length: 31, width: 20, height: 6},
	{id: "book", name: "Hardback book", category: 4, openingCount: 2, length: 24, width: 16, height: 4},
	{id: "mug", name: "Mug box", category: 5, openingCount: 2, length: 12, width: 12, height: 11},
	{id: "phone", name: "Phone box", category: 6, openingCount: 3, length: 17, width: 9, height: 5},
];
