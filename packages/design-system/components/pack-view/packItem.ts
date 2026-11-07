import Coordinates from "./coordinates";
import Dimensions from "./dimensions";

export default interface PackItem extends Dimensions, Coordinates {
	id?: string;
	kind?: number;
}
