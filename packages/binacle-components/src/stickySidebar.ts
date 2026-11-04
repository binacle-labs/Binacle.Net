// A sticky menu stops where its column ends. As the footer comes into the window the menu gets shorter by
// that much, so its top stays in view.
export function initStickySidebar(): void {
	document.querySelectorAll<HTMLElement>("[data-sidebar-inner]").forEach((menu) => {
		const footer = document.querySelector("[data-site-footer]");
		if (!footer) {
			return;
		}
		const fit = (): void => {
			menu.style.setProperty("--below", Math.max(0, window.innerHeight - footer.getBoundingClientRect().top) + "px");
		};
		window.addEventListener("scroll", fit, {passive: true});
		window.addEventListener("resize", fit);
		fit();
	});
}
