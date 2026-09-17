import {ApiResponse, readApiResponse, readEmptyResponse} from "./response";
import {
	AccountCreateRequest,
	AccountGetResponse,
	AccountListItem,
	AccountPatchRequest,
	AccountUpdateRequest,
	PagedResponse,
	PageQuery,
	SubscriptionCreateRequest,
	SubscriptionGetResponse,
	SubscriptionPatchRequest,
	SubscriptionUpdateRequest,
	TokenRequest,
	TokenResponse,
} from "./types";

export const tokenPath = "/api/auth/token";
export const accountsPath = "/api/admin/accounts";
export const subscriptionsPath = "/api/admin/subscriptions";
export const createAccountPath = "/api/admin/account";
export const accountPath = (id: string) => `${createAccountPath}/${encodeURIComponent(id)}`;
export const subscriptionPath = (accountId: string) => `${accountPath(accountId)}/subscription`;

export interface ServiceClientOptions {
	// Where the API lives, with no trailing slash. Left out, requests go to the host that served the page.
	baseUrl?: string;
	// Swapped by tests, and by a host that wraps fetch. Read on every call, not captured in the constructor.
	fetch?: typeof fetch;
	// The bearer token for the admin routes. A function is read on every call, so a login after construction
	// is picked up. Absent, the admin routes go out without one and the API answers 401.
	token?: string | (() => string | null | undefined);
}

export class ServiceClient {
	private readonly baseUrl: string;
	private readonly fetchOverride?: typeof fetch;
	private readonly tokenOption?: ServiceClientOptions["token"];

	constructor(options: ServiceClientOptions = {}) {
		this.baseUrl = options.baseUrl ?? "";
		this.fetchOverride = options.fetch;
		this.tokenOption = options.token;
	}

	// The one route that needs no token.
	requestToken(request: TokenRequest): Promise<ApiResponse<TokenResponse>> {
		return this.withBody<TokenResponse>("POST", tokenPath, request, false);
	}

	listAccounts(query: PageQuery = {}): Promise<ApiResponse<PagedResponse<AccountListItem>>> {
		return this.get<PagedResponse<AccountListItem>>(withQuery(accountsPath, query));
	}

	getAccount(id: string): Promise<ApiResponse<AccountGetResponse>> {
		return this.get<AccountGetResponse>(accountPath(id));
	}

	createAccount(request: AccountCreateRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("POST", createAccountPath, request);
	}

	updateAccount(id: string, request: AccountUpdateRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("PUT", accountPath(id), request);
	}

	patchAccount(id: string, request: AccountPatchRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("PATCH", accountPath(id), request);
	}

	deleteAccount(id: string): Promise<ApiResponse<void>> {
		return this.withoutBody("DELETE", accountPath(id));
	}

	listSubscriptions(query: PageQuery = {}): Promise<ApiResponse<PagedResponse<SubscriptionGetResponse>>> {
		return this.get<PagedResponse<SubscriptionGetResponse>>(withQuery(subscriptionsPath, query));
	}

	getSubscription(accountId: string): Promise<ApiResponse<SubscriptionGetResponse>> {
		return this.get<SubscriptionGetResponse>(subscriptionPath(accountId));
	}

	createSubscription(accountId: string, request: SubscriptionCreateRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("POST", subscriptionPath(accountId), request);
	}

	updateSubscription(accountId: string, request: SubscriptionUpdateRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("PUT", subscriptionPath(accountId), request);
	}

	patchSubscription(accountId: string, request: SubscriptionPatchRequest): Promise<ApiResponse<void>> {
		return this.withoutBody("PATCH", subscriptionPath(accountId), request);
	}

	deleteSubscription(accountId: string): Promise<ApiResponse<void>> {
		return this.withoutBody("DELETE", subscriptionPath(accountId));
	}

	private async get<T>(path: string): Promise<ApiResponse<T>> {
		const response = await this.send("GET", path);
		return readApiResponse<T>(response);
	}

	// A call that answers a body.
	private async withBody<T>(method: string, path: string, body: unknown, authorised = true): Promise<ApiResponse<T>> {
		const response = await this.send(method, path, body, authorised);
		return readApiResponse<T>(response);
	}

	// A call that answers 201 or 204 and nothing else on success.
	private async withoutBody(method: string, path: string, body?: unknown): Promise<ApiResponse<void>> {
		const response = await this.send(method, path, body);
		return readEmptyResponse(response);
	}

	private send(method: string, path: string, body?: unknown, authorised = true): Promise<Response> {
		const fetcher = this.fetchOverride ?? globalThis.fetch;
		const headers: Record<string, string> = {};

		if (body !== undefined)
			headers["Content-Type"] = "application/json";

		const token = authorised ? this.readToken() : null;
		if (token)
			headers["Authorization"] = `Bearer ${token}`;

		return fetcher(`${this.baseUrl}${path}`, {
			method,
			headers,
			body: body === undefined ? undefined : JSON.stringify(body),
		});
	}

	private readToken(): string | null {
		const token = typeof this.tokenOption === "function" ? this.tokenOption() : this.tokenOption;
		return token ? token : null;
	}
}

// Only the parameters that were given go on the URL, so the API applies its own defaults for the rest.
function withQuery(path: string, query: PageQuery): string {
	const parameters = new URLSearchParams();

	if (query.page !== undefined)
		parameters.set("page", String(query.page));
	if (query.pageSize !== undefined)
		parameters.set("pageSize", String(query.pageSize));
	if (query.allowDeleted !== undefined)
		parameters.set("allowDeleted", String(query.allowDeleted));

	const text = parameters.toString();
	return text.length === 0 ? path : `${path}?${text}`;
}
