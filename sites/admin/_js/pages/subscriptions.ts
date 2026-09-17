import {ServiceClient, SubscriptionGetResponse} from "binacle-net-service-client";
import {Paging} from "../paging";
import {call, clientFor, goToLogin, loggedIn} from "../session";

export class SubscriptionsPage {
	rows: SubscriptionGetResponse[] = [];
	paging = new Paging();
	error = "";

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
		const outcome = await call(this.baseUrl, this.client.listSubscriptions(this.paging.query()));
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

	open(row: SubscriptionGetResponse): void {
		window.location.href = `/account/?id=${encodeURIComponent(row.accountId)}`;
	}
}
