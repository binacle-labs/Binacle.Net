export const RAMP_STEPS = 6;
export const CATEGORIES = 6;

const TOP_LIFT = 0.08;
const RIGHT_DROP = 0.09;
const UNDER_DROP = 0.16;
const PRINT_DROP = 0.3;

export function parseOklch(value: string): [number, number, number] {
	const numbers = value.match(/[\d.]+/g);
	if (!numbers || numbers.length < 3) {
		throw new Error(`Expected an oklch() value, got "${value}"`);
	}
	return [Number(numbers[0]), Number(numbers[1]), Number(numbers[2])];
}

// three cannot read oklch(), so the tokens become hex here.
export function oklchToHex(l: number, c: number, h: number): string {
	const a = c * Math.cos(h * Math.PI / 180);
	const b = c * Math.sin(h * Math.PI / 180);
	const long = (l + 0.3963377774 * a + 0.2158037573 * b) ** 3;
	const medium = (l - 0.1055613458 * a - 0.0638541728 * b) ** 3;
	const short = (l - 0.0894841775 * a - 1.2914855480 * b) ** 3;
	const linear = [
		4.0767416621 * long - 3.3077115913 * medium + 0.2309699292 * short,
		-1.2684380046 * long + 2.6097574011 * medium - 0.3413193965 * short,
		-0.0041960863 * long - 0.7034186147 * medium + 1.7076147010 * short
	];
	const channels = linear.map(value => {
		const clamped = Math.min(1, Math.max(0, value));
		const srgb = clamped <= 0.0031308 ? 12.92 * clamped : 1.055 * clamped ** (1 / 2.4) - 0.055;
		return ("0" + Math.round(srgb * 255).toString(16)).slice(-2);
	});
	return "#" + channels.join("");
}

// In BoxGeometry's group order: +x, -x, +y, -y, +z, -z. Hidden faces repeat the visible ones.
export function faceColors(token: string): string[] {
	const [l, c, h] = parseOklch(token);
	const top = oklchToHex(Math.min(1, l + TOP_LIFT), c, h);
	const left = oklchToHex(l, c, h);
	const right = oklchToHex(l - RIGHT_DROP, c, h);
	const under = oklchToHex(l - UNDER_DROP, c, h);
	return [right, right, top, under, left, left];
}

export function printColor(token: string): string {
	const [l, c, h] = parseOklch(token);
	return oklchToHex(l - PRINT_DROP, c, h);
}

export function rampStep(index: number, count: number): number {
	return count <= 1 ? 0 : Math.round(index * (RAMP_STEPS - 1) / (count - 1));
}

// At rest every item is in colour; while stepping only the last one placed is. A lit item type overrides both.
export function isInColor(index: number, itemType: string | undefined, shown: number, count: number, lit: string | null): boolean {
	if (lit) {
		return itemType === lit;
	}
	return shown === count || index === shown - 1;
}
