import Box from "./box";

export default class Item extends Box {
	public quantity: number;
	constructor(length: number | null, width: number | null, height: number | null, quantity: number) {
		super(length, width, height);
		this.quantity = quantity;
	}

	getDimensions() {
		const dimensions = super.getDimensions();
		dimensions.push({field: 'quantity', label: 'Quantity', value: Number(this.quantity)});
		return dimensions;
	}

}

