// A heading counts as reached once it is this far from the top of the window.
const REACHED = 140;

function setCurrent(links: Element[], at: number): void {
	links.forEach((link, index) => link.setAttribute("aria-current", String(index === at)));
}

// The header names the page once its title has scrolled away.
function initPastTitle(): void {
	const header = document.querySelector<HTMLElement>("[data-site-header]");
	const title = document.querySelector("[data-doc-title]");
	if (!header || !title) {
		return;
	}
	const past = (): void => {
		header.classList.toggle("site-header--past", title.getBoundingClientRect().bottom < header.offsetHeight);
	};
	window.addEventListener("scroll", past, {passive: true});
	window.addEventListener("resize", past);
	past();
}

// The build writes the outline, the box and the button. This marks the current section in all three.
export function initSections(): void {
	initPastTitle();

	const heads = Array.from(document.querySelectorAll<HTMLElement>("[data-doc-body] h2[id]"));
	const box = document.querySelector<HTMLElement>("[data-section-box]");
	const button = document.querySelector<HTMLElement>("[data-section-button]");
	if (heads.length < 2 || !box || !button) {
		return;
	}

	const groups = Array.from(document.querySelectorAll("nav")).map((nav) => Array.from(nav.querySelectorAll("[data-section-link]"))).filter((links) => links.length);
	const boxRows = Array.from(box.querySelectorAll<HTMLElement>("li"));
	const toggle = button.querySelector("button") as HTMLButtonElement;
	const list = button.querySelector("ol") as HTMLElement;
	const name = button.querySelector("[data-section-button-name]") as HTMLElement;
	const section = document.querySelector("[data-section]");

	const openList = (open: boolean): void => {
		list.hidden = !open;
		toggle.setAttribute("aria-expanded", String(open));
		const current = list.querySelector<HTMLElement>('[aria-current="true"]');
		if (open && current) {
			list.scrollTop = current.offsetTop - list.clientHeight / 2;
		}
	};
	toggle.addEventListener("click", () => openList(list.hidden));
	list.addEventListener("click", () => openList(false));
	document.addEventListener("click", (event) => {
		if (!button.contains(event.target as Node)) {
			openList(false);
		}
	});

	let picked = -1;
	const mark = (): void => {
		let at = 0;
		heads.forEach((head, index) => {
			if (head.getBoundingClientRect().top < REACHED) {
				at = index;
			}
		});
		// At the bottom the last sections cannot scroll up to the top: light the one clicked, or the last in view.
		if (window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 2) {
			const seen = heads.filter((head) => head.getBoundingClientRect().top < window.innerHeight);
			if (picked >= 0 && heads[picked].getBoundingClientRect().top < window.innerHeight) {
				at = picked;
			} else if (seen.length) {
				at = heads.indexOf(seen[seen.length - 1]);
			}
		} else {
			picked = -1;
		}
		groups.forEach((links) => setCurrent(links, at));
		boxRows.forEach((row, index) => {
			row.hidden = Math.abs(index - at) > 2;
		});

		// Once the end of the page scrolls in, every section is passed: the box and the button step aside.
		const end = document.querySelector("[data-pager], [data-site-footer]");
		const gone = !!end && end.getBoundingClientRect().top < window.innerHeight;
		const past = heads[0].getBoundingClientRect().top < REACHED;
		box.classList.toggle("section-box--gone", gone);
		button.classList.toggle("section-button--gone", gone || !past);
		name.textContent = heads[at].textContent;
		if (gone || !past) {
			openList(false);
		}
		if (section) {
			section.textContent = past ? heads[at].textContent : "";
		}
	};

	groups.forEach((links) => links.forEach((link, index) => {
		link.addEventListener("click", () => {
			window.setTimeout(() => {
				picked = index;
				mark();
			}, 50);
		});
	}));
	window.addEventListener("scroll", mark, {passive: true});
	window.addEventListener("resize", mark);
	mark();
}
