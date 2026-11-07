import type {Alpine as AlpineType} from 'alpinejs';
import {defineComponent} from "../shared/defineComponent";
import type {Dimensions} from "@binacle/design-system";
import {errorFromFailure} from "../components/errorsDialog/errorFromFailure";
import type {PackScene} from "../components/packView/packViewComponent";
import {BinacleClient, PackBinResponse, PackPresetRequest} from "@binacle/api-client";
import {BOXES, BoxSize, ItemKind, KINDS, PRESET} from "./kinds";

// The home page's still is this algorithm's answer, so the two pages agree.
const ALGORITHM = 'BFD';
const MAX_COUNT = 9;
const OPENING_BOX = 'Large';
const OPENING_COUNTS: Record<number, number> = {1: 1, 2: 1, 3: 2, 4: 2, 5: 2, 6: 3};

export function everydayItemsAppPlugin(Alpine: AlpineType) {
	Alpine.data('everyday_items_app', everydayItemsApp);
}

export interface EverydayItemsOptions {
	// Empty means fetch relative, from whatever host is serving the page.
	baseUrl?: string;
}

// The home page links here with ?box= and a box's name.
export function boxAsked(search: string): string | null {
	const name = new URLSearchParams(search).get('box');
	return BOXES.some(box => box.name === name) ? name : null;
}

export function sizeText(size: Dimensions): string {
	return `${size.length} x ${size.width} x ${size.height}`;
}

function sizeOf({length, width, height}: Dimensions): Dimensions {
	return {length, width, height};
}

export const everydayItemsApp = defineComponent((options: EverydayItemsOptions = {}) => ({
	boxes: BOXES,
	kinds: KINDS,
	model: {
		box: OPENING_BOX,
		counts: {...OPENING_COUNTS} as Record<number, number>,
	},
	result: null as PackBinResponse | null,
	// The kind whose row is pressed, shown alone in the view.
	lit: null as number | null,
	submitting: false,
	status: '',
	statusIsError: false,
	// Goes up on every edit, so an answer can tell the form changed while it was out.
	formVersion: 0,
	init() {
		this.model.box = boxAsked(window.location.search) ?? OPENING_BOX;
		this.$watch('model', () => this.onFormEdited());
		// The view is a child of this component, so it is not listening until the next tick.
		this.$nextTick(() => this.pack(false));
	},
	box(): BoxSize {
		return BOXES.find(box => box.name === this.model.box) ?? BOXES[BOXES.length - 1];
	},
	countOf(kind: ItemKind) {
		return this.model.counts[kind.kind] ?? 0;
	},
	totalCount() {
		return KINDS.reduce((total, kind) => total + this.countOf(kind), 0);
	},
	canRemove(kind: ItemKind) {
		return this.countOf(kind) > 0;
	},
	canAdd(kind: ItemKind) {
		return this.countOf(kind) < MAX_COUNT;
	},
	changeCount(kind: ItemKind, by: number) {
		this.model.counts[kind.kind] = Math.max(0, Math.min(MAX_COUNT, this.countOf(kind) + by));
	},
	clearAll() {
		this.model.counts = {};
	},
	// An answer belongs to the form it was asked for, so any edit drops it and the view goes back to the box.
	onFormEdited() {
		this.formVersion++;
		this.dropResult();
		this.setStatus('');
		this.showBox();
	},
	dropResult() {
		this.result = null;
		this.lit = null;
	},
	setStatus(text: string, isError = false) {
		this.status = text;
		this.statusIsError = isError;
	},
	boxScene(): PackScene {
		return {bin: sizeOf(this.box())};
	},
	showBox() {
		const scene = this.boxScene();
		this.$dispatch('update-scene', async () => scene);
	},
	onSubmit() {
		return this.pack(true);
	},
	async pack(reveal: boolean) {
		this.dropResult();
		if (this.totalCount() < 1) {
			this.setStatus('Add at least one item.', true);
			this.showBox();
			return;
		}
		// Set before the request, so the button and the status line change in the click's own frame.
		this.submitting = true;
		this.setStatus('Packing...');
		// Started before the dispatch, so the loader runs for the whole wait.
		const scene = this.packScene(reveal);
		this.$dispatch('update-scene', () => scene);
		await scene;
	},
	request(): PackPresetRequest {
		return {
			parameters: {algorithm: ALGORITHM},
			items: KINDS.filter(kind => this.countOf(kind) > 0).map(kind => ({
				id: kind.name,
				quantity: this.countOf(kind),
				...sizeOf(kind),
			})),
		};
	},
	async packScene(reveal: boolean): Promise<PackScene> {
		const version = this.formVersion;
		let result: PackBinResponse | null = null;
		try {
			result = await this.getResult(this.box(), this.request());
		} finally {
			this.submitting = false;
		}

		if (version !== this.formVersion) {
			return this.boxScene();
		}
		if (!result) {
			this.setStatus('');
			return this.boxScene();
		}

		this.result = result;
		this.setStatus(this.resultStatusText(result));
		// The id is the kind's name, which the readout prints from kindNames.
		const items = (result.packedItems ?? []).map(({id, ...item}) => ({...item, kind: this.kindNamed(id)?.kind}));
		return {bin: result.bin, items, kindNames: KINDS.map(kind => kind.name), reveal};
	},
	async getResult(box: BoxSize, request: PackPresetRequest): Promise<PackBinResponse | null> {
		try {
			const client = new BinacleClient({baseUrl: options.baseUrl ?? ''});
			const response = await client.packPresetBin(PRESET, box.name, request);
			if (response.ok) {
				return response.data;
			}
			this.$dispatch('error-occurred', errorFromFailure(response));
			return null;
		} catch (error) {
			this.$logger.error("[Binacle] Error while fetching packing results", error);
			this.$dispatch('error-occurred', {
				title: "Error while fetching packing results",
				errors: [error instanceof Error ? error.message : String(error)]
			});
			return null;
		}
	},
	kindNamed(name: string): ItemKind | undefined {
		return KINDS.find(kind => kind.name === name);
	},
	unfitCountOf(kind: ItemKind) {
		return this.result?.unpackedItems?.find(item => item.id === kind.name)?.quantity ?? 0;
	},
	resultStatusText(result: PackBinResponse) {
		const unfit = (result.unpackedItems ?? []).reduce((total, item) => total + item.quantity, 0);
		if (unfit === 0) {
			return `Everything fits in the ${result.bin.id.toLowerCase()} box.`;
		}
		return `${result.packedItems?.length ?? 0} of ${this.totalCount()} items fit. ${unfit} did not.`;
	},
	boxSizeText(box: BoxSize) {
		return sizeText(box);
	},
	kindSizeText(kind: ItemKind) {
		return `${sizeText(kind)} cm`;
	},
	kindKeyStyle(kind: ItemKind) {
		return `background: var(--kind-${kind.kind})`;
	},
	resultTitle(result: PackBinResponse) {
		return `${result.bin.id} box`;
	},
	resultBinSizeText(result: PackBinResponse) {
		return sizeText(result.bin);
	},
	resultItemCountText(result: PackBinResponse) {
		const count = result.packedItems?.length ?? 0;
		return count === 1 ? '1 item' : `${count} items`;
	},
	// Two decimals, as the API sends them: JSON drops the zeros of a 65.00.
	resultBinFillText(result: PackBinResponse) {
		return `${result.packedBinVolumePercentage.toFixed(2)}%`;
	},
	// One row per kind that was asked for.
	resultKinds() {
		return KINDS.filter(kind => this.countOf(kind) > 0);
	},
	kindResultText(kind: ItemKind) {
		const asked = this.countOf(kind);
		const unfit = this.unfitCountOf(kind);
		const packed = `${asked - unfit} of ${asked} packed`;
		return unfit > 0 ? `${packed} - ${unfit} did not fit` : packed;
	},
	isLit(kind: ItemKind) {
		return this.lit === kind.kind;
	},
	// A second press on the same row shows every kind again.
	toggleKind(kind: ItemKind) {
		this.lit = this.isLit(kind) ? null : kind.kind;
		this.$dispatch('highlight-kind', this.lit);
	},
}));
