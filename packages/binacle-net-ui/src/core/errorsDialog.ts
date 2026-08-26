import type {Alpine as AlpineType} from 'alpinejs';
import {defineComponent} from "../utils";
import {Error} from "../viewModels";

export function errorsDialogPlugin(Alpine: AlpineType) {
	Alpine.data('errors_dialog', errorsDialog);
}

export const errorsDialog = defineComponent((default_title: string) => ({
	errors: [] as string[],
	defaultTitle: default_title,
	title: default_title,
	init() {
		// Escape and the backdrop close a native dialog without going through closeDialog.
		this.dialogElement()?.addEventListener('close', () => this.closeDialog());
	},
	dialogElement(): HTMLDialogElement | null {
		return this.$root?.querySelector('dialog') ?? null;
	},
	hasErrors() {
		return this.errors.length > 0;
	},
	closeDialog() {
		this.errors = [];
		this.title = this.defaultTitle;
		const dialog = this.dialogElement();
		if (dialog?.open) {
			dialog.close();
		}
	},
	onErrorOccurred(data: string[] | Error) {
		if (Array.isArray(data)) {
			this.errors = data;
		}
		const error = data as Error;
		if (error.title) {
			this.title = error.title;
		}
		if (error.errors) {
			this.errors = error.errors;
		}
		if (!this.hasErrors()) {
			return;
		}
		const dialog = this.dialogElement();
		// showModal is what takes focus, traps Tab and makes Escape close it. The class binding only styles it.
		if (dialog && !dialog.open && typeof dialog.showModal === 'function') {
			dialog.showModal();
		}
	}
}));
