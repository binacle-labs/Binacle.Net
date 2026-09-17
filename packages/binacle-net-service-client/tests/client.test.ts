import {
	accountPath,
	accountsPath,
	createAccountPath,
	ServiceClient,
	subscriptionPath,
	subscriptionsPath,
	TokenResponse,
	tokenPath,
} from "../src";

// The client only ever reads ok, status and text(), so a stub is enough.
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

const token: TokenResponse = {tokenType: "Bearer", accessToken: "abc", expiresIn: 3600};

const emptyPage = JSON.stringify({total: 0, page: 1, pageSize: 50, totalPages: 0, items: []});

describe("the token route", () => {
	test("posts the credentials with no Authorization header", async () => {
		const fetchMock = mockFetch(stubResponse(200, JSON.stringify(token)));

		const response = await new ServiceClient({token: "stale"}).requestToken({username: "u", password: "p"});

		expect(fetchMock.mock.calls[0][0]).toBe(tokenPath);
		expect(fetchMock.mock.calls[0][1].method).toBe("POST");
		expect(fetchMock.mock.calls[0][1].headers).toEqual({"Content-Type": "application/json"});
		expect(JSON.parse(fetchMock.mock.calls[0][1].body)).toEqual({username: "u", password: "p"});
		expect(response.ok && response.data).toEqual(token);
	});

	test("a 401 with no body is a failure with a null problem", async () => {
		mockFetch(stubResponse(401, ""));

		const response = await new ServiceClient().requestToken({username: "u", password: "wrong"});

		expect(response).toEqual({ok: false, status: 401, problem: null});
	});
});

describe("the bearer token", () => {
	test("a string goes on every admin call", async () => {
		const fetchMock = mockFetch(stubResponse(200, emptyPage));

		await new ServiceClient({token: "abc"}).listAccounts();

		expect(fetchMock.mock.calls[0][1].headers).toEqual({Authorization: "Bearer abc"});
	});

	test("a function is read on every call, so a later login is picked up", async () => {
		const fetchMock = mockFetch(stubResponse(200, emptyPage));
		let current: string | null = null;
		const client = new ServiceClient({token: () => current});

		await client.listAccounts();
		current = "after-login";
		await client.listAccounts();

		expect(fetchMock.mock.calls[0][1].headers).toEqual({});
		expect(fetchMock.mock.calls[1][1].headers).toEqual({Authorization: "Bearer after-login"});
	});

	test("a baseUrl is put in front of the route", async () => {
		const fetchMock = mockFetch(stubResponse(200, emptyPage));

		await new ServiceClient({baseUrl: "https://localhost:7194"}).listAccounts();

		expect(fetchMock.mock.calls[0][0]).toBe(`https://localhost:7194${accountsPath}`);
	});
});

describe("the list routes", () => {
	test("send only the query parameters that were given", async () => {
		const fetchMock = mockFetch(stubResponse(200, emptyPage));
		const client = new ServiceClient();

		await client.listAccounts();
		await client.listAccounts({page: 2});
		await client.listSubscriptions({page: 1, pageSize: 10, allowDeleted: true});

		expect(fetchMock.mock.calls[0][0]).toBe(accountsPath);
		expect(fetchMock.mock.calls[1][0]).toBe(`${accountsPath}?page=2`);
		expect(fetchMock.mock.calls[2][0]).toBe(`${subscriptionsPath}?page=1&pageSize=10&allowDeleted=true`);
	});

	test("are GETs with no body", async () => {
		const fetchMock = mockFetch(stubResponse(200, emptyPage));

		await new ServiceClient().listAccounts();

		expect(fetchMock.mock.calls[0][1].method).toBe("GET");
		expect(fetchMock.mock.calls[0][1].body).toBeUndefined();
	});
});

describe("the routes with a body and no answer", () => {
	test.each([
		["createAccount", "POST", createAccountPath, (c: ServiceClient) => c.createAccount({username: "u", password: "p", email: "e"})],
		["updateAccount", "PUT", accountPath("id-1"), (c: ServiceClient) => c.updateAccount("id-1", {username: "u", password: "p", email: "e", status: "Active", role: "User"})],
		["patchAccount", "PATCH", accountPath("id-1"), (c: ServiceClient) => c.patchAccount("id-1", {status: "Suspended"})],
		["createSubscription", "POST", subscriptionPath("id-1"), (c: ServiceClient) => c.createSubscription("id-1", {type: "Normal"})],
		["updateSubscription", "PUT", subscriptionPath("id-1"), (c: ServiceClient) => c.updateSubscription("id-1", {type: "Normal", status: "Active"})],
		["patchSubscription", "PATCH", subscriptionPath("id-1"), (c: ServiceClient) => c.patchSubscription("id-1", {status: "Inactive"})],
	])("%s is a %s to %s and a 204 is ok", async (_name, method, path, call) => {
		const fetchMock = mockFetch(stubResponse(204, ""));

		const response = await call(new ServiceClient({token: "abc"}));

		expect(fetchMock.mock.calls[0][0]).toBe(path);
		expect(fetchMock.mock.calls[0][1].method).toBe(method);
		expect(fetchMock.mock.calls[0][1].headers).toEqual({"Content-Type": "application/json", Authorization: "Bearer abc"});
		expect(response).toEqual({ok: true, status: 204, data: undefined});
	});

	test("a 201 with no body is ok", async () => {
		mockFetch(stubResponse(201, ""));

		const response = await new ServiceClient().createAccount({username: "u", password: "p", email: "e"});

		expect(response).toEqual({ok: true, status: 201, data: undefined});
	});

	test("a 422 carries the problem body", async () => {
		const problem = {title: "Unprocessable Content", status: 422, errors: {Password: ["too short"]}};
		mockFetch(stubResponse(422, JSON.stringify(problem)));

		const response = await new ServiceClient().createAccount({username: "u", password: "p", email: "e"});

		expect(response).toEqual({ok: false, status: 422, problem});
	});

	test("a 409 with no body is a failure with a null problem", async () => {
		mockFetch(stubResponse(409, ""));

		const response = await new ServiceClient().createAccount({username: "u", password: "p", email: "e"});

		expect(response).toEqual({ok: false, status: 409, problem: null});
	});
});

describe("the delete routes", () => {
	test.each([
		["deleteAccount", accountPath("id-1"), (c: ServiceClient) => c.deleteAccount("id-1")],
		["deleteSubscription", subscriptionPath("id-1"), (c: ServiceClient) => c.deleteSubscription("id-1")],
	])("%s is a DELETE to %s with no body", async (_name, path, call) => {
		const fetchMock = mockFetch(stubResponse(204, ""));

		const response = await call(new ServiceClient({token: "abc"}));

		expect(fetchMock.mock.calls[0][0]).toBe(path);
		expect(fetchMock.mock.calls[0][1].method).toBe("DELETE");
		expect(fetchMock.mock.calls[0][1].headers).toEqual({Authorization: "Bearer abc"});
		expect(fetchMock.mock.calls[0][1].body).toBeUndefined();
		expect(response).toEqual({ok: true, status: 204, data: undefined});
	});
});

describe("an id in the path", () => {
	test("is escaped", () => {
		expect(accountPath("a/b")).toBe(`${createAccountPath}/a%2Fb`);
	});
});
