import type {Alpine as AlpineType} from "alpinejs";

import {everydayItemsPlugin} from "../src/apps/everydayItems/plugin";
import {packingDemoPlugin} from "../src/apps/packingDemo/plugin";
import {protocolDecoderPlugin} from "../src/apps/protocolDecoder/plugin";

// The aggregate plugins are the package's whole public surface - a host page imports one, calls
// Alpine.plugin, and uses the x-data names in its markup. What is asserted here is that list, because the
// names are what the .cshtml and the Jekyll page write by hand.
function fakeAlpine() {
	const data: string[] = [];
	const directives: string[] = [];
	const magics: string[] = [];

	// Real Alpine hands itself to the callback, which is how the nested plugins register anything.
	const alpine = {
		plugin: (callback: (alpine: AlpineType) => void) => callback(alpine as unknown as AlpineType),
		data: (name: string) => data.push(name),
		directive: (name: string) => directives.push(name),
		magic: (name: string) => magics.push(name),
	};

	return {alpine: alpine as unknown as AlpineType, data, directives, magics};
}

describe("packingDemoPlugin", () => {
	test("registers the three components the packing page uses", () => {
		const {alpine, data} = fakeAlpine();

		packingDemoPlugin(alpine);

		expect(data.sort()).toEqual(["errors_dialog", "pack_view", "packing_demo_app"]);
	});

	test("registers the field naming helpers, which only the form needs", () => {
		const {alpine, directives, magics} = fakeAlpine();

		packingDemoPlugin(alpine);

		expect(directives).toEqual(["field-prefix"]);
		expect(magics.sort()).toEqual(["fieldId", "fieldName", "logger"]);
	});

	test("does not register the decoder", () => {
		const {alpine, data} = fakeAlpine();

		packingDemoPlugin(alpine);

		expect(data).not.toContain("protocol_decoder_app");
	});
});

describe("protocolDecoderPlugin", () => {
	test("registers the three components the vipaq page uses", () => {
		const {alpine, data} = fakeAlpine();

		protocolDecoderPlugin(alpine);

		expect(data.sort()).toEqual(["errors_dialog", "pack_view", "protocol_decoder_app"]);
	});

	// The decoder page has no form, so it carries none of the field machinery.
	test("registers no field helpers", () => {
		const {alpine, directives, magics} = fakeAlpine();

		protocolDecoderPlugin(alpine);

		expect(directives).toEqual([]);
		expect(magics).toEqual(["logger"]);
	});

	test("does not register the packing form", () => {
		const {alpine, data} = fakeAlpine();

		protocolDecoderPlugin(alpine);

		expect(data).not.toContain("packing_demo_app");
	});
});

describe("both plugins", () => {
	// Two pages, one 3D view and one dialog implementation. A host loading both would otherwise get two.
	test("share the 3D view and the error dialog", () => {
		const demo = fakeAlpine();
		const decoder = fakeAlpine();

		packingDemoPlugin(demo.alpine);
		protocolDecoderPlugin(decoder.alpine);

		expect(demo.data).toContain("pack_view");
		expect(decoder.data).toContain("pack_view");
		expect(demo.data).toContain("errors_dialog");
		expect(decoder.data).toContain("errors_dialog");
	});
});

describe("everydayItemsPlugin", () => {
	test("registers the three components the items page uses", () => {
		const {alpine, data} = fakeAlpine();

		everydayItemsPlugin(alpine);

		expect(data.sort()).toEqual(["errors_dialog", "everyday_items_app", "pack_view"]);
	});

	// Its form has no typed fields, so it carries none of the field machinery.
	test("registers no field helpers", () => {
		const {alpine, directives, magics} = fakeAlpine();

		everydayItemsPlugin(alpine);

		expect(directives).toEqual([]);
		expect(magics).toEqual(["logger"]);
	});
});
