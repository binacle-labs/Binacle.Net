function icon(path: string): string {
	return `<svg class="pack-view__icon" viewBox="0 0 16 16" width="16" height="16" aria-hidden="true"><path d="${path}"/></svg>`;
}

export const ICONS = {
	first: icon("M4 3v10M12 3L6 8l6 5"),
	previous: icon("M10 3L5 8l5 5"),
	play: icon("M5 3l8 5-8 5z"),
	stop: icon("M4.5 4.5h7v7h-7z"),
	next: icon("M6 3l5 5-5 5"),
	last: icon("M12 3v10M4 3l6 5-6 5")
};
