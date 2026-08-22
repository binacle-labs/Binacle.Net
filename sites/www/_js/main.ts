// The whole of this site's JavaScript. Two behaviours, both degrading cleanly, and it loads deferred.
// The pre-paint theme read is not here - it has to run before first paint, so it is inlined in
// _includes/theme-init.html.

type Theme = "light" | "dark";

const STORAGE_KEY = "theme";
const root = document.documentElement;

// Old browsers have no matchMedia at all. The typeof check is what tells TypeScript that, since it types
// the property as always present.
function hasMatchMedia(): boolean {
	return typeof window.matchMedia === "function";
}

function systemTheme(): Theme {
	return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

function currentTheme(): Theme {
	const set = root.getAttribute("data-theme");
	if (set === "light" || set === "dark") return set;
	// Nothing stored, so the stylesheet is following the system. Fall back to the site default only when
	// the browser reports no preference at all.
	if (hasMatchMedia()) return systemTheme();
	return (root.getAttribute("data-default-theme") as Theme) || "dark";
}

function applyTheme(theme: Theme, button: HTMLButtonElement): void {
	root.setAttribute("data-theme", theme);
	try {
		localStorage.setItem(STORAGE_KEY, theme);
	} catch {
		// Private windows throw on write. The choice still applies for this page load.
	}
	button.setAttribute("aria-pressed", String(theme === "dark"));
	button.setAttribute("aria-label", theme === "dark" ? "Switch to the light theme" : "Switch to the dark theme");
	const label = button.querySelector("[data-theme-label]");
	if (label) label.textContent = theme === "dark" ? "Light" : "Dark";
}

function wireThemeToggle(): void {
	const button = document.querySelector<HTMLButtonElement>(".theme-toggle");
	if (!button) return;
	// Revealed here, not in the markup: with JavaScript off the toggle cannot work, and a dead control is
	// worse than no control.
	button.hidden = false;
	applyTheme(currentTheme(), button);
	button.addEventListener("click", () => {
		applyTheme(currentTheme() === "dark" ? "light" : "dark", button);
	});
}

// The docker run line is the primary conversion on three of the four pages, so one click instead of a
// drag-select is the highest-value JavaScript on the site. A button marks what it copies with
// data-copy="<id>"; with JavaScript off the text is still selectable, which is the fallback.
function wireCopyButtons(): void {
	document.querySelectorAll<HTMLButtonElement>("[data-copy]").forEach((button) => {
		const target = document.getElementById(button.dataset.copy || "");
		if (!target || !navigator.clipboard) return;
		button.hidden = false;
		const original = button.textContent;
		button.addEventListener("click", async () => {
			try {
				await navigator.clipboard.writeText(target.textContent?.trim() || "");
				button.textContent = "Copied";
				setTimeout(() => { button.textContent = original; }, 2000);
			} catch {
				// Clipboard is permission-gated and refuses outside a secure context. Leave the label alone
				// rather than claiming a copy that did not happen.
			}
		});
	});
}

wireThemeToggle();
wireCopyButtons();
