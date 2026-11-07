import {ApiFailure, hasValidationErrors} from "@binacle/api-client";
import ErrorViewModel from "./error";
import {getResponseStatusText} from "./getResponseStatusText";

// What the dialog shows for a failed API call.
export function errorFromFailure(failure: ApiFailure): ErrorViewModel {
	const error: ErrorViewModel = {
		title: `Error: ${getResponseStatusText(failure.status)}`,
		errors: []
	};

	const problem = failure.problem;
	if (problem === null) {
		// An empty body means the server sent no details, not that a body failed to parse - a
		// response that fails to parse throws before it reaches this branch.
		if (failure.status === 429) {
			error.errors.push('You have been rate limited. Wait a moment, then try again.');
		} else {
			error.errors.push('The server did not send any further details about this error.');
		}
		return error;
	}

	if (problem.title) {
		error.title = problem.title;
	}
	if (problem.detail) {
		error.errors.push(problem.detail);
	}
	if (failure.status === 422 && hasValidationErrors(problem)) {
		const errorsByField = problem.errors ?? {};
		for (const key in errorsByField) {
			errorsByField[key].forEach((fieldError: string) => {
				error.errors.push(`${key}: ${fieldError}`);
			});
		}
	}
	return error;
}
