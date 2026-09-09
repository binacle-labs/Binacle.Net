import {
	ApiResponse,
	BinacleClient,
	HttpValidationProblemDetails,
	hasValidationErrors,
	PackCompareResponse,
	PackCustomRequest,
	packCompareBinsPath,
} from "../src";

// The client only ever reads ok, status and text(), so a stub is enough and a real Response would need a
// body stream per case.
function stubResponse(status: number, body: string): Response {
	return {
		ok: status >= 200 && status < 300,
		status,
		text: () => Promise.resolve(body),
	} as unknown as Response;
}

function mockFetch(response: Response) {
	const fetchMock = jest.fn().mockResolvedValue(response);
	globalThis.fetch = fetchMock as unknown as typeof fetch;

	return fetchMock;
}

function request(): PackCustomRequest {
	return {
		parameters: {algorithm: "Best"},
		bins: [{id: "bin_1", length: 10, width: 10, height: 10}],
		items: [{id: "box_1", quantity: 1, length: 2, width: 2, height: 2}],
	};
}

const compareResponse: PackCompareResponse = {
	results: [
		{
			status: "FullyPacked",
			bin: {id: "bin_1", length: 10, width: 10, height: 10},
			algorithmUsed: "FFD",
			packedItems: [{id: "box_1", length: 2, width: 2, height: 2, x: 0, y: 0, z: 0}],
			unpackedItems: null,
			packedItemsVolumePercentage: 100,
			packedBinVolumePercentage: 0.8,
			viPaqData: null,
		},
	],
};

// Narrows to the success half so a test can read `data` without a cast. Fails the test if it is a failure.
function successOf<T>(response: ApiResponse<T>): T {
	if (!response.ok)
		throw new Error(`Expected a success, got ${response.status}.`);
	return response.data;
}

describe("the request", () => {
	test("goes to the compare-bins endpoint", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		await new BinacleClient().packCompareBins(request());

		expect(fetchMock.mock.calls[0][0]).toBe(packCompareBinsPath);
	});

	test("a baseUrl is put in front of the endpoint", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		await new BinacleClient({baseUrl: "https://api.example.com"}).packCompareBins(request());

		expect(fetchMock.mock.calls[0][0]).toBe(`https://api.example.com${packCompareBinsPath}`);
	});

	test("is a POST", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		await new BinacleClient().packCompareBins(request());

		expect(fetchMock.mock.calls[0][1].method).toBe("POST");
	});

	test("declares a JSON body", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		await new BinacleClient().packCompareBins(request());

		expect(fetchMock.mock.calls[0][1].headers).toEqual({"Content-Type": "application/json"});
	});

	test("carries the request as JSON", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		await new BinacleClient().packCompareBins(request());

		expect(JSON.parse(fetchMock.mock.calls[0][1].body)).toEqual(request());
	});

	test("a fetch passed in the options is used instead of the global one", async () => {
		mockFetch(stubResponse(500, ""));
		const ownFetch = jest.fn().mockResolvedValue(stubResponse(200, JSON.stringify(compareResponse)));

		const response = await new BinacleClient({fetch: ownFetch as unknown as typeof fetch})
			.packCompareBins(request());

		expect(ownFetch).toHaveBeenCalledTimes(1);
		expect(response.ok).toBe(true);
	});
});

describe("a 200", () => {
	test("answers ok", async () => {
		mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.ok).toBe(true);
	});

	test("carries the status", async () => {
		mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.status).toBe(200);
	});

	test("answers the parsed body, untouched", async () => {
		mockFetch(stubResponse(200, JSON.stringify(compareResponse)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(successOf(response)).toEqual(compareResponse);
	});

	test("a body that is not JSON throws", async () => {
		mockFetch(stubResponse(200, "<html>a proxy answered</html>"));

		await expect(new BinacleClient().packCompareBins(request())).rejects.toThrow();
	});

	test("an empty body throws rather than answering a success with nothing in it", async () => {
		mockFetch(stubResponse(200, ""));

		await expect(new BinacleClient().packCompareBins(request()))
			.rejects.toThrow("Binacle.Net answered 200 with an empty body.");
	});
});

describe("a 4xx with a body", () => {
	const problem: HttpValidationProblemDetails = {
		type: "https://tools.ietf.org/html/rfc9110#section-15.5.21",
		title: "Unprocessable Content",
		status: 422,
		detail: "The request is invalid.",
		errors: {"bins[0].length": ["Length must be 1 or more."]},
	};

	test("answers not ok", async () => {
		mockFetch(stubResponse(422, JSON.stringify(problem)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.ok).toBe(false);
	});

	test("carries the status", async () => {
		mockFetch(stubResponse(422, JSON.stringify(problem)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.status).toBe(422);
	});

	test("carries the problem details", async () => {
		mockFetch(stubResponse(422, JSON.stringify(problem)));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.ok ? null : response.problem).toEqual(problem);
	});

	test("the field errors are reachable through hasValidationErrors", async () => {
		mockFetch(stubResponse(422, JSON.stringify(problem)));

		const response = await new BinacleClient().packCompareBins(request());
		const found = response.ok ? null : response.problem;

		expect(hasValidationErrors(found) ? found.errors : null).toEqual(problem.errors);
	});

	test("a 400 with plain problem details carries no field errors", async () => {
		mockFetch(stubResponse(400, JSON.stringify({title: "Bad Request", status: 400})));

		const response = await new BinacleClient().packCompareBins(request());

		expect(hasValidationErrors(response.ok ? null : response.problem)).toBe(false);
	});

	test("a body that is there and does not parse throws, because that is not an absent body", async () => {
		mockFetch(stubResponse(400, "<html>a proxy answered</html>"));

		await expect(new BinacleClient().packCompareBins(request())).rejects.toThrow();
	});
});

describe("a 429 with no body", () => {
	test("answers not ok rather than throwing", async () => {
		mockFetch(stubResponse(429, ""));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.ok).toBe(false);
	});

	test("carries the status, so the caller can tell a rate limit from anything else", async () => {
		mockFetch(stubResponse(429, ""));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.status).toBe(429);
	});

	test("the missing body reads as null, not as a parse failure", async () => {
		mockFetch(stubResponse(429, ""));

		const response = await new BinacleClient().packCompareBins(request());

		expect(response.ok ? "ok" : response.problem).toBeNull();
	});
});
