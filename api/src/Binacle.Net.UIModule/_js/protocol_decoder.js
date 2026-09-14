import Alpine from 'alpinejs';

import {protocolDecoderPlugin} from 'binacle-net-ui';
Alpine.plugin(protocolDecoderPlugin);

// The samples panel. The component owns the strings; this only opens the list and copies one.
Alpine.data('samples_panel', () => ({
	open: false,
	show() {
		this.open = true;
	},
	hide() {
		this.open = false;
	},
	// The clipboard API exists only on https and localhost. Elsewhere the text is still selectable.
	canCopy() {
		return !!navigator.clipboard;
	},
	copy(sample) {
		return navigator.clipboard.writeText(sample.encoded);
	}
}));

Alpine.start();
