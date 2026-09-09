// The v4 wire contracts, one type per schema in spec/v4.json under components/schemas. Names match the
// spec's names so a failing contract test points straight at the schema it checked.
//
// The spec shares its pieces through $ref rather than repeating them, and these types share the same way:
// Bin, Box, PackedBox and UnpackedBox are used by every route, and the three custom pack routes take one
// request shape between them.

// The packing heuristic. Best runs more than one and keeps the winner.
export type Algorithm = "FFD" | "WFD" | "BFD" | "Best";

// Outcome of a packing operation. Pack routes never exit early, so there is no early-fail value here.
export type BinPackResultStatus = "Unknown" | "FullyPacked" | "PartiallyPacked" | "NotPacked";

export interface OperationParameters {
	algorithm: Algorithm;
	includeViPaqData?: boolean;
}

// A container to pack into. The id is echoed back on the matching result.
export interface Bin {
	id: string;
	length: number;
	width: number;
	height: number;
}

// An item to pack, with how many of it there are.
export interface Box {
	id: string;
	quantity: number;
	length: number;
	width: number;
	height: number;
}

// An item placed in the bin. x, y and z are its corner, measured from the bin origin.
export interface PackedBox {
	id: string;
	length: number;
	width: number;
	height: number;
	x: number;
	y: number;
	z: number;
}

// An item that did not fit.
export interface UnpackedBox {
	id: string;
	quantity: number;
}

// What pack/compare-bins, pack/smallest-bin and pack/best-bin all take. The spec declares one schema per
// route (PackCustomCompareRequest, PackCustomSmallestBinRequest, PackCustomBestBinRequest) and all three
// carry these same three fields.
export interface PackCustomRequest {
	parameters: OperationParameters;
	bins: Bin[];
	items: Box[];
}

// The result for one bin. This is the whole body of pack/smallest-bin and pack/best-bin, and one entry of
// pack/compare-bins.
export interface PackBinResponse {
	status: BinPackResultStatus;
	bin: Bin;
	// The heuristic that actually ran. With Algorithm "Best", the one that won.
	algorithmUsed: string;
	packedItems?: PackedBox[] | null;
	unpackedItems?: UnpackedBox[] | null;
	// 0-100.
	packedItemsVolumePercentage: number;
	packedBinVolumePercentage: number;
	// Base64 ViPaq token, present only when includeViPaqData was set.
	viPaqData?: string | null;
}

// The body of pack/compare-bins: one result per bin considered.
export interface PackCompareResponse {
	results: PackBinResponse[];
}

// RFC 7807. What the API answers on 4xx and 5xx.
export interface ProblemDetails {
	type?: string | null;
	title?: string | null;
	status?: number | null;
	detail?: string | null;
	instance?: string | null;
}

// A ProblemDetails that also lists what failed validation, keyed by field.
export interface HttpValidationProblemDetails extends ProblemDetails {
	errors?: Record<string, string[]>;
}
