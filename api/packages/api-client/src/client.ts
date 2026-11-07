import {ApiResponse, readApiResponse} from "./response";
import {PackBinResponse, PackCompareResponse, PackCustomRequest, PackPresetRequest} from "./types";

// The routes this client covers. pack/smallest-bin and pack/best-bin take the same request and answer a
// single PackBinResponse; they go here when something needs them.
export const packCompareBinsPath = "/api/v4/pack/compare-bins";

// The preset and the bin are named in the path, so this one is built per call.
export function packPresetBinPath(preset: string, bin: string): string {
	return `/api/v4/pack/bin/${encodeURIComponent(preset)}/${encodeURIComponent(bin)}`;
}

export interface BinacleClientOptions {
	// Where the API lives, with no trailing slash. Left out, requests go to the host that served the page.
	baseUrl?: string;
	// Swapped by tests, and by a host that wraps fetch. Read on every call, not captured in the constructor.
	fetch?: typeof fetch;
}

export class BinacleClient {
	private readonly baseUrl: string;
	private readonly fetchOverride?: typeof fetch;

	constructor(options: BinacleClientOptions = {}) {
		this.baseUrl = options.baseUrl ?? "";
		this.fetchOverride = options.fetch;
	}

	// Packs the items into every bin and answers a result for each. The body comes back parsed and
	// untouched; what to show for each status is the caller's call.
	packCompareBins(request: PackCustomRequest): Promise<ApiResponse<PackCompareResponse>> {
		return this.post<PackCompareResponse>(packCompareBinsPath, request);
	}

	// Packs the items into one bin of a preset the API holds, so the request carries no bin sizes.
	packPresetBin(preset: string, bin: string, request: PackPresetRequest): Promise<ApiResponse<PackBinResponse>> {
		return this.post<PackBinResponse>(packPresetBinPath(preset, bin), request);
	}

	private async post<T>(path: string, body: unknown): Promise<ApiResponse<T>> {
		const send = this.fetchOverride ?? globalThis.fetch;
		const response = await send(`${this.baseUrl}${path}`, {
			method: "POST",
			headers: {"Content-Type": "application/json"},
			body: JSON.stringify(body),
		});

		return readApiResponse<T>(response);
	}
}
