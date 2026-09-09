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

// The rate limiter answers 429 with no body at all, so response.json() would throw on an empty stream.
// Read the text first: no body means `problem: null`. A body that is there and does not parse still throws,
// which is a different fault and stays one.
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
