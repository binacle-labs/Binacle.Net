import Box from "./box";

export default class Bin extends Box {
	public copy: number;

	constructor(length: number, width: number, height: number, copy: number = 0) {
		super(length, width, height);
		this.copy = copy;
	}

	get id() {
		return this.copy > 0 ? `${super.id} (${this.copy})` : super.id;
	}

	// The API rejects duplicate ids. Highest plus one, not the count, so removing a bin cannot hand out
	// a number that is still in use.
	static copyOf(bins: Bin[], bin: Bin): Bin {
		const highestCopy = bins
			.filter(x => x.length === bin.length && x.width === bin.width && x.height === bin.height)
			.reduce((highest, x) => Math.max(highest, x.copy), 0);
		return new Bin(bin.length, bin.width, bin.height, highestCopy + 1);
	}
}
