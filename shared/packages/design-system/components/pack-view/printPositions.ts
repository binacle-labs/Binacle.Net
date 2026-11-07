import {PRINTS} from "./prints";

type Point = [number, number];
type Segment = [Point, Point];
type Vector = [number, number, number];

interface Face {
	center: Vector;
	right: Vector;
	up: Vector;
	normal: Vector;
	width: number;
	height: number;
}

const BOX = 10;
const CURVE_STEPS = 8;
const FILL = 0.78;
// Just off the face, or the face hides the line.
const LIFT = 0.02;

// Paths open with M and use absolute M, L, C and Z only.
export function pathSegments(path: string): Segment[] {
	const tokens = path.match(/[MLCZ]|-?[\d.]+/g) ?? [];
	const segments: Segment[] = [];
	let index = 0;
	let current: Point = [0, 0];
	let start: Point = current;
	const point = (): Point => [Number(tokens[index++]), Number(tokens[index++])];
	const lineTo = (next: Point) => {
		segments.push([current, next]);
		current = next;
	};

	while (index < tokens.length) {
		const command = tokens[index++];
		if (command === "M") {
			current = start = point();
		} else if (command === "L") {
			lineTo(point());
		} else if (command === "C") {
			const from = current;
			const first = point();
			const second = point();
			const end = point();
			for (let step = 1; step <= CURVE_STEPS; step++) {
				const u = step / CURVE_STEPS;
				const v = 1 - u;
				const along = (axis: 0 | 1) =>
					v ** 3 * from[axis] + 3 * v * v * u * first[axis] + 3 * v * u * u * second[axis] + u ** 3 * end[axis];
				lineTo([along(0), along(1)]);
			}
		} else if (command === "Z") {
			lineTo(start);
		}
	}
	return segments;
}

const SEGMENTS_BY_ITEM_TYPE = new Map<string, Segment[]>(
	Object.values(PRINTS).map(print => [
		print.itemType,
		print.paths.reduce<Segment[]>((all, path) => all.concat(pathSegments(path)), [])
	])
);

// Opposite faces sit side by side, so a pair starts at an even index. The sides are seen from outside with up as
// up; the top has its picture along the length, its top edge at the back, and the bottom mirrors it.
function boxFaces(length: number, height: number, width: number): Face[] {
	return [
		{center: [0, 0, width / 2], right: [1, 0, 0], up: [0, 1, 0], normal: [0, 0, 1], width: length, height},
		{center: [0, 0, -width / 2], right: [-1, 0, 0], up: [0, 1, 0], normal: [0, 0, -1], width: length, height},
		{center: [length / 2, 0, 0], right: [0, 0, -1], up: [0, 1, 0], normal: [1, 0, 0], width, height},
		{center: [-length / 2, 0, 0], right: [0, 0, 1], up: [0, 1, 0], normal: [-1, 0, 0], width, height},
		{center: [0, height / 2, 0], right: [1, 0, 0], up: [0, 0, -1], normal: [0, 1, 0], width: length, height: width},
		{center: [0, -height / 2, 0], right: [1, 0, 0], up: [0, 0, 1], normal: [0, -1, 0], width: length, height: width}
	];
}

// On the smaller faces the picture shrinks to noise.
export function largestPair(faces: Face[]): Face[] {
	const area = (index: number) => faces[index].width * faces[index].height;
	const [first] = [0, 2, 4].sort((a, b) => area(b) - area(a));
	return [faces[first], faces[first + 1]];
}

// Scene sides, not API sides: the scene's y is up, so height comes before width. Null for an item type with no picture.
export function printPositions(itemType: string, length: number, height: number, width: number): number[] | null {
	const segments = SEGMENTS_BY_ITEM_TYPE.get(itemType);
	if (!segments) {
		return null;
	}
	const positions: number[] = [];
	for (const face of largestPair(boxFaces(length, height, width))) {
		const scale = Math.min(face.width, face.height) / BOX * FILL;
		for (const segment of segments) {
			for (const [px, py] of segment) {
				for (let axis = 0; axis < 3; axis++) {
					positions.push(
						face.center[axis]
						+ face.right[axis] * (px - BOX / 2) * scale
						+ face.up[axis] * (py - BOX / 2) * scale
						+ face.normal[axis] * LIFT
					);
				}
			}
		}
	}
	return positions;
}
