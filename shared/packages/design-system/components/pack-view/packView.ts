import {BoxGeometry, BufferGeometry, Color, EdgesGeometry, Mesh, MeshBasicMaterial, Object3D, PerspectiveCamera, Scene, WebGLRenderer} from "three";
import {OrbitControls} from "three/examples/jsm/controls/OrbitControls.js";
import {LineMaterial} from "three/examples/jsm/lines/LineMaterial.js";
import {LineSegments2} from "three/examples/jsm/lines/LineSegments2.js";
import {LineSegmentsGeometry} from "three/examples/jsm/lines/LineSegmentsGeometry.js";
import Dimensions from "./dimensions";
import {fitDistance, lensFor, START_DIRECTION} from "./cameraFit";
import {isInColor, rampStep} from "./colors";
import {ICONS} from "./icons";
import PackItem from "./packItem";
import Palette, {readPalette} from "./palette";
import {printPositions} from "./printPositions";
import {scenePosition} from "./scenePosition";

const PLAY_MS = 1000;
const PLAY_LABEL = "Play the placement order";
const STOP_LABEL = "Stop";

export interface ShowOptions {
	kindNames?: string[];
	note?: string;
}

export interface PackView {
	preview(bin: Dimensions): void;
	show(bin: Dimensions, items: PackItem[], options?: ShowOptions): void;
	wait(bin: Dimensions | null): void;
	highlight(kind: number | null): void;
	clear(): void;
}

interface DrawnItem {
	mesh: Mesh<BoxGeometry, MeshBasicMaterial[]>;
	outline: LineSegments2;
	print: LineSegments2 | null;
}

type ControlName = "first" | "previous" | "play" | "next" | "last";

// Drawn as triangles, not GL lines: WebGL ignores a GL line's width, and a depth offset does not reach it.
function edges(geometry: BufferGeometry, color: string, offset = 0): LineSegments2 {
	const lines = new LineSegmentsGeometry().fromEdgesGeometry(new EdgesGeometry(geometry));
	return new LineSegments2(lines, new LineMaterial({
		color,
		linewidth: 1,
		polygonOffset: offset > 0,
		polygonOffsetFactor: offset,
		polygonOffsetUnits: offset
	}));
}

function printLines(kind: number, item: PackItem, color: string): LineSegments2 | null {
	const positions = printPositions(kind, item.length, item.height, item.width);
	if (!positions) {
		return null;
	}
	const geometry = new LineSegmentsGeometry();
	geometry.setPositions(positions);
	return new LineSegments2(geometry, new LineMaterial({color, linewidth: 1}));
}

function createRenderer(stage: Element): WebGLRenderer | null {
	try {
		return new WebGLRenderer({antialias: true});
	} catch {
		const note = document.createElement("p");
		note.className = "pack-view__note";
		note.textContent = "The 3D view needs WebGL, and this browser has it off.";
		stage.append(note);
		return null;
	}
}

// The figure's markup is the host's: this finds its parts by their data attributes and fills them.
export function createPackView(figure: HTMLElement): PackView {
	const stage = figure.querySelector<HTMLElement>("[data-pack-view-stage]")!;
	const renderer = createRenderer(stage);
	if (!renderer) {
		const nothing = () => {
			// Empty on purpose: the note in the stage has said why.
		};
		return {show: nothing, preview: nothing, wait: nothing, highlight: nothing, clear: nothing};
	}
	return createScene(figure, stage, renderer);
}

function createScene(figure: HTMLElement, stage: HTMLElement, renderer: WebGLRenderer): PackView {
	const empty = figure.querySelector<HTMLElement>("[data-pack-view-empty]");
	const binDim = figure.querySelector<HTMLElement>("[data-bin-dim]");
	const placed = figure.querySelector<HTMLElement>("[data-placed]");
	const controlsBar = figure.querySelector<HTMLElement>("[data-pack-view-controls]");

	const canvas = renderer.domElement;

	const scene = new Scene();
	const camera = new PerspectiveCamera(40, 1, 1, 1000);
	renderer.setPixelRatio(window.devicePixelRatio);
	canvas.classList.add("pack-view__canvas");
	stage.prepend(canvas);

	const loader = document.createElement("div");
	loader.className = "pack-view__wait";
	loader.setAttribute("aria-hidden", "true");
	stage.append(loader);

	const orbit = new OrbitControls(camera, canvas);
	// Keeps the camera above the floor.
	orbit.maxPolarAngle = Math.PI / 2 - 0.05;
	orbit.enablePan = false;
	orbit.addEventListener("change", render);
	// The wheel scrolls the page, so zoom needs Ctrl or Cmd. A phone has neither, so two fingers zoom there, and a
	// vertical swipe stays the page's.
	orbit.enableZoom = false;
	canvas.addEventListener("wheel", event => {
		orbit.enableZoom = event.ctrlKey || event.metaKey;
	}, {capture: true});
	canvas.addEventListener("pointerdown", event => {
		if (event.pointerType === "touch") {
			orbit.enableZoom = true;
		}
	}, {capture: true});
	canvas.style.touchAction = "pan-y";

	let palette: Palette;
	let bin: Dimensions | null = null;
	let items: PackItem[] = [];
	let drawn: DrawnItem[] = [];
	let binLines: LineSegments2 | null = null;
	let shown = 0;
	let timer: number | null = null;
	let kindNames: string[] = [];
	let note = "";
	let lit: number | null = null;
	let waiting = false;

	function readColors() {
		palette = readPalette(figure);
		renderer.setClearColor(new Color(palette.ground));
	}

	function render() {
		renderer.render(scene, camera);
	}

	function resize() {
		const width = stage.clientWidth;
		const height = stage.clientHeight;
		if (!width || !height) {
			return;
		}
		renderer.setSize(width, height, false);
		camera.aspect = width / height;
		camera.fov = lensFor(camera.aspect);
		camera.updateProjectionMatrix();
		frame();
		render();
	}

	function frame() {
		if (!bin) {
			return;
		}
		const distance = fitDistance(bin, camera.fov, camera.aspect);
		camera.position.copy(START_DIRECTION).multiplyScalar(distance);
		camera.near = distance / 50;
		camera.far = distance * 4;
		camera.lookAt(0, 0, 0);
		orbit.target.set(0, 0, 0);
		orbit.minDistance = distance * 0.5;
		orbit.maxDistance = distance * 2.5;
		orbit.update();
		camera.updateProjectionMatrix();
	}

	function dispose(object: Object3D) {
		scene.remove(object);
		object.traverse(child => {
			const {geometry, material} = child as Mesh;
			geometry?.dispose();
			[material ?? []].flat().forEach(each => each.dispose());
		});
	}

	function drawBin() {
		if (binLines) {
			dispose(binLines);
		}
		binLines = null;
		if (!bin) {
			return;
		}
		// Offset behind the item faces, so an item against a wall hides the bin's line instead of being combed by it.
		binLines = edges(new BoxGeometry(bin.length, bin.height, bin.width), palette.binLine, 2);
		scene.add(binLines);
	}

	function drawItem(item: PackItem): DrawnItem {
		const geometry = new BoxGeometry(item.length, item.height, item.width);
		const materials = Array.from({length: 6}, () => new MeshBasicMaterial({
			polygonOffset: true,
			polygonOffsetFactor: 1,
			polygonOffsetUnits: 1
		}));
		const mesh = new Mesh(geometry, materials);
		const position = scenePosition(bin!, item);
		mesh.position.set(position.x, position.y, position.z);

		const outline = edges(geometry, palette.itemLine);
		mesh.add(outline);
		const print = item.kind ? printLines(item.kind, item, palette.prints[item.kind - 1]) : null;
		if (print) {
			mesh.add(print);
		}
		return {mesh, outline, print};
	}

	function paint() {
		drawn.forEach(({mesh, outline, print}, index) => {
			const {kind} = items[index];
			const own = kind ? palette.kinds[kind - 1] : palette.ramp[rampStep(index, items.length)];
			const inColor = isInColor(index, kind, shown, items.length, lit);
			const faces = inColor ? own : palette.rest;
			mesh.material.forEach((material, face) => material.color.set(faces[face]));
			outline.material.color.set(palette.itemLine);
			if (print) {
				print.visible = inColor;
				print.material.color.set(palette.prints[kind! - 1]);
			}
		});
	}

	function setShown(count: number) {
		const target = Math.max(0, Math.min(items.length, count));
		while (drawn.length > target) {
			dispose(drawn.pop()!.mesh);
		}
		while (drawn.length < target) {
			const item = drawItem(items[drawn.length]);
			drawn.push(item);
			scene.add(item.mesh);
		}
		shown = target;
		paint();
		readout();
		render();
	}

	// Built from elements, not markup: an item's id is whatever the user typed.
	function itemReadout(element: HTMLElement) {
		const item = items[shown - 1];
		const name = item.id || (item.kind ? kindNames[item.kind - 1] : "");

		const count = document.createElement("span");
		count.className = "pack-view__count";
		count.textContent = `Item ${shown} of ${items.length}`;

		const size = document.createElement("span");
		size.className = "pack-view__placed";
		size.textContent = `${item.length} x ${item.width} x ${item.height} (${item.x}, ${item.y}, ${item.z})`;

		const detail = document.createElement("span");
		detail.append(name ? `${name}, ` : "", size);

		element.replaceChildren(count, " ", detail);
	}

	function readout() {
		if (binDim) {
			binDim.hidden = !bin;
			const number = binDim.querySelector("[data-bin-dim-number]");
			if (bin && number) {
				number.textContent = `${bin.length} x ${bin.width} x ${bin.height}`;
			}
		}
		if (placed) {
			if (waiting) {
				placed.textContent = "Packing";
			} else if (!items.length) {
				placed.textContent = note || empty?.textContent || "";
			} else if (shown === 0) {
				placed.textContent = `Empty. ${items.length} items to place`;
			} else {
				itemReadout(placed);
			}
		}
		updateControls();
	}

	function labelControl(control: HTMLButtonElement, icon: string, label: string) {
		control.innerHTML = icon;
		control.setAttribute("aria-label", label);
		control.title = label;
	}

	function createControl(bar: HTMLElement, icon: string, label: string, action: () => void) {
		const control = document.createElement("button");
		control.type = "button";
		control.className = "pack-view__control";
		labelControl(control, icon, label);
		control.addEventListener("click", action);
		bar.append(control);
		return control;
	}

	function stop() {
		if (timer !== null) {
			window.clearInterval(timer);
		}
		timer = null;
		if (controls) {
			labelControl(controls.play, ICONS.play, PLAY_LABEL);
		}
	}

	function play() {
		if (timer !== null) {
			stop();
			updateControls();
			return;
		}
		// Goes on from where it stopped, and starts over only when every item is in.
		if (shown >= items.length) {
			setShown(0);
		}
		if (controls) {
			labelControl(controls.play, ICONS.stop, STOP_LABEL);
		}
		timer = window.setInterval(() => {
			if (shown >= items.length) {
				stop();
				updateControls();
				return;
			}
			setShown(shown + 1);
		}, PLAY_MS);
		updateControls();
	}

	function updateControls() {
		if (!controls) {
			return;
		}
		const none = !items.length;
		const playing = timer !== null;
		controls.first.disabled = none || playing || shown === 0;
		controls.previous.disabled = none || playing || shown === 0;
		controls.next.disabled = none || playing || shown === items.length;
		controls.last.disabled = none || playing || shown === items.length;
		controls.play.disabled = none;
	}

	const controls: Record<ControlName, HTMLButtonElement> | null = controlsBar && {
		first: createControl(controlsBar, ICONS.first, "Empty bin", () => setShown(0)),
		previous: createControl(controlsBar, ICONS.previous, "Take the last item out", () => setShown(shown - 1)),
		play: createControl(controlsBar, ICONS.play, PLAY_LABEL, play),
		next: createControl(controlsBar, ICONS.next, "Place the next item", () => setShown(shown + 1)),
		last: createControl(controlsBar, ICONS.last, "Every item", () => setShown(items.length))
	};

	if (controls) {
		figure.tabIndex = 0;
		figure.addEventListener("keydown", event => {
			if (!items.length || event.target !== figure) {
				return;
			}
			const keys: Record<string, () => void> = {
				ArrowLeft: () => setShown(shown - 1),
				ArrowRight: () => setShown(shown + 1),
				Home: () => setShown(0),
				End: () => setShown(items.length),
				" ": play
			};
			const action = keys[event.key];
			if (!action || (timer !== null && event.key !== " ")) {
				return;
			}
			event.preventDefault();
			action();
		});
	}

	function retheme() {
		readColors();
		binLines?.material.color.set(palette.binLine);
		paint();
		render();
	}

	new MutationObserver(retheme).observe(document.documentElement, {attributes: true, attributeFilter: ["data-theme"]});
	matchMedia("(prefers-color-scheme: dark)").addEventListener("change", retheme);
	new ResizeObserver(resize).observe(stage);

	readColors();
	resize();
	readout();

	function load(newBin: Dimensions | null, newItems: PackItem[], count: number, options: ShowOptions = {}) {
		stop();
		waiting = false;
		figure.classList.remove("pack-view--waiting");
		kindNames = options.kindNames ?? [];
		note = options.note ?? "";
		lit = null;
		drawn.forEach(({mesh}) => dispose(mesh));
		drawn = [];
		bin = newBin;
		items = newItems;
		shown = 0;
		if (empty) {
			empty.hidden = items.length > 0;
		}
		figure.classList.toggle("pack-view--empty", !items.length);
		drawBin();
		frame();
		setShown(count);
	}

	return {
		show(newBin, newItems, options) {
			load(newBin, newItems, newItems.length, options);
		},
		highlight(kind) {
			lit = kind || null;
			paint();
			render();
		},
		preview(newBin) {
			load(newBin, [], 0);
		},
		wait(newBin) {
			load(newBin, [], 0);
			waiting = true;
			figure.classList.add("pack-view--waiting");
			figure.classList.remove("pack-view--empty");
			readout();
		},
		clear() {
			load(null, [], 0);
		}
	};
}
