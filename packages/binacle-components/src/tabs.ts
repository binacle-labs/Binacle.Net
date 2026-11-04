// One panel per button, in order.
export function initTabs(): void {
	document.querySelectorAll("[data-tabs]").forEach((box) => {
		const tabs = box.querySelectorAll('[role="tab"]');
		const panels = box.querySelectorAll<HTMLElement>('[role="tabpanel"]');
		tabs.forEach((tab, picked) => {
			tab.addEventListener("click", () => {
				tabs.forEach((other, index) => other.setAttribute("aria-selected", String(index === picked)));
				panels.forEach((panel, index) => {
					panel.hidden = index !== picked;
				});
			});
		});
	});
}
