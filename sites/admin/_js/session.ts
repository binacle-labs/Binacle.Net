import {ApiProblem, ApiResponse, hasValidationErrors, ServiceClient} from "binacle-net-service-client";

const tokenKey = "binacle-admin-token";

export function readToken(): string | null {
	return sessionStorage.getItem(tokenKey);
}

export function saveToken(token: string): void {
	sessionStorage.setItem(tokenKey, token);
}

export function clearToken(): void {
	sessionStorage.removeItem(tokenKey);
}

export function loggedIn(): boolean {
	return readToken() !== null;
}

// Sends the browser to the login page. Every page but login calls it when there is no token, and again on
// a 401, which means the token expired.
export function goToLogin(): void {
	clearToken();
	window.location.href = "/";
}

export function clientFor(baseUrl: string): ServiceClient {
	return new ServiceClient({baseUrl, token: readToken});
}

export type Outcome<T> = {ok: true; data: T} | {ok: false; error: string};

// Every page call goes through here. A thrown fetch - the API is down, or does not allow this origin - becomes
// a message, not a broken page. A 401 ends the session.
export async function call<T>(baseUrl: string, request: Promise<ApiResponse<T>>): Promise<Outcome<T>> {
	let response: ApiResponse<T>;
	try {
		response = await request;
	} catch (fault) {
		return {ok: false, error: `Could not reach the API at ${baseUrl}. Is it running, and does it allow this origin? (${String(fault)})`};
	}

	if (response.ok)
		return {ok: true, data: response.data};

	if (response.status === 401) {
		goToLogin();
		return {ok: false, error: "Not logged in."};
	}

	return {ok: false, error: describeFailure(response.status, response.problem)};
}

function describeFailure(status: number, problem: ApiProblem | null): string {
	if (problem === null)
		return `The API answered ${status} with no body.`;

	const lines = [`${status} ${problem.title ?? ""}`.trim()];
	if (problem.detail)
		lines.push(problem.detail);
	if (hasValidationErrors(problem))
		for (const [field, messages] of Object.entries(problem.errors ?? {}))
			lines.push(`${field || "request"}: ${messages.join(" ")}`);

	return lines.join(" - ");
}
