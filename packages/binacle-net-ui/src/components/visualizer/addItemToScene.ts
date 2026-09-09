import Coordinates from "../../shared/coordinates";
import Dimensions from "../../shared/dimensions";
import {getBinOrigin} from "./getBinOrigin";
import {createItem} from "./createItem";
import {Scene} from "three";
import {itemMaterial} from "./itemMaterial";

export function addItemToScene(scene: Scene, bin: Dimensions, packedItem: Dimensions & Coordinates, index: number) {
	const binOrigin = getBinOrigin(bin);

	let item = createItem(packedItem, index, itemMaterial, binOrigin);

	scene.add(item);
}
