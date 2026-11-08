import Alpine from 'alpinejs';

import {protocolDecoderPlugin} from '@binacle/apps';
Alpine.plugin(protocolDecoderPlugin);

const COPIED_FOR_MS = 1500;

// The samples drawer. The component owns the strings; this opens the list, and copies or uses one.
Alpine.data('samples_panel', () => ({
	copied: null,
	show() {
		this.$refs.panel.showModal();
	},
	hide() {
		this.$refs.panel.close();
	},
	// The clipboard API exists only on https and localhost. Elsewhere the text is still selectable.
	canCopy() {
		return !!navigator.clipboard;
	},
	copy(sample) {
		return navigator.clipboard.writeText(sample.encoded).then(() => {
			this.copied = sample.name;
			setTimeout(() => {
				this.copied = null;
			}, COPIED_FOR_MS);
		});
	},
	use(sample) {
		this.model.result = sample.encoded;
		this.hide();
		this.addResult();
	},
	// Grows with a long string, and goes back to its rows when the string is short. scrollHeight is a whole
	// number, so setting it on a short string would round the box a pixel taller.
	fitToText(textarea) {
		textarea.style.height = '';
		if (textarea.scrollHeight > textarea.clientHeight) {
			textarea.style.height = `${textarea.scrollHeight + textarea.offsetHeight - textarea.clientHeight}px`;
		}
	}
}));

Alpine.start();
