// The phone nav scrolls sideways: mark the sides that have more, so the CSS can fade them.
export function initNavFade(): void {
	document.querySelectorAll<HTMLElement>("[data-site-nav]").forEach((nav) => {
		function mark(): void {
			const left = nav.scrollLeft > 1;
			const right = nav.scrollLeft + nav.clientWidth < nav.scrollWidth - 1;
			if (left && right) {
				nav.setAttribute("data-more", "both");
			} else if (left) {
				nav.setAttribute("data-more", "left");
			} else if (right) {
				nav.setAttribute("data-more", "right");
			} else {
				nav.removeAttribute("data-more");
			}
		}
		nav.addEventListener("scroll", mark, {passive: true});
		window.addEventListener("resize", mark);
		// A late font changes how wide the links are.
		if (document.fonts) {
			document.fonts.ready.then(mark);
		}
		mark();
	});
}
