// The service wire contracts, one type per schema in spec/service.json under components/schemas. Names match
// the spec's names so a failing contract test points straight at the schema it checked.

export type AccountRole = "Admin" | "User" | "Guest";
export type AccountStatus = "Active" | "Inactive" | "Suspended";
export type SubscriptionType = "Normal" | "Demo";
export type SubscriptionStatus = "Active" | "Inactive" | "Suspended";

export interface TokenRequest {
	username: string;
	password: string;
}

export interface TokenResponse {
	tokenType: string;
	accessToken: string;
	expiresIn: number;
	refreshToken?: string | null;
}

// Both list routes take these as query parameters. Left out: page 1, 50 per page, deleted rows hidden.
export interface PageQuery {
	page?: number;
	pageSize?: number;
	allowDeleted?: boolean;
}

export interface PagedResponse<T> {
	total: number;
	page: number;
	pageSize: number;
	totalPages: number;
	items: T[];
}

export interface AccountCreateRequest {
	username: string;
	password: string;
	email: string;
}

export interface AccountUpdateRequest {
	username: string;
	email: string;
	password: string;
	status: AccountStatus;
	role: AccountRole;
}

// Every field optional, at least one must be present.
export interface AccountPatchRequest {
	username?: string | null;
	email?: string | null;
	password?: string | null;
	status?: AccountStatus | null;
	role?: AccountRole | null;
}

export interface AccountSubscription {
	id: string;
	type: SubscriptionType;
	status: SubscriptionStatus;
	createdAtUtc: string;
	isDeleted: boolean;
	deletedAtUtc?: string | null;
}

export interface AccountGetResponse {
	id: string;
	username: string;
	role: AccountRole;
	email: string;
	status: AccountStatus;
	passwordHash?: string | null;
	securityStamp: string;
	subscription?: AccountSubscription | null;
	createdAtUtc: string;
	isDeleted: boolean;
	deletedAtUtc?: string | null;
}

// The list row. No password hash and no security stamp - those come back only on a single get.
export interface AccountListItem {
	id: string;
	username: string;
	role: AccountRole;
	email: string;
	status: AccountStatus;
	subscriptionId?: string | null;
	createdAtUtc: string;
	isDeleted: boolean;
	deletedAtUtc?: string | null;
}

export interface SubscriptionCreateRequest {
	type: SubscriptionType;
}

export interface SubscriptionUpdateRequest {
	type: SubscriptionType;
	status: SubscriptionStatus;
}

// Every field optional, at least one must be present.
export interface SubscriptionPatchRequest {
	type?: SubscriptionType | null;
	status?: SubscriptionStatus | null;
}

// Also the list row.
export interface SubscriptionGetResponse {
	id: string;
	accountId: string;
	type: SubscriptionType;
	status: SubscriptionStatus;
	createdAtUtc: string;
	isDeleted: boolean;
	deletedAtUtc?: string | null;
}

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
