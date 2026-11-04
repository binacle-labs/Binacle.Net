// A still that opens the dialog it names. Escape, a click on the picture or the backdrop closes it.
export function initLightbox(): void {
	document.querySelectorAll("[data-lightbox]").forEach((button) => {
		const box = document.getElementById(button.getAttribute("data-lightbox") as string) as HTMLDialogElement | null;
		if (!box) {
			return;
		}
		button.addEventListener("click", () => box.showModal());
		box.addEventListener("click", (event) => {
			const target = event.target as Element;
			if (target === box || target.closest("[data-pack-still]") || target.hasAttribute("data-close")) {
				box.close();
			}
		});
	});
}
