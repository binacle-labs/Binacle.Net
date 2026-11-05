import {initDrawer, initStickySidebar} from "binacle-components";
import {ThemeSwitcherButtonElement, optionsFromDocument} from "theme-switcher";
import {initSections} from "./sections";

ThemeSwitcherButtonElement.configure(optionsFromDocument());
customElements.define("theme-switcher", ThemeSwitcherButtonElement);

function initVersionSelect(): void {
	document.querySelectorAll<HTMLSelectElement>("[data-versionselect]").forEach((select) => {
		select.addEventListener("change", () => {
			// The option value is the url of this page in the chosen version, written by the build.
			const target = new URL(select.value, window.location.origin);
			if (target.origin === window.location.origin) {
				window.location.href = target.href;
			}
		});
	});
}

// The menu is one include for every page, so the build cannot know which groups hold this one.
function openCurrentGroups(): void {
	const current = document.querySelector('[data-sidebar-nav] [aria-current="page"]');
	let group = current?.closest("details");
	while (group) {
		group.open = true;
		group = group.parentElement?.closest("details");
	}
}

function initDialogs(): void {
	document.querySelectorAll<HTMLElement>("[data-dialog-open]").forEach((button) => {
		const dialog = document.getElementById(button.dataset.dialogOpen as string) as HTMLDialogElement | null;
		button.addEventListener("click", () => dialog?.showModal());
	});
	document.querySelectorAll("[data-dialog-close]").forEach((button) => {
		button.addEventListener("click", () => button.closest("dialog")?.close());
	});
}

document.addEventListener("DOMContentLoaded", () => {
	openCurrentGroups();
	initDrawer();
	initStickySidebar();
	initVersionSelect();
	initDialogs();
	initSections();
});
