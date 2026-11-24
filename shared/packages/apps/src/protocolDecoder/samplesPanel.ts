import type {Alpine as AlpineType} from 'alpinejs';

import {defineComponent} from "../shared/defineComponent";
import type {SampleData} from "./sampleData";

const COPIED_FOR_MS = 1500;

// What the decoder's scope lends this component. It nests inside that scope, so it must not define its own.
interface DecoderScope {
	model: { result: string | null };
	addResult(): void;
}

export function samplesPanelPlugin(Alpine: AlpineType) {
	Alpine.data('samples_panel', samplesPanel);
}

// The samples drawer. The component owns the strings; this opens the list, and copies or uses one.
export const samplesPanel = defineComponent(() => ({
	copied: null as string | null,
	show() {
		(this.$refs.panel as HTMLDialogElement).showModal();
	},
	hide() {
		(this.$refs.panel as HTMLDialogElement).close();
	},
	// The clipboard API exists only on https and localhost. Elsewhere the text is still selectable.
	canCopy() {
		return !!navigator.clipboard;
	},
	copy(sample: SampleData) {
		return navigator.clipboard.writeText(sample.encoded).then(() => {
			this.copied = sample.name;
			setTimeout(() => {
				this.copied = null;
			}, COPIED_FOR_MS);
		});
	},
	use(sample: SampleData) {
		const parent = this as unknown as DecoderScope;
		parent.model.result = sample.encoded;
		this.hide();
		parent.addResult();
	},
	// Grows with a long string, and goes back to its rows when the string is short. scrollHeight is a whole
	// number, so setting it on a short string would round the box a pixel taller.
	fitToText(textarea: HTMLTextAreaElement) {
		textarea.style.height = '';
		if (textarea.scrollHeight > textarea.clientHeight) {
			textarea.style.height = `${textarea.scrollHeight + textarea.offsetHeight - textarea.clientHeight}px`;
		}
	}
}));
