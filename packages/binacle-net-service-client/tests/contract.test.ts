import {readFileSync} from "fs";
import {join} from "path";

import Ajv, {ValidateFunction} from "ajv";

import {
	AccountCreateRequest,
	AccountGetResponse,
	AccountListItem,
	AccountPatchRequest,
	AccountRole,
	AccountStatus,
	AccountUpdateRequest,
	PagedResponse,
	SubscriptionCreateRequest,
	SubscriptionGetResponse,
	SubscriptionPatchRequest,
	SubscriptionStatus,
	SubscriptionType,
	SubscriptionUpdateRequest,
	TokenRequest,
	TokenResponse,
} from "../src";

// The fixtures below are annotated with our hand-written types, so the compiler rejects a fixture our types
// cannot describe, and ajv rejects one the spec cannot. A field renamed in the API reaches spec/service.json
// through `just openapi sync-all-copies` and fails here.
const spec = JSON.parse(readFileSync(join(__dirname, "..", "spec", "service.json"), "utf8"));

// strict: false, because an OpenAPI document carries keywords ajv's strict mode rejects. validateFormats:
// false, because uuid, date-time and int32 are OpenAPI formats; the enums, the required lists and the types
// are what this test is checking.
const ajv = new Ajv({strict: false, validateFormats: false});

ajv.addSchema(spec, "service");

function validatorFor(schema: string): ValidateFunction {
	const validate = ajv.getSchema(`service#/components/schemas/${schema}`);
	if (!validate)
		throw new Error(`spec/service.json has no schema named '${schema}'.`);
	return validate;
}

function mismatches(schema: string, fixture: unknown): string[] {
	const validate = validatorFor(schema);
	if (validate(fixture))
		return [];

	return (validate.errors ?? []).map(error => {
		const field = error.instancePath === "" ? schema : `${schema}${error.instancePath}`;
		return `${field} ${error.message ?? "is invalid"}`;
	});
}

const accountId = "7433feec-4863-41df-ba45-57eb52c3f014";
const subscriptionId = "526501c7-653c-4430-9808-cf64aaf188fa";
const when = "2025-01-11T14:30:53+00:00";

const subscription: SubscriptionGetResponse = {
	id: subscriptionId,
	accountId,
	type: "Normal",
	status: "Active",
	createdAtUtc: when,
	isDeleted: false,
	deletedAtUtc: null,
};

const account: AccountGetResponse = {
	id: accountId,
	username: "user@example.binacle.net",
	role: "User",
	email: "user@example.binacle.net",
	status: "Active",
	passwordHash: "type::hash::salt",
	securityStamp: "753a88e6-f69b-4362-8b7b-d4b1958c926f",
	subscription: {
		id: subscriptionId,
		type: "Normal",
		status: "Active",
		createdAtUtc: when,
		isDeleted: false,
		deletedAtUtc: null,
	},
	createdAtUtc: when,
	isDeleted: false,
	deletedAtUtc: null,
};

const accountRow: AccountListItem = {
	id: accountId,
	username: "user@example.binacle.net",
	role: "User",
	email: "user@example.binacle.net",
	status: "Active",
	subscriptionId,
	createdAtUtc: when,
	isDeleted: false,
	deletedAtUtc: null,
};

describe("the auth types", () => {
	test("the request matches TokenRequest", () => {
		const request: TokenRequest = {username: "test_user", password: "testpassword"};

		expect(mismatches("TokenRequest", request)).toEqual([]);
	});

	test("the response matches TokenResponse", () => {
		const response: TokenResponse = {tokenType: "Bearer", accessToken: "eyJ.abc.def", expiresIn: 3600, refreshToken: null};

		expect(mismatches("TokenResponse", response)).toEqual([]);
	});
});

describe("the account types", () => {
	test("matches AccountCreateRequest", () => {
		const request: AccountCreateRequest = {username: "test_user", password: "testpassword", email: "user@domain.test"};

		expect(mismatches("AccountCreateRequest", request)).toEqual([]);
	});

	test("matches AccountUpdateRequest", () => {
		const request: AccountUpdateRequest = {
			username: "test_user",
			email: "user@domain.test",
			password: "testpassword",
			status: "Active",
			role: "User",
		};

		expect(mismatches("AccountUpdateRequest", request)).toEqual([]);
	});

	test("matches AccountPatchRequest with one field", () => {
		const request: AccountPatchRequest = {status: "Suspended"};

		expect(mismatches("AccountPatchRequest", request)).toEqual([]);
	});

	test("matches AccountGetResponse", () => {
		expect(mismatches("AccountGetResponse", account)).toEqual([]);
	});

	test("matches AccountGetResponse with no subscription and no hash", () => {
		const bare: AccountGetResponse = {...account, subscription: null, passwordHash: null};

		expect(mismatches("AccountGetResponse", bare)).toEqual([]);
	});

	test("a page of rows matches PagedResponseOfAccountListItem", () => {
		const page: PagedResponse<AccountListItem> = {total: 1, page: 1, pageSize: 50, totalPages: 1, items: [accountRow]};

		expect(mismatches("PagedResponseOfAccountListItem", page)).toEqual([]);
	});

	test("every role the type allows is one the spec allows", () => {
		const roles: AccountRole[] = ["Admin", "User", "Guest"];

		for (const role of roles)
			expect(mismatches("AccountRole", role)).toEqual([]);
	});

	test("every status the type allows is one the spec allows", () => {
		const statuses: AccountStatus[] = ["Active", "Inactive", "Suspended"];

		for (const status of statuses)
			expect(mismatches("AccountStatus", status)).toEqual([]);
	});
});

describe("the subscription types", () => {
	test("matches SubscriptionCreateRequest", () => {
		const request: SubscriptionCreateRequest = {type: "Demo"};

		expect(mismatches("SubscriptionCreateRequest", request)).toEqual([]);
	});

	test("matches SubscriptionUpdateRequest", () => {
		const request: SubscriptionUpdateRequest = {type: "Normal", status: "Inactive"};

		expect(mismatches("SubscriptionUpdateRequest", request)).toEqual([]);
	});

	test("matches SubscriptionPatchRequest with one field", () => {
		const request: SubscriptionPatchRequest = {status: "Suspended"};

		expect(mismatches("SubscriptionPatchRequest", request)).toEqual([]);
	});

	test("matches SubscriptionGetResponse", () => {
		expect(mismatches("SubscriptionGetResponse", subscription)).toEqual([]);
	});

	test("a page of rows matches PagedResponseOfSubscriptionGetResponse", () => {
		const page: PagedResponse<SubscriptionGetResponse> = {total: 1, page: 1, pageSize: 50, totalPages: 1, items: [subscription]};

		expect(mismatches("PagedResponseOfSubscriptionGetResponse", page)).toEqual([]);
	});

	test("every type the type allows is one the spec allows", () => {
		const types: SubscriptionType[] = ["Normal", "Demo"];

		for (const type of types)
			expect(mismatches("SubscriptionType", type)).toEqual([]);
	});

	test("every status the type allows is one the spec allows", () => {
		const statuses: SubscriptionStatus[] = ["Active", "Inactive", "Suspended"];

		for (const status of statuses)
			expect(mismatches("SubscriptionStatus", status)).toEqual([]);
	});
});

describe("the check itself", () => {
	test("names the field when one is missing", () => {
		const {status, ...withoutStatus} = subscription;

		expect(mismatches("SubscriptionGetResponse", withoutStatus)).toEqual(["SubscriptionGetResponse must have required property 'status'"]);
	});

	test("names the field when a value is not one the spec allows", () => {
		const wrongStatus = {...subscription, status: "Paused"};

		expect(mismatches("SubscriptionGetResponse", wrongStatus)).toEqual(["SubscriptionGetResponse/status must be equal to one of the allowed values"]);
	});

	test("says so when the spec has no such schema", () => {
		expect(() => mismatches("NoSuchSchema", {})).toThrow("spec/service.json has no schema named 'NoSuchSchema'.");
	});
});
