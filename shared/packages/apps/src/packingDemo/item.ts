import Box from "./box";

export default class Item extends Box {
	public quantity: number;
	constructor(length: number | null, width: number | null, height: number | null, quantity: number) {
		super(length, width, height);
		this.quantity = quantity;
	}

	get id() {
		return `${super.id}-${this.quantity}`;
	}

	getDimensions() {
		const dimensions = super.getDimensions();
		dimensions.push({name: 'Quantity', value: Number(this.quantity)});
		return dimensions;
	}

}

