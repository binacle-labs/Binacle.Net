import {call, clientFor, saveToken} from "../session";

export class LoginPage {
	username = "";
	password = "";
	error = "";
	busy = false;

	constructor(private readonly baseUrl: string) {}

	async submit(): Promise<void> {
		this.busy = true;
		this.error = "";

		const client = clientFor(this.baseUrl);
		const outcome = await call(this.baseUrl, client.requestToken({username: this.username, password: this.password}));

		this.busy = false;
		if (!outcome.ok) {
			this.error = outcome.error;
			return;
		}

		saveToken(outcome.data.accessToken);
		window.location.href = "/accounts/";
	}
}
