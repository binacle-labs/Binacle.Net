import Alpine from 'alpinejs';

import {packingDemoPlugin} from 'binacle-net-ui';
Alpine.plugin(packingDemoPlugin);

// The request panel. The component hands over method, path and body; the host is this page's origin, because
// the module always calls the API it is served from.
Alpine.data('request_panel', () => ({
	open: false,
	show() {
		this.open = true;
	},
	hide() {
		this.open = false;
	},
	curl(request) {
		if (!request) {
			return '';
		}
		const body = JSON.stringify(request.body, null, 2).replace(/'/g, "'\\''");
		return `curl -X ${request.method} ${window.location.origin}${request.path} \\\n`
			+ `  -H 'Content-Type: application/json' \\\n`
			+ `  -d '${body}'`;
	},
	// The clipboard API exists only on https and localhost. Elsewhere the text is still selectable.
	canCopy() {
		return !!navigator.clipboard;
	},
	copy(request) {
		return navigator.clipboard.writeText(this.curl(request));
	}
}));

Alpine.start();
