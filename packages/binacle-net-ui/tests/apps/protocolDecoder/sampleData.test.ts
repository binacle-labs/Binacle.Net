import * as fs from "node:fs";
import * as path from "node:path";
import {ViPaqSerializer} from "binacle-vipaq";
import {parseDimensions, parseItems} from "binacle-compact-notation";

import {sampleData} from "../../../src/apps/protocolDecoder/sampleData";

// Resolved against this file so it holds whatever the jest working directory is.
const packed = path.join(__dirname, "../../../../../vipaq/data/packed/demo-samples");

interface PackedSample {
	Name: string;
	Bin: string;
	Items: string[];
}

// Each sample is one placed bin out of the generated demo-sample results, by file and sample name.
const sources: [string, string, string][] = [
	["Five boxes, packed full", "01-opening-set.ffd.json", "DemoSample_01_OpeningSet_30x20x20"],
	["Ten boxes, two sizes", "04-bfd-loses.ffd.json", "DemoSample_04_BfdLoses_60x35x25"],
	["Twenty-four cubes", "13-twenty-four-cubes.ffd.json", "DemoSample_13_TwentyFourCubes_40x30x25"],
	["Thirteen boxes, mixed sizes", "19-five-bins.ffd.json", "DemoSample_19_FiveBins_35x30x25"],
	["Eight flat items", "14-flat-items.ffd.json", "DemoSample_14_FlatItems_50x50x12"],
];

function readPacked(fileName: string, sampleName: string): PackedSample {
	const rows = JSON.parse(fs.readFileSync(path.join(packed, fileName), "utf8")) as PackedSample[];
	const row = rows.find(r => r.Name === sampleName);
	if (!row) {
		throw new Error(`no sample named ${sampleName} in ${fileName}`);
	}
	return row;
}

function fromBase64(encoded: string): Uint8Array<ArrayBuffer> {
	return Uint8Array.from(atob(encoded), x => x.charCodeAt(0));
}

function toBase64(bytes: Uint8Array): string {
	return btoa(String.fromCharCode(...bytes));
}

function sample(name: string) {
	const found = sampleData.find(s => s.name === name);
	if (!found) {
		throw new Error(`no sample named ${name}`);
	}
	return found;
}

describe("every sample", () => {
	test("has a source in the packed demo-sample data", () => {
		const named = sources.map(([name]) => name).sort();

		expect(sampleData.map(s => s.name).sort()).toEqual(named);
	});

	test.each(sampleData.map(s => [s.name, s.encoded]))("%s decodes", async (_name, encoded) => {
		const result = await ViPaqSerializer.deserialize(fromBase64(encoded));

		expect(result.items.length).toBeGreaterThan(0);
	});
});

// The packed file holds the placement, not the string, so the proof is serializing the placement.
describe("each sample is its packed result", () => {
	test.each(sources)("%s is %s / %s", async (sampleName, fileName, packedName) => {
		const row = readPacked(fileName, packedName);

		const encoded = toBase64(await ViPaqSerializer.serialize(parseDimensions(row.Bin), parseItems(row.Items)));

		expect(sample(sampleName).encoded).toBe(encoded);
	});

	// A sample that draws well is a full bin. Below this the visitor sees mostly empty space.
	test.each(sources)("%s fills at least three quarters of its bin", (sampleName, fileName, packedName) => {
		const row = readPacked(fileName, packedName);
		const bin = parseDimensions(row.Bin);
		const items = parseItems(row.Items);

		const packedVolume = items.reduce((sum, i) => sum + i.length * i.width * i.height, 0);

		expect(packedVolume / (bin.length * bin.width * bin.height)).toBeGreaterThanOrEqual(0.75);
	});
});
