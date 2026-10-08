import {
	AccountGetResponse,
	ApiResponse,
	AccountPatchRequest,
	AccountRole,
	AccountStatus,
	ServiceClient,
	SubscriptionPatchRequest,
	SubscriptionStatus,
	SubscriptionType,
} from "binacle-net-service-client";
import {call, clientFor, goToLogin, loggedIn} from "../session";

export const roles: AccountRole[] = ["Admin", "User", "Guest"];
export const accountStatuses: AccountStatus[] = ["Active", "Inactive", "Suspended"];
export const subscriptionTypes: SubscriptionType[] = ["Normal", "Demo"];
export const subscriptionStatuses: SubscriptionStatus[] = ["Active", "Inactive", "Suspended"];

// The account edit form. Empty text means "leave it", so only what was typed goes out in the patch.
interface AccountForm {
	username: string;
	email: string;
	password: string;
	status: AccountStatus | "";
	role: AccountRole | "";
}

interface SubscriptionForm {
	type: SubscriptionType | "";
	status: SubscriptionStatus | "";
}

export class AccountPage {
	id = "";
	account: AccountGetResponse | null = null;
	error = "";
	notice = "";
	form: AccountForm = {username: "", email: "", password: "", status: "", role: ""};
	subscriptionForm: SubscriptionForm = {type: "", status: ""};

	readonly roles = roles;
	readonly accountStatuses = accountStatuses;
	readonly subscriptionTypes = subscriptionTypes;
	readonly subscriptionStatuses = subscriptionStatuses;

	private readonly client: ServiceClient;

	constructor(private readonly baseUrl: string) {
		this.client = clientFor(baseUrl);
	}

	async init(): Promise<void> {
		if (!loggedIn()) {
			goToLogin();
			return;
		}
		this.id = new URLSearchParams(window.location.search).get("id") ?? "";
		if (this.id === "") {
			this.error = "No account id in the address.";
			return;
		}
		await this.load();
	}

	async load(): Promise<void> {
		this.error = "";
		const outcome = await call(this.baseUrl, this.client.getAccount(this.id));
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}
		this.account = outcome.data;
		this.form = {username: "", email: "", password: "", status: "", role: ""};
		this.subscriptionForm = {type: "", status: ""};
	}

	async submitPatch(): Promise<void> {
		const patch: AccountPatchRequest = {};
		if (this.form.username) patch.username = this.form.username;
		if (this.form.email) patch.email = this.form.email;
		if (this.form.password) patch.password = this.form.password;
		if (this.form.status) patch.status = this.form.status;
		if (this.form.role) patch.role = this.form.role;

		await this.apply(this.client.patchAccount(this.id, patch), "Account updated.");
	}

	async remove(): Promise<void> {
		if (!window.confirm(`Delete account ${this.account?.username ?? this.id}?`))
			return;
		const outcome = await call(this.baseUrl, this.client.deleteAccount(this.id));
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}
		window.location.href = "/accounts/";
	}

	async submitSubscriptionCreate(): Promise<void> {
		if (!this.subscriptionForm.type) {
			this.error = "Pick a subscription type.";
			return;
		}
		await this.apply(this.client.createSubscription(this.id, {type: this.subscriptionForm.type}), "Subscription created.");
	}

	async submitSubscriptionPatch(): Promise<void> {
		const patch: SubscriptionPatchRequest = {};
		if (this.subscriptionForm.type) patch.type = this.subscriptionForm.type;
		if (this.subscriptionForm.status) patch.status = this.subscriptionForm.status;

		await this.apply(this.client.patchSubscription(this.id, patch), "Subscription updated.");
	}

	async removeSubscription(): Promise<void> {
		if (!window.confirm("Delete this account's subscription?"))
			return;
		await this.apply(this.client.deleteSubscription(this.id), "Subscription deleted.");
	}

	private async apply(request: Promise<ApiResponse<void>>, done: string): Promise<void> {
		this.error = "";
		this.notice = "";
		const outcome = await call(this.baseUrl, request);
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}
		this.notice = done;
		await this.load();
	}
}
