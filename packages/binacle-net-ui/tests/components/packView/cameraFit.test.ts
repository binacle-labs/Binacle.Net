import {PerspectiveCamera, Vector3} from "three";
import {fitDistance, lensFor, START_DIRECTION} from "../../../src/components/packView/cameraFit";
import Dimensions from "../../../src/shared/dimensions";

function corners(bin: Dimensions): Vector3[] {
	const all: Vector3[] = [];
	for (const x of [-1, 1]) {
		for (const y of [-1, 1]) {
			for (const z of [-1, 1]) {
				all.push(new Vector3(x * bin.length / 2, y * bin.height / 2, z * bin.width / 2));
			}
		}
	}
	return all;
}

// The largest distance of any corner from the centre of the picture. Past 1 it is cut off.
function reach(bin: Dimensions, aspect: number, turn: number, scale = 1): number {
	const fov = lensFor(aspect);
	const camera = new PerspectiveCamera(fov, aspect, 0.1, 100000);
	const distance = fitDistance(bin, fov, aspect) * scale;
	camera.position.copy(START_DIRECTION).multiplyScalar(distance).applyAxisAngle(new Vector3(0, 1, 0), turn);
	camera.lookAt(0, 0, 0);
	camera.updateMatrixWorld();
	return Math.max(...corners(bin).map(corner => {
		const {x, y} = corner.clone().project(camera);
		return Math.max(Math.abs(x), Math.abs(y));
	}));
}

describe("lensFor", () => {
	test.each([
		[0.5, 40],
		[0.6, 34],
		[0.99, 34],
		[1, 28],
		[1.8, 28]
	])("aspect %d takes %d degrees", (aspect, fov) => {
		expect(lensFor(aspect)).toBe(fov);
	});
});

describe("fitDistance", () => {
	const bins = [
		{length: 60, width: 40, height: 30},
		{length: 200, width: 10, height: 10},
		{length: 10, width: 10, height: 200},
		{length: 100, width: 100, height: 2}
	];
	const aspects = [0.5, 0.8, 1, 16 / 9];
	const cases = bins.flatMap(bin => aspects.map(aspect => [bin, aspect] as const));

	test.each(cases)("%j at aspect %d holds every corner all the way round", (bin, aspect) => {
		for (let degree = 0; degree < 360; degree += 3) {
			expect(reach(bin, aspect, degree * Math.PI / 180)).toBeLessThanOrEqual(1);
		}
	});

	// Without this, a fit that always returned a huge distance would pass.
	test.each(cases)("%j at aspect %d is close: at two thirds of it a corner is cut", (bin, aspect) => {
		const widest = Math.max(...Array.from({length: 120}, (_, step) =>
			reach(bin, aspect, step * 3 * Math.PI / 180, 2 / 3)));

		expect(widest).toBeGreaterThan(1);
	});

	test("scales with the bin", () => {
		const small = fitDistance({length: 6, width: 4, height: 3}, 28, 1.5);
		const large = fitDistance({length: 60, width: 40, height: 30}, 28, 1.5);

		expect(large).toBeCloseTo(small * 10);
	});
});
