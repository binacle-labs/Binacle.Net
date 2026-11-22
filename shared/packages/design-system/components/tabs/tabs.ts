let sets = 0;

// One panel per button, in order. Keys and links follow the ARIA tabs pattern.
export function initTabs(): void {
	document.querySelectorAll("[data-tabs]").forEach((box) => {
		const tabs = Array.from(box.querySelectorAll<HTMLElement>('[role="tab"]'));
		const panels = Array.from(box.querySelectorAll<HTMLElement>('[role="tabpanel"]'));
		const set = ++sets;
		const last = tabs.length - 1;

		const select = (picked: number): void => {
			tabs.forEach((tab, index) => {
				tab.setAttribute("aria-selected", String(index === picked));
				// Tab stops once on the row; the arrows move inside it.
				tab.tabIndex = index === picked ? 0 : -1;
			});
			panels.forEach((panel, index) => {
				panel.hidden = index !== picked;
			});
		};

		tabs.forEach((tab, index) => {
			const panel = panels[index];
			if (panel) {
				tab.id ||= `tabs-${set}-tab-${index + 1}`;
				panel.id ||= `tabs-${set}-panel-${index + 1}`;
				tab.setAttribute("aria-controls", panel.id);
				panel.setAttribute("aria-labelledby", tab.id);
				// A panel may open on plain text, which Tab would skip.
				panel.tabIndex = 0;
			}

			tab.addEventListener("click", () => select(index));
			tab.addEventListener("keydown", (event) => {
				const moves: Record<string, number> = {
					ArrowRight: index === last ? 0 : index + 1,
					ArrowLeft: index === 0 ? last : index - 1,
					Home: 0,
					End: last,
				};
				const next = moves[event.key];
				if (next === undefined) {
					return;
				}
				event.preventDefault();
				select(next);
				tabs[next].focus();
			});
		});

		const selected = tabs.findIndex((tab) => tab.getAttribute("aria-selected") === "true");
		select(selected === -1 ? 0 : selected);
	});
}
