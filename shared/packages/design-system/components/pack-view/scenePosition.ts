import Coordinates from "./coordinates";
import Dimensions from "./dimensions";
import PackItem from "./packItem";

// The API's y is depth and z is up; the scene's y is up. The bin sits centred on the origin.
export function scenePosition(bin: Dimensions, item: PackItem): Coordinates {
	return {
		x: -bin.length / 2 + item.length / 2 + item.x,
		y: -bin.height / 2 + item.height / 2 + item.z,
		z: -bin.width / 2 + item.width / 2 + item.y
	};
}
