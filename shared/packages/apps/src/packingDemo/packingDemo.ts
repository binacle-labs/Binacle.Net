import type { Alpine as AlpineType } from 'alpinejs';
import {defineComponent} from "../shared/defineComponent";
import {largestBin, nextSampleIndex, randomBin, randomItemFor, sampleAt} from "./samples";
import Bin from "./bin";
import Item from "./item";
import {errorFromFailure} from "../components/errorsDialog/errorFromFailure";
import type {PackScene} from "../components/packView/packViewComponent";
import {
	ApiFailure,
	BinacleClient,
	packCompareBinsPath,
	PackBinResponse,
	PackCompareResponse,
	PackCustomRequest,
	UnpackedBox
} from "@binacle/api-client";

// The call as it went over the wire, for a host that shows it. The path is relative: the host is the page's
// to name.
export interface SentRequest {
	method: 'POST';
	path: string;
	body: PackCustomRequest;
}

// The API's BinPackResultStatus, in the words a visitor reads.
const resultStatusTexts: Record<string, string> = {
	Unknown: 'Unknown',
	NotPacked: 'Not packed',
	PartiallyPacked: 'Partially packed',
	FullyPacked: 'Fully packed',
};

// The API's Algorithm values, in the words a visitor reads. The dropdown and the result row read this one
// table, so the two can never disagree. Best is first because it is what the page opens on.
const algorithmTexts: Record<string, string> = {
	Best: 'Try all, keep the best',
	FFD: 'First Fit Decreasing',
	BFD: 'Best Fit Decreasing',
	WFD: 'Worst Fit Decreasing',
};

export function packingDemoAppPlugin(Alpine: AlpineType) {
	Alpine.data('packing_demo_app', packingDemoApp);
}

export interface PackingDemoOptions {
	// Empty means fetch relative, from whatever host is serving the page.
	baseUrl?: string;
}

export const packingDemoApp = defineComponent((options: PackingDemoOptions = {}) => ({
	model: {
		bins: [] as Bin[],
		items: [] as Item[],
		algorithm: '',
	},
	algorithms: Object.entries(algorithmTexts).map(([value, text]) => ({value, text})),
	results: [] as PackBinResponse[],
	selectedResult: null as PackBinResponse | null,
	// What was asked for when `results` came back, not what the dropdown reads now.
	resultsAlgorithm: '',
	formErrors: [] as string[],
	sampleIndex: 0,
	submitting: false,
	submitStatus: '',
	// Set by a failed submit: every row then shows what is wrong, not only the rows already left.
	submitFailed: false,
	// Goes up on every edit, so an answer can tell the form changed while it was out.
	formVersion: 0,
	previewedBinId: '',
	// Set the moment a valid submit goes out, and left standing whatever the answer was.
	lastRequest: null as SentRequest | null,
	init() {
		// Sample zero, never a roll: the page opens on the same readable set every time.
		this.showSample(0);
		this.model.algorithm = this.algorithms[0].value;
		// $watch is deep, so this is the one place that catches every edit - randomize, a sample, a keystroke,
		// a bin added or cleared.
		this.$watch('model', () => this.onFormEdited());
		// The view is a child of this component, so it is not listening until the next tick.
		this.$nextTick(() => this.showBin());
	},
	// An answer belongs to the form it was asked for, so any edit drops it.
	onFormEdited() {
		const answerDropped = this.results.length > 0 || this.submitting;
		this.formVersion++;
		this.submitStatus = '';
		this.formErrors = [];
		this.results = [];
		this.selectedResult = null;
		this.resultsAlgorithm = '';
		// Redrawing resets the camera, so an edit that leaves the bin as it is does not redraw.
		if (answerDropped || (this.firstValidBin()?.id ?? '') !== this.previewedBinId) {
			this.showBin();
		}
	},
	firstValidBin() {
		return this.model.bins.find(bin => !bin.hasErrors()) ?? null;
	},
	binScene(): PackScene {
		const bin = this.firstValidBin();
		if (!bin) {
			return {};
		}
		return {bin: {length: Number(bin.length), width: Number(bin.width), height: Number(bin.height)}};
	},
	showBin() {
		const scene = this.binScene();
		this.previewedBinId = this.firstValidBin()?.id ?? '';
		this.$dispatch('update-scene', async () => scene);
	},
	// A row keeps quiet while it is being typed in.
	showsErrors(touched: boolean) {
		return touched || this.submitFailed;
	},
	showSample(index: number) {
		const sample = sampleAt(index);
		this.sampleIndex = index;
		this.submitFailed = false;
		this.model.bins = sample.bins;
		this.model.items = sample.items;
	},
	// `every` below is true on an empty list, so without this an emptied form posts no bins and no items.
	listErrors() {
		const errors = [] as string[];
		if (this.model.bins.length < 1) {
			errors.push('Add at least one bin.');
		}
		if (this.model.items.length < 1) {
			errors.push('Add at least one item.');
		}
		return errors;
	},
	isValid() {
		const binsValid = this.model.bins.every(bin => !bin.hasErrors());
		const itemsValid = this.model.items.every(item => !item.hasErrors());
		return this.listErrors().length < 1 && binsValid && itemsValid;
	},
	removeBin(index: number) {
		this.model.bins.splice(index, 1);
	},
	// A fresh roll when there are none, so nothing here has to handle an empty list.
	sizingBin() {
		return this.model.bins.length > 0 ? largestBin(this.model.bins) : randomBin();
	},
	addBin: {
		['@click']() {
			// A copy, not a roll: a fourth candidate is only worth comparing if it keeps the same footprint.
			const last = this.model.bins[this.model.bins.length - 1];
			this.model.bins.push(last ? Bin.copyOf(this.model.bins, last) : randomBin());
		}
	},
	clearAllBins: {
		['@click']() {
			this.model.bins = [];
		}
	},
	removeItem(index: number) {
		this.model.items.splice(index, 1);
	},
	addItem: {
		['@click']() {
			this.model.items.push(randomItemFor(this.sizingBin(), 1));
		}
	},
	clearAllItems: {
		['@click']() {
			this.model.items = [];
		}
	},
	// A different sample from the hand-picked set, not a roll of new dimensions.
	randomize: {
		['@click']() {
			this.showSample(nextSampleIndex(this.sampleIndex));
		}
	},
	handleErrorResponse(failure: ApiFailure) {
		this.$dispatch('error-occurred', errorFromFailure(failure));
	},
	async getResults(request: PackCustomRequest) : Promise<PackCompareResponse | null> {
		try {
			const client = new BinacleClient({baseUrl: options.baseUrl ?? ''});
			const response = await client.packCompareBins(request);
			if(response.ok){
				return response.data;
			}
			this.handleErrorResponse(response);
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
	async onSubmit() {
		this.formErrors = this.listErrors();
		if (!this.isValid()) {
			this.submitFailed = true;
			if (this.formErrors.length < 1) {
				this.formErrors = ['Fix the fields marked in red.'];
			}
			this.submitStatus = '';
			this.$logger.error("[Binacle] Model is not valid");
			return;
		}
		this.submitFailed = false;
		// Set before the request, so the button and the status line change in the click's own frame.
		this.submitting = true;
		this.submitStatus = 'Packing...';

		const request = {
			parameters: {
				algorithm: this.model.algorithm,
			},
			// x-model hands back the input's string, and the API declares these as int.
			bins: this.model.bins.map(x => ({
				id: x.id,
				length: Number(x.length),
				width: Number(x.width),
				height: Number(x.height)
			})),
			items: this.model.items.map(x => ({
				id: x.id,
				length: Number(x.length),
				width: Number(x.width),
				height: Number(x.height),
				quantity: Number(x.quantity)
			}))
		} as PackCustomRequest;

		this.lastRequest = {method: 'POST', path: packCompareBinsPath, body: request};

		// Started before the dispatch, so the loader runs for the whole wait, and the answer does not depend on
		// a view listening.
		const scene = this.packScene(request);
		this.$dispatch('update-scene', () => scene);
		await scene;
	},
	async packScene(request: PackCustomRequest): Promise<PackScene> {
		const version = this.formVersion;
		let response: PackCompareResponse | null = null;
		try {
			this.$logger.log('[Binacle] Packing request sent', request);
			response = await this.getResults(request);
			this.$logger.log('[Binacle] Packing results received', response);
		} finally {
			this.submitting = false;
		}

		if (version !== this.formVersion) {
			return this.binScene();
		}
		if (!response || !response.results) {
			this.results = [];
			this.selectedResult = null;
			this.resultsAlgorithm = '';
			this.submitStatus = 'No results.';
			return this.binScene();
		}

		const firstSuccessfulResult = response.results.find(x => !!x.bin);
		this.results = response.results;
		this.selectedResult = firstSuccessfulResult || null;
		this.resultsAlgorithm = request.parameters.algorithm;
		this.submitStatus = this.results.length > 0 ? '' : 'No results.';
		return firstSuccessfulResult ? {...this.sceneOf(firstSuccessfulResult), reveal: true} : this.binScene();
	},
	sceneOf(result: PackBinResponse): PackScene {
		// An item's id here is only its size, which the readout already prints.
		const items = (result.packedItems ?? []).map(({id, ...item}) => item);
		return {bin: result.bin, items, note: items.length > 0 ? '' : this.resultStatusText(result)};
	},
	isSelected(result: PackBinResponse) {
		return this.selectedResult === result;
	},
	selectResult(result: PackBinResponse) {
		this.selectedResult = result;
		this.$dispatch('update-scene', async () => this.sceneOf(result));
	},
	colorClass(result: PackBinResponse) {
		if (result.status === 'FullyPacked') {
			return 'green';
		}
		if (result.status === 'PartiallyPacked') {
			return 'orange';
		}
		return 'red';
	},
	resultStatusText(result: PackBinResponse) {
		return resultStatusTexts[result.status] ?? 'Unknown';
	},
	// A code the table does not know prints as the code. It is short, and it is the only answer there is.
	resultAlgorithmText(result: PackBinResponse) {
		return `Winner: ${algorithmTexts[result.algorithmUsed] ?? result.algorithmUsed}`;
	},
	// With one heuristic asked for, every row would repeat the dropdown a line below it.
	showsAlgorithmUsed() {
		return this.resultsAlgorithm === 'Best';
	},
	resultTitle(result: PackBinResponse) {
		return `Bin: ${result.bin.id}`;
	},
	resultBinPercentageText(result: PackBinResponse) {
		return `Packed Bin Volume: ${result.packedBinVolumePercentage}%`;
	},
	resultItemPercentageText(result: PackBinResponse) {
		return `Packed Items Volume: ${result.packedItemsVolumePercentage}%`
	},
	resultIsFullyPacked(result: PackBinResponse) {
		return result.status === 'FullyPacked';
	},
	resultBinSizeText(result: PackBinResponse) {
		return `${result.bin.length} x ${result.bin.width} x ${result.bin.height}`;
	},
	resultItemCountText(result: PackBinResponse) {
		const count = result.packedItems?.length ?? 0;
		return count === 1 ? '1 item' : `${count} items`;
	},
	// Two decimals, as the API sends them: JSON drops the zeros of a 65.00.
	resultBinFillText(result: PackBinResponse) {
		return `${result.packedBinVolumePercentage.toFixed(2)}%`;
	},
	resultItemsFillText(result: PackBinResponse) {
		return `Packed items volume ${result.packedItemsVolumePercentage.toFixed(2)}%`;
	},
	// Fully packed it is always 100, and with nothing packed always 0.
	showsItemsFill(result: PackBinResponse) {
		return result.status === 'PartiallyPacked';
	},
	resultAlgorithmName(result: PackBinResponse) {
		return algorithmTexts[result.algorithmUsed] ?? result.algorithmUsed;
	},
	submitButtonText() {
		return this.submitting ? 'Working...' : 'Get results';
	},
	unpackedItemsOf(result: PackBinResponse): UnpackedBox[] {
		return result.unpackedItems ?? [];
	},
	hasUnpackedItems(result: PackBinResponse) {
		return this.unpackedItemsOf(result).length > 0;
	},
	unpackedItemsTitle(result: PackBinResponse) {
		const count = this.unpackedItemsOf(result).reduce((total, item) => total + item.quantity, 0);
		return count === 1 ? 'Could not fit 1 item' : `Could not fit ${count} items`;
	},
	unpackedItemText(item: UnpackedBox) {
		return `${item.quantity} x ${item.id}`;
	},
	// The API only hands back the id, so the size comes from what was sent.
	unpackedItemSizeText(item: UnpackedBox) {
		const sent = this.lastRequest?.body.items.find(x => x.id === item.id);
		return sent ? `${sent.length} x ${sent.width} x ${sent.height}` : item.id;
	}
}));
