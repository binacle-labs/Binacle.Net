import {readFileSync} from "fs";
import {join} from "path";

import Ajv, {ValidateFunction} from "ajv";

import {
	HttpValidationProblemDetails,
	PackBinResponse,
	PackCompareResponse,
	PackCustomRequest,
	ProblemDetails,
} from "../src";

// The point of this package. The fixtures below are annotated with our hand-written types, so the compiler
// rejects a fixture our types cannot describe, and ajv rejects one the spec cannot. A field renamed in the
// API reaches spec/v4.json through `just openapi sync-all-copies` and fails here.
const spec = JSON.parse(readFileSync(join(__dirname, "..", "spec", "v4.json"), "utf8"));

// strict: false, because an OpenAPI document carries keywords ajv's strict mode rejects - `openapi`, `paths`
// and `example` among them. validateFormats: false, because int32 and double are OpenAPI formats, not ones
// ajv knows; the enums, the required lists and the types are what this test is checking.
const ajv = new Ajv({strict: false, validateFormats: false});

// The whole document goes in under one name, so a $ref inside components/schemas resolves against it.
ajv.addSchema(spec, "v4");

function validatorFor(schema: string): ValidateFunction {
	const validate = ajv.getSchema(`v4#/components/schemas/${schema}`);
	if (!validate)
		throw new Error(`spec/v4.json has no schema named '${schema}'.`);
	return validate;
}

// Returns one line per mismatch, each naming the field. A bare "invalid" tells the next person nothing.
function mismatches(schema: string, fixture: unknown): string[] {
	const validate = validatorFor(schema);
	if (validate(fixture))
		return [];

	return (validate.errors ?? []).map(error => {
		const field = error.instancePath === "" ? schema : `${schema}${error.instancePath}`;
		return `${field} ${error.message ?? "is invalid"}`;
	});
}

const request: PackCustomRequest = {
	parameters: {algorithm: "Best", includeViPaqData: true},
	bins: [
		{id: "custom_bin_1", length: 10, width: 40, height: 60},
		{id: "custom_bin_2", length: 20, width: 40, height: 60},
	],
	items: [
		{id: "box_1", quantity: 2, length: 2, width: 5, height: 10},
		{id: "box_2", quantity: 1, length: 12, width: 15, height: 10},
	],
};

const packedBin: PackBinResponse = {
	status: "FullyPacked",
	bin: {id: "custom_bin_2", length: 20, width: 40, height: 60},
	algorithmUsed: "FFD",
	packedItems: [
		{id: "box_1", length: 2, width: 5, height: 10, x: 0, y: 0, z: 0},
		{id: "box_1", length: 2, width: 5, height: 10, x: 2, y: 0, z: 0},
	],
	unpackedItems: null,
	packedItemsVolumePercentage: 100,
	packedBinVolumePercentage: 4.6,
	viPaqData: "AQIDBA==",
};

const partiallyPackedBin: PackBinResponse = {
	status: "PartiallyPacked",
	bin: {id: "custom_bin_1", length: 10, width: 40, height: 60},
	algorithmUsed: "BFD",
	packedItems: [{id: "box_1", length: 2, width: 5, height: 10, x: 0, y: 0, z: 0}],
	unpackedItems: [{id: "box_2", quantity: 1}],
	packedItemsVolumePercentage: 5.2,
	packedBinVolumePercentage: 0.4,
	viPaqData: null,
};

const compareResponse: PackCompareResponse = {
	results: [partiallyPackedBin, packedBin],
};

describe("the request type", () => {
	test("matches PackCustomCompareRequest", () => {
		expect(mismatches("PackCustomCompareRequest", request)).toEqual([]);
	});

	// The three custom pack routes share one request shape, which is why adding them later is not a rewrite.
	test("matches PackCustomSmallestBinRequest, which shares the shape", () => {
		expect(mismatches("PackCustomSmallestBinRequest", request)).toEqual([]);
	});

	test("matches PackCustomBestBinRequest, which shares the shape", () => {
		expect(mismatches("PackCustomBestBinRequest", request)).toEqual([]);
	});

	test("matches with the optional parameters left out", () => {
		const minimal: PackCustomRequest = {
			parameters: {algorithm: "FFD"},
			bins: [{id: "bin", length: 10, width: 10, height: 10}],
			items: [{id: "box", quantity: 1, length: 2, width: 2, height: 2}],
		};

		expect(mismatches("PackCustomCompareRequest", minimal)).toEqual([]);
	});

	test("every algorithm the type allows is one the spec allows", () => {
		const algorithms: PackCustomRequest["parameters"]["algorithm"][] = ["FFD", "WFD", "BFD", "Best"];

		for (const algorithm of algorithms)
			expect(mismatches("OperationParameters", {algorithm})).toEqual([]);
	});
});

describe("the response type", () => {
	test("matches PackCompareResponse", () => {
		expect(mismatches("PackCompareResponse", compareResponse)).toEqual([]);
	});

	test("one entry matches PackBinResponse, the body of the single-bin routes", () => {
		expect(mismatches("PackBinResponse", packedBin)).toEqual([]);
	});

	test("a partially packed entry with unpacked items matches", () => {
		expect(mismatches("PackBinResponse", partiallyPackedBin)).toEqual([]);
	});

	test("every status the type allows is one the spec allows", () => {
		const statuses: PackBinResponse["status"][] = ["Unknown", "FullyPacked", "PartiallyPacked", "NotPacked"];

		for (const status of statuses)
			expect(mismatches("BinPackResultStatus", status)).toEqual([]);
	});
});

describe("the error types", () => {
	test("matches ProblemDetails", () => {
		const problem: ProblemDetails = {
			type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
			title: "Bad Request",
			status: 400,
			detail: "The request is invalid.",
			instance: "/api/v4/pack/compare-bins",
		};

		expect(mismatches("ProblemDetails", problem)).toEqual([]);
	});

	test("matches HttpValidationProblemDetails, field errors and all", () => {
		const problem: HttpValidationProblemDetails = {
			type: "https://tools.ietf.org/html/rfc9110#section-15.5.21",
			title: "Unprocessable Content",
			status: 422,
			detail: "The request is invalid.",
			instance: "/api/v4/pack/compare-bins",
			errors: {"bins[0].length": ["Length must be 1 or more."]},
		};

		expect(mismatches("HttpValidationProblemDetails", problem)).toEqual([]);
	});
});

describe("the check itself", () => {
	test("names the field when one is missing", () => {
		const {status, ...withoutStatus} = packedBin;

		expect(mismatches("PackBinResponse", withoutStatus)).toEqual(["PackBinResponse must have required property 'status'"]);
	});

	test("names the field when a value is not one the spec allows", () => {
		const wrongStatus = {...packedBin, status: "AlmostPacked"};

		expect(mismatches("PackBinResponse", wrongStatus)).toEqual(["PackBinResponse/status must be equal to one of the allowed values"]);
	});

	test("says so when the spec has no such schema", () => {
		expect(() => mismatches("NoSuchSchema", {})).toThrow("spec/v4.json has no schema named 'NoSuchSchema'.");
	});
});
