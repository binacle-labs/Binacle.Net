import {HttpValidationProblemDetails, ProblemDetails} from "./types";

// What the API answers with on a 4xx or 5xx. 422 carries the field errors, everything else does not.
export type ApiProblem = ProblemDetails | HttpValidationProblemDetails;

export interface ApiSuccess<T> {
	ok: true;
	status: number;
	data: T;
}

export interface ApiFailure {
	ok: false;
	status: number;
	// null when the response carried no body at all, which is not the same as a body that failed to parse.
	problem: ApiProblem | null;
}

// Every call answers one of these. Branch on `ok` and TypeScript narrows to the right half.
export type ApiResponse<T> = ApiSuccess<T> | ApiFailure;

// True when the failure lists per-field validation errors, so `problem.errors` is safe to read. Only 422
// carries them.
export function hasValidationErrors(problem: ApiProblem | null): problem is HttpValidationProblemDetails {
	const errors = (problem as HttpValidationProblemDetails | null)?.errors;
	return errors !== null && errors !== undefined;
}

// 401, 403, 404, 409 and 429 come back with no body, so response.json() would throw on an empty stream.
// Read the text first: no body means `problem: null`.
export async function readApiResponse<T>(response: Response): Promise<ApiResponse<T>> {
	const text = await response.text();

	if (!response.ok) {
		return {
			ok: false,
			status: response.status,
			problem: text.length === 0 ? null : JSON.parse(text) as ApiProblem,
		};
	}

	if (text.length === 0)
		throw new Error(`Binacle.Net answered ${response.status} with an empty body.`);

	return {ok: true, status: response.status, data: JSON.parse(text) as T};
}

// The create, update, patch and delete routes answer 201 or 204 with no body, so here no body on a success
// is the expected shape.
export async function readEmptyResponse(response: Response): Promise<ApiResponse<void>> {
	const text = await response.text();

	if (!response.ok) {
		return {
			ok: false,
			status: response.status,
			problem: text.length === 0 ? null : JSON.parse(text) as ApiProblem,
		};
	}

	return {ok: true, status: response.status, data: undefined};
}
