const COPIED_FOR_MS = 1500;

// data-copy names the element to copy. Left empty, it copies the code in the block around the button.
// The button starts hidden and stays so where there is no clipboard to write to.
export function initCopyButton(): void {
	document.querySelectorAll<HTMLElement>("[data-copy]").forEach((button) => {
		const id = button.getAttribute("data-copy");
		const block = button.closest("[data-code-block], [data-command]");
		const source = id ? document.getElementById(id) : block?.querySelector("code");
		if (!source || !navigator.clipboard) {
			return;
		}
		button.hidden = false;
		button.title = "Copy";
		button.addEventListener("click", () => {
			navigator.clipboard.writeText((source.textContent ?? "").trim()).then(() => {
				button.textContent = "Copied";
				button.classList.add("is-copied");
				setTimeout(() => {
					button.textContent = "Copy";
					button.classList.remove("is-copied");
				}, COPIED_FOR_MS);
			});
		});
	});
}
