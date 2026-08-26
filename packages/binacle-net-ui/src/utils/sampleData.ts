// Generated from shared/data/demo-samples by `just regen demo-samples`. Do not edit.

// A bin is [length, width, height], an item [length, width, height, quantity].
export interface SampleData {
	name: string;
	bins: [number, number, number][];
	items: [number, number, number, number][];
}

export const sampleData: SampleData[] = [
	{
		name: "01-opening-set",
		bins: [[50, 40, 40], [40, 30, 30], [30, 20, 20]],
		items: [[20, 20, 20, 2], [10, 10, 10, 4]]
	},
	{
		name: "02-packs-nowhere",
		bins: [[30, 30, 30]],
		items: [[20, 20, 20, 3]]
	},
	{
		name: "03-three-answers",
		bins: [[60, 40, 20]],
		items: [[12, 10, 15, 5], [18, 10, 12, 4], [8, 15, 12, 4]]
	},
	{
		name: "04-bfd-loses",
		bins: [[60, 35, 25]],
		items: [[16, 25, 10, 4], [25, 8, 25, 6]]
	},
	{
		name: "05-one-of-each",
		bins: [[40, 30, 25], [25, 20, 20]],
		items: [[24, 18, 14, 1], [16, 12, 10, 1]]
	},
	{
		name: "06-long-items",
		bins: [[50, 30, 20], [30, 30, 30]],
		items: [[50, 10, 10, 3], [20, 10, 10, 2]]
	},
	{
		name: "07-tall-items",
		bins: [[20, 20, 60], [40, 40, 30]],
		items: [[8, 8, 55, 4]]
	},
	{
		name: "08-cube-bin",
		bins: [[60, 40, 40], [34, 34, 34]],
		items: [[30, 20, 20, 4], [16, 16, 16, 2]]
	},
	{
		name: "09-bfd-fits-more",
		bins: [[50, 40, 35], [40, 30, 30]],
		items: [[22, 16, 14, 2], [18, 12, 12, 3], [10, 10, 10, 5], [30, 8, 8, 2]]
	},
	{
		name: "10-six-types",
		bins: [[45, 35, 30], [35, 30, 25], [22, 20, 16]],
		items: [[20, 15, 10, 1], [18, 12, 12, 1], [14, 14, 14, 1], [12, 10, 8, 1], [10, 8, 6, 1], [22, 9, 9, 1]]
	},
	{
		name: "11-seven-types",
		bins: [[65, 50, 40], [40, 30, 30], [30, 25, 20]],
		items: [[10, 22, 8, 3], [20, 18, 20, 1], [8, 16, 18, 1], [24, 14, 8, 3], [6, 20, 22, 1], [20, 24, 12, 2], [14, 14, 25, 2]]
	},
	{
		name: "12-middle-bin-wins",
		bins: [[60, 50, 40], [40, 35, 30], [30, 25, 20]],
		items: [[15, 15, 15, 4], [12, 10, 10, 6]]
	},
	{
		name: "13-twenty-four-cubes",
		bins: [[50, 40, 30], [40, 30, 25], [30, 25, 20]],
		items: [[10, 10, 10, 24]]
	},
	{
		name: "14-flat-items",
		bins: [[50, 50, 12], [40, 30, 30], [30, 30, 20]],
		items: [[24, 24, 5, 8]]
	},
	{
		name: "15-same-volume-different-shape",
		bins: [[40, 40, 25], [50, 25, 32], [100, 20, 20]],
		items: [[12, 12, 12, 16]]
	},
	{
		name: "16-only-bfd-fully-packs",
		bins: [[60, 55, 50], [35, 30, 30], [30, 25, 25]],
		items: [[25, 25, 25, 4], [20, 20, 15, 4], [12, 12, 12, 6]]
	},
	{
		name: "17-four-bins-bfd-ahead",
		bins: [[55, 45, 40], [45, 40, 35], [40, 30, 30], [30, 30, 25]],
		items: [[18, 18, 18, 3], [14, 12, 10, 5], [9, 9, 9, 6]]
	},
	{
		name: "18-four-bins-bfd-fully-packs",
		bins: [[50, 40, 40], [45, 35, 30], [35, 30, 25], [25, 25, 20]],
		items: [[20, 20, 12, 2], [15, 12, 10, 8], [9, 9, 9, 10]]
	},
	{
		name: "19-five-bins",
		bins: [[60, 60, 50], [50, 45, 40], [45, 35, 30], [35, 30, 25], [25, 20, 20]],
		items: [[20, 18, 16, 4], [15, 15, 10, 4], [10, 10, 8, 8]]
	},
	{
		name: "20-wfd-wins",
		bins: [[587, 233, 220]],
		items: [[105, 81, 46, 38], [115, 81, 61, 22], [72, 51, 23, 32]]
	},
];
