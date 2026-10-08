import Coordinates from "./coordinates";
import Dimensions from "./dimensions";

export default interface SceneData {
	bin: Dimensions | null,
	items: (Dimensions & Coordinates)[]
}

