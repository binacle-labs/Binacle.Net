import Dictionary from "../shared/dictionary";

export default class ErrorCollection {
	private fields: Dictionary<string[]> = {};

	constructor() {

	}

	push(fieldName: string, message: string) {
		this.fields[fieldName] = this.fields[fieldName] || [];
		this.fields[fieldName].push(message);
	}

	get errorMessages() {
		const messages = [] as string[];
		for (const fieldName in this.fields) {
			for (const errorMessage of this.fields[fieldName]) {
				messages.push(errorMessage);
			}
		}
		return messages;
	}

	hasError(fieldName: string) {
		return (this.fields[fieldName]?.length ?? 0) > 0;
	}

	hasErrors() {
		for (const fieldName in this.fields) {
			if (this.fields[fieldName].length > 0) {
				return true;
			}
		}
		return false;
	}

}
