import * as fs from "node:fs";
import * as path from "node:path";
import {PRINTS} from "@binacle/design-system/components/pack-view/prints";
import {BOXES, ITEM_TYPES, PRESET} from "../../src/everydayItems/itemTypes";

const presets = path.join(__dirname, "../../../../../api/src/Binacle.Net/Config_Files/Presets.json");

interface PresetBin {
	ID: string;
	Length: number;
	Width: number;
	Height: number;
}

describe("the boxes", () => {
	test("are the API's preset, by id and size", () => {
		const bins: PresetBin[] = JSON.parse(fs.readFileSync(presets, "utf8")).PresetOptions.Presets[PRESET].Bins;

		expect(BOXES).toEqual(bins.map(bin => ({name: bin.ID, length: bin.Length, width: bin.Width, height: bin.Height})));
	});
});

describe("the item types", () => {
	test("each has a picture under its id", () => {
		expect(ITEM_TYPES.map(itemType => itemType.id).sort()).toEqual(Object.keys(PRINTS).sort());
	});
});
