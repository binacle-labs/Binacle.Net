import {faceColors, KINDS, printColor, RAMP_STEPS} from "./colors";

export default interface Palette {
	ground: string;
	binLine: string;
	itemLine: string;
	ramp: string[][];
	rest: string[];
	kinds: string[][];
	prints: string[];
}

function steps(count: number, read: (step: number) => string[]): string[][] {
	return Array.from({length: count}, (_, index) => read(index + 1));
}

// The tokens differ per theme, so this runs again whenever the theme changes.
export function readPalette(element: Element): Palette {
	const style = getComputedStyle(element);
	const token = (name: string) => style.getPropertyValue(name).trim();
	const ground = token("--ground");
	return {
		ground,
		binLine: token("--bin-line"),
		itemLine: token("--item-line") || ground,
		ramp: steps(RAMP_STEPS, step => faceColors(token(`--item-${step}`))),
		rest: faceColors(token("--item-rest")),
		kinds: steps(KINDS, step => faceColors(token(`--kind-${step}`))),
		prints: Array.from({length: KINDS}, (_, index) => printColor(token(`--kind-${index + 1}`)))
	};
}
