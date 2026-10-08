import {AccountListItem, ServiceClient} from "binacle-net-service-client";
import {Paging} from "../paging";
import {call, clientFor, goToLogin, loggedIn} from "../session";

export class AccountsPage {
	rows: AccountListItem[] = [];
	paging = new Paging();
	error = "";
	notice = "";
	create = {username: "", email: "", password: ""};

	private readonly client: ServiceClient;

	constructor(private readonly baseUrl: string) {
		this.client = clientFor(baseUrl);
	}

	async init(): Promise<void> {
		if (!loggedIn()) {
			goToLogin();
			return;
		}
		await this.load();
	}

	async load(): Promise<void> {
		this.error = "";
		const outcome = await call(this.baseUrl, this.client.listAccounts(this.paging.query()));
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}
		this.rows = outcome.data.items;
		this.paging.take(outcome.data);
	}

	async go(page: number): Promise<void> {
		this.paging.page = page;
		await this.load();
	}

	async submitCreate(): Promise<void> {
		this.error = "";
		this.notice = "";
		const outcome = await call(this.baseUrl, this.client.createAccount(this.create));
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}
		this.notice = `Created ${this.create.username}.`;
		this.create = {username: "", email: "", password: ""};
		await this.load();
	}

	open(row: AccountListItem): void {
		window.location.href = `/account/?id=${encodeURIComponent(row.id)}`;
	}
}
