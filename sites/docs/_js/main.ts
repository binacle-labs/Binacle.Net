import {initCodeWrap, initCopyButton, initDrawer, initLightbox, initStickySidebar, initTabs} from "binacle-components";
import {ThemeSwitcherButtonElement, currentTheme, optionsFromDocument} from "theme-switcher";
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

// Swagger is its own page in a frame, so it cannot see the site's theme. It only shows while its dialog is open.
function initSwaggerTheme(): void {
	const frame = document.querySelector<HTMLIFrameElement>("[data-swagger-frame]");
	if (!frame) {
		return;
	}
	const paint = (): void => {
		frame.contentDocument?.documentElement.classList.toggle("dark-mode", currentTheme() === "dark");
	};
	frame.addEventListener("load", paint);
	document.querySelector('[data-dialog-open="swagger-dialog"]')?.addEventListener("click", paint);
}

document.addEventListener("DOMContentLoaded", () => {
	openCurrentGroups();
	initDrawer();
	initStickySidebar();
	initVersionSelect();
	initDialogs();
	initSwaggerTheme();
	initLightbox();
	initTabs();
	initSections();
	initCodeWrap();
	initCopyButton();
});
