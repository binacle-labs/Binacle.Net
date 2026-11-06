import {scenePosition} from "../../../src/components/packView/scenePosition";

describe("scenePosition", () => {
	const bin = {length: 10, width: 20, height: 30};

	test("an item at the bin's corner sits against its three walls", () => {
		const item = {length: 2, width: 4, height: 6, x: 0, y: 0, z: 0};

		expect(scenePosition(bin, item)).toEqual({x: -4, y: -12, z: -8});
	});

	// The API's z is up and its y is depth; the scene has them the other way round.
	test("takes the API's z for the scene's y and the API's y for the scene's z", () => {
		const item = {length: 2, width: 4, height: 6, x: 1, y: 2, z: 3};

		expect(scenePosition(bin, item)).toEqual({x: -3, y: -9, z: -6});
	});

	test("an item filling the bin is centred on the origin", () => {
		const item = {...bin, x: 0, y: 0, z: 0};

		expect(scenePosition(bin, item)).toEqual({x: 0, y: 0, z: 0});
	});
});
