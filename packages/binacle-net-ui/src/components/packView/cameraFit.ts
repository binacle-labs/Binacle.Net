import {Vector3} from "three";
import Dimensions from "../../shared/dimensions";

export const START_DIRECTION = new Vector3(1, 0.9, 1.25).normalize();

const UP = new Vector3(0, 1, 0);
const TURN_STEPS = 24;
const MARGIN = 1.03;

// A narrow lens, so a tall bin does not taper.
export function lensFor(aspect: number): number {
	if (aspect < 0.6) {
		return 40;
	}
	return aspect < 1 ? 34 : 28;
}

// Fits the eight corners, not a bounding sphere, so a long or flat bin fills the view. It holds all the way round
// at the starting height, so turning the bin never cuts it.
export function fitDistance(bin: Dimensions, fov: number, aspect: number): number {
	const tanVertical = Math.tan(fov * Math.PI / 360);
	const tanHorizontal = tanVertical * aspect;
	const elevation = Math.asin(START_DIRECTION.y);
	const azimuth = Math.atan2(START_DIRECTION.x, START_DIRECTION.z);

	const corners: Vector3[] = [];
	for (const x of [-bin.length / 2, bin.length / 2]) {
		for (const y of [-bin.height / 2, bin.height / 2]) {
			for (const z of [-bin.width / 2, bin.width / 2]) {
				corners.push(new Vector3(x, y, z));
			}
		}
	}

	let distance = 0;
	for (let step = 0; step < TURN_STEPS; step++) {
		const turn = azimuth + step * 2 * Math.PI / TURN_STEPS;
		const back = new Vector3(
			Math.cos(elevation) * Math.sin(turn),
			Math.sin(elevation),
			Math.cos(elevation) * Math.cos(turn)
		);
		const right = UP.clone().cross(back).normalize();
		const up = back.clone().cross(right);
		for (const corner of corners) {
			const depth = corner.dot(back);
			distance = Math.max(
				distance,
				depth + Math.abs(corner.dot(right)) / tanHorizontal,
				depth + Math.abs(corner.dot(up)) / tanVertical
			);
		}
	}
	return distance * MARGIN;
}
