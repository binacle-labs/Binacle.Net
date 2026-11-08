import ErrorCollection from "./errorCollection";


// Null is a field not filled in yet: a row added by hand starts blank.
export default class Box {
	static minAllowedDimension = 1;
	static maxAllowedDimension = 65535;

	public length: number | null;
	public width: number | null;
	public height: number | null;

	constructor(length: number | null, width: number | null, height: number | null) {
		this.length = length;
		this.width = width;
		this.height = height;
	}

	public get size(): string {
		return `${this.length}x${this.width}x${this.height}`;
	}

	isFieldValid(fieldName: string): boolean {
		const errors = this.errorState;
		if (fieldName) {
			return !errors.hasError(fieldName);
		}
		return true;
	}

	hasErrors(): boolean {
		const errors = this.errorState;
		return errors.hasErrors();
	}

	get allErrorMessages(): string[] {
		const errors = this.errorState;
		return errors.errorMessages;
	}

	// `field` is the property the input binds to; `label` starts the message.
	getDimensions() {
		return [
			{field: 'length', label: 'Length', value: Number(this.length)},
			{field: 'width', label: 'Width', value: Number(this.width)},
			{field: 'height', label: 'Height', value: Number(this.height)},
		];
	}

	get errorState(): ErrorCollection {
		const errors = new ErrorCollection();

		const dimensions = this.getDimensions();

		dimensions.forEach(({field, label, value}) => {
			if (value === null || value === undefined) {
				errors.push(field, `${label} is required`);
			}

			if (Number.isNaN(value)) {
				errors.push(field, `${label} must be a number`);
			}

			if (!Number.isInteger(value)) {
				errors.push(field, `${label} must be an integer`);
			}

			if (value < Box.minAllowedDimension || value > Box.maxAllowedDimension) {
				errors.push(
					field,
					`${label} must be between ${Box.minAllowedDimension} and ${Box.maxAllowedDimension}`,
				);
			}
		});

		return errors;
	}
}
