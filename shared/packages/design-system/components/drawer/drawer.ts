const BURGER = '<button type="button" class="site-header__burger" aria-label="Menu" aria-expanded="false" aria-controls="site-drawer" data-drawer-open="site-drawer"><span class="site-header__burger-icon" aria-hidden="true"></span></button>';
const DRAWER_HEAD = '<div class="drawer__head"><span class="label">Menu</span><button type="button" class="drawer__close" aria-label="Close" data-drawer-close><span class="drawer__close-icon" aria-hidden="true"></span></button></div>';

// The copies take the drawer's class in place of the header's.
function copyLinks(nav: Element, selector: string, className: string): string {
	return Array.from(nav.querySelectorAll(selector)).map((link) => {
		const copy = link.cloneNode(true) as Element;
		copy.className = className;
		return copy.outerHTML;
	}).join("");
}

// A site that writes its own drawer has its button in the header already.
function buildSiteDrawer(header: Element): void {
	const nav = header.querySelector("[data-site-nav]");
	const shell = nav?.parentElement;
	if (!nav || !shell || shell.querySelector("[data-drawer-open]")) {
		return;
	}
	shell.insertAdjacentHTML("afterbegin", BURGER);
	header.insertAdjacentHTML("afterend",
		'<div class="drawer" id="site-drawer" data-drawer="drawer"><div class="drawer__inner">' + DRAWER_HEAD +
		'<nav class="drawer__pages" aria-label="Main">' + copyLinks(nav, "a:not([data-other-site])", "drawer__page-link") + "</nav>" +
		'<nav class="drawer__sites" aria-label="Sites">' + copyLinks(nav, "a[data-other-site]", "drawer__site-link") + "</nav></div></div>" +
		'<div class="drawer-backdrop" data-drawer-close></div>');
}

function isShown(element: Element): boolean {
	return element.getClientRects().length > 0;
}

function isOpen(panel: Element): boolean {
	return panel.classList.contains("is-open");
}

function openPanels(): HTMLElement[] {
	return Array.from(document.querySelectorAll<HTMLElement>("[data-drawer]")).filter(isOpen);
}

function openers(panel: HTMLElement): HTMLElement[] {
	return Array.from(document.querySelectorAll<HTMLElement>('[data-drawer-open="' + panel.id + '"]'));
}

// While open a drawer is a dialog and the rest of the page is inert, so Tab stays in it.
export function initDrawer(): void {
	// A header marked data-nav-row keeps the row that scrolls sideways.
	document.querySelectorAll("[data-site-header]:not([data-nav-row])").forEach(buildSiteDrawer);

	let opener: HTMLElement | null = null;
	let stilled: HTMLElement[] = [];

	// Everything beside the drawer and its backdrop, at every level up to the body.
	function stillTheRest(panel: HTMLElement): void {
		let element = panel;
		while (element.parentElement && element !== document.body) {
			const parent: HTMLElement = element.parentElement;
			for (const sibling of Array.from(parent.children) as HTMLElement[]) {
				if (sibling === element || sibling.inert || sibling.hasAttribute("data-drawer-close") || sibling.tagName === "SCRIPT") {
					continue;
				}
				sibling.inert = true;
				stilled.push(sibling);
			}
			element = parent;
		}
	}

	function setOpen(panel: HTMLElement, open: boolean): void {
		panel.classList.toggle("is-open", open);
		openers(panel).forEach((button) => button.setAttribute("aria-expanded", String(open)));
		if (!open) {
			["role", "aria-modal", "aria-label"].forEach((name) => panel.removeAttribute(name));
			stilled.forEach((element) => {
				element.inert = false;
			});
			stilled = [];
			return;
		}
		panel.setAttribute("role", "dialog");
		panel.setAttribute("aria-modal", "true");
		panel.setAttribute("aria-label", "Menu");
		stillTheRest(panel);
		const current = panel.querySelector<HTMLElement>('[aria-current="page"]');
		panel.scrollTop = current ? Math.max(0, current.offsetTop - 120) : 0;
		panel.querySelector<HTMLElement>("[data-drawer-close]")?.focus({preventScroll: true});
	}

	function closeAll(refocus: boolean): void {
		const open = openPanels();
		open.forEach((panel) => setOpen(panel, false));
		if (refocus && open.length && opener && isShown(opener)) {
			opener.focus();
		}
	}

	document.querySelectorAll<HTMLElement>("[data-drawer-open]").forEach((button) => {
		const panel = document.getElementById(button.getAttribute("data-drawer-open") as string);
		if (panel) {
			button.addEventListener("click", () => {
				opener = button;
				setOpen(panel, true);
			});
		}
	});
	document.querySelectorAll("[data-drawer-close]").forEach((button) => {
		button.addEventListener("click", () => closeAll(true));
	});
	// A link to a section must land on the page, not under the drawer. Focus goes where the link goes.
	document.querySelectorAll("[data-drawer]").forEach((panel) => {
		panel.addEventListener("click", (event) => {
			if ((event.target as Element).closest("a")) {
				closeAll(false);
			}
		});
	});
	document.addEventListener("keydown", (event) => {
		if (event.key === "Escape") {
			closeAll(true);
		}
	});
	// Widening the window until the drawer's button is gone closes it.
	window.addEventListener("resize", () => {
		openPanels().forEach((panel) => {
			if (!openers(panel).some(isShown)) {
				setOpen(panel, false);
			}
		});
	});
}
