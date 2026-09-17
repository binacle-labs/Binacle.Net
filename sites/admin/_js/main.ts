import Alpine from "alpinejs";

import {AccountPage} from "./pages/account";
import {AccountsPage} from "./pages/accounts";
import {LoginPage} from "./pages/login";
import {SubscriptionsPage} from "./pages/subscriptions";
import {goToLogin, loggedIn} from "./session";

// The nav on every page.
Alpine.data("shell", () => ({
	loggedIn: loggedIn(),
	logout: goToLogin,
}));

Alpine.data("login", (baseUrl: string) => new LoginPage(baseUrl));
Alpine.data("accounts", (baseUrl: string) => new AccountsPage(baseUrl));
Alpine.data("account", (baseUrl: string) => new AccountPage(baseUrl));
Alpine.data("subscriptions", (baseUrl: string) => new SubscriptionsPage(baseUrl));

Alpine.start();
