// Known-good ViPaq strings a visitor can load without producing one first. Each is a packed result from
// vipaq/data/packed/demo-samples, and a test serializes that placement and holds the string to it.

export interface SampleData {
	name: string;
	encoded: string;
}

export const sampleData: SampleData[] = [
	{
		name: "Five boxes, packed full",
		encoded: "AAAFAB4UFBQUFAAAAAoKChQAAAoKChQKAAoKChQACgoKChQKCg=="
	},
	{
		name: "Ten boxes, two sizes",
		encoded: "AAAKADwjGRkIGQAAABkIGRkAABkIGQAIABkIGRkIABkIGQAQABkIGRkQABAKGQAYABAKGRkYAAoQGTIAAAoQGTIQAA=="
	},
	{
		name: "Twenty-four cubes",
		encoded: "AAAYACgeGQoKCgAAAAoKCgoAAAoKCgAKAAoKCgAACgoKChQAAAoKCgoKAAoKCgoACgoKCgAUAAoKCgAKCgoKCh4AAAoKChQKAAoKChQACgoKCgoUAAoKCgoKCgoKCgAUCgoKCh4KAAoKCh4ACgoKChQUAAoKChQKCgoKCgoUCgoKCh4UAAoKCh4KCgoKChQUCgoKCh4UCg=="
	},
	{
		name: "Thirteen boxes, mixed sizes",
		encoded: "AAANACMeGRQSEAAAAA8PChQAAA8PChQPAA8PChQACg8PChQPCgoKCAASAAoKCAAAEAoKCAoSAAoKCAASCAoKCAoAEAoKCAoSCAoKCAASEAoKCAoSEA=="
	},
	{
		name: "Eight flat items",
		encoded: "AAAIADIyDBgYBQAAABgYBRgAABgYBQAYABgYBQAABRgYBRgYABgYBRgABRgYBQAYBRgYBRgYBQ=="
	}
];
