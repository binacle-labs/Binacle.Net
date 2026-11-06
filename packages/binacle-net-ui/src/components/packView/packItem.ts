import Coordinates from "../../shared/coordinates";
import Dimensions from "../../shared/dimensions";

export default interface PackItem extends Dimensions, Coordinates {
	id?: string;
	kind?: number;
}
