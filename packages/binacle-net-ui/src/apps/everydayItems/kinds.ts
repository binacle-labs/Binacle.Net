import Dimensions from "../../shared/dimensions";

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

// `kind` is the number of its --kind- colour, and what the view colours the item by.
export interface ItemKind extends Dimensions {
	kind: number;
	name: string;
}

export const KINDS: ItemKind[] = [
	{kind: 1, name: "Boxed kettle", length: 24, width: 20, height: 28},
	{kind: 2, name: "Shoebox", length: 33, width: 22, height: 13},
	{kind: 3, name: "Cereal box", length: 31, width: 20, height: 6},
	{kind: 4, name: "Hardback book", length: 24, width: 16, height: 4},
	{kind: 5, name: "Mug box", length: 12, width: 12, height: 11},
	{kind: 6, name: "Phone box", length: 17, width: 9, height: 5},
];
