import * as fs from "fs";
import * as path from "path";
import {parseDimensions, parseQuantity} from "binacle-compact-notation";

// Turns shared/data/demo-samples into the demo's sample set, as a TypeScript file both webpack hosts import.
// One source file is one sample: its entries are that sample's bins, and every entry repeats the same items.

interface ScenarioEntry {
	Name: string;
	Bin: string;
	Metrics: string;
	Result: Record<string, string>;
	Items: string[];
}

interface Sample {
	name: string;
	bins: [number, number, number][];
	items: [number, number, number, number][];
}

const sourceDir = path.resolve(__dirname, "..", "..", "..", "shared", "data", "demo-samples");
const outputPath = path.resolve(__dirname, "..", "src", "utils", "sampleData.ts");

// "LxWxH [Q]". parseItem in the notation package is the placed shape, which carries coordinates instead.
function parseItem(compact: string): [number, number, number, number] {
	const bracket = compact.indexOf("[");
	if (bracket < 0) {
		throw new Error(`Item '${compact}' carries no '[Q]' quantity.`);
	}
	const {length, width, height} = parseDimensions(compact.slice(0, bracket));
	return [length, width, height, parseQuantity(compact.slice(bracket))];
}

function parseBin(compact: string): [number, number, number] {
	const {length, width, height} = parseDimensions(compact);
	return [length, width, height];
}

function readSample(file: string): Sample {
	const entries = JSON.parse(fs.readFileSync(path.join(sourceDir, file), "utf8")) as ScenarioEntry[];
	if (entries.length < 1) {
		throw new Error(`${file} holds no entries.`);
	}

	// A file whose entries disagree is a corrupt source. Taking the first list would hide it.
	const items = entries[0].Items;
	for (const entry of entries) {
		if (entry.Items.join(" ") !== items.join(" ")) {
			throw new Error(
				`${file} disagrees on its items: ${entry.Name} has [${entry.Items}], ${entries[0].Name} has [${items}].`
			);
		}
	}

	return {
		name: path.basename(file, ".json"),
		bins: entries.map(entry => parseBin(entry.Bin)),
		items: items.map(parseItem)
	};
}

function render(samples: Sample[]): string {
	const lines = [
		"// Generated from shared/data/demo-samples by `just regen demo-samples`. Do not edit.",
		"",
		"// A bin is [length, width, height], an item [length, width, height, quantity].",
		"export interface SampleData {",
		"\tname: string;",
		"\tbins: [number, number, number][];",
		"\titems: [number, number, number, number][];",
		"}",
		"",
		"export const sampleData: SampleData[] = ["
	];

	for (const sample of samples) {
		lines.push("\t{");
		lines.push(`\t\tname: ${JSON.stringify(sample.name)},`);
		lines.push(`\t\tbins: [${sample.bins.map(bin => `[${bin.join(", ")}]`).join(", ")}],`);
		lines.push(`\t\titems: [${sample.items.map(item => `[${item.join(", ")}]`).join(", ")}]`);
		lines.push("\t},");
	}

	lines.push("];", "");
	return lines.join("\n");
}

function main(): void {
	// The file name carries the order, and the first one is what the demo opens on.
	const files = fs.readdirSync(sourceDir).filter(file => file.endsWith(".json")).sort();
	if (files.length < 1) {
		throw new Error(`${sourceDir} holds no scenario files.`);
	}
or
	const samples = files.map(readSample);
	fs.writeFileSync(outputPath, render(samples));
	console.log(`Wrote ${samples.length} samples to ${outputPath}, opening on ${samples[0].name}.`);
}

try {
	main();
} catch (error) {
	console.error(error);
	process.exit(1);
}
