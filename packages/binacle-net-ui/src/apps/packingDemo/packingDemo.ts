import type { Alpine as AlpineType } from 'alpinejs';
import {defineComponent} from "../../shared/defineComponent";
import {getResponseStatusText} from "./getResponseStatusText";
import {largestBin, nextSampleIndex, randomBin, randomItemFor, sampleAt} from "./samples";
import Bin from "./bin";
import Item from "./item";
import ErrorViewModel from "../../components/errorsDialog/error";
import {
	ApiFailure,
	BinacleClient,
	hasValidationErrors,
	PackBinResponse,
	PackCompareResponse,
	PackCustomRequest,
	UnpackedBox
} from "binacle-net-client";

// The API's BinPackResultStatus, in the words a visitor reads.
const resultStatusTexts: Record<string, string> = {
	Unknown: 'Unknown',
	NotPacked: 'Not packed',
	PartiallyPacked: 'Partially packed',
	FullyPacked: 'Fully packed',
};

// The API's Algorithm values, in the words a visitor reads. The dropdown and the result row read this one
// table, so the two can never disagree.
const algorithmTexts: Record<string, string> = {
	FFD: 'First Fit Decreasing',
	BFD: 'Best Fit Decreasing',
	WFD: 'Worst Fit Decreasing',
	Best: 'Try all, keep the best',
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
	init() {
		// Sample zero, never a roll: the page opens on the same readable set every time.
		this.showSample(0);
		this.model.algorithm = this.algorithms[0].value;
		// $watch is deep, so this is the one place that catches every edit - randomize, a sample, a keystroke,
		// a bin added or cleared.
		this.$watch('model', () => {
			this.submitStatus = '';
		});
	},
	showSample(index: number) {
		const sample = sampleAt(index);
		this.sampleIndex = index;
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
		let errorObj = {
			title: `Error: ${getResponseStatusText(failure.status)}`,
			errors: []
		} as ErrorViewModel;

		const problem = failure.problem;
		if(problem === null){
			errorObj.errors.push('An error occurred, but the error response could not be parsed.');
		}
		else {
			if(problem.title){
				errorObj.title = problem.title;
			}
			if(problem.detail){
				errorObj.errors.push(problem.detail);
			}
			if(failure.status === 422 && hasValidationErrors(problem)){
				const errorsByField = problem.errors ?? {};
				for(const key in errorsByField){
					const fieldErrors = errorsByField[key];
					fieldErrors.forEach((err: string) => {
						errorObj.errors.push(`${key}: ${err}`);
					});
				}
			}
		}
		this.$dispatch('error-occurred', errorObj);
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
	onSubmit() {
		this.formErrors = this.listErrors();
		if (!this.isValid()) {
			this.submitStatus = '';
			this.$logger.error("[Binacle] Model is not valid");
			return;
		}
		// Set before the dispatch, so the button and the status line change in the click's own frame.
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

		this.$dispatch('update-scene', async () => {
			try {
				this.$logger.log('[Binacle] Packing request sent', request);
				const response = await this.getResults(request);
				this.$logger.log('[Binacle] Packing results received', response);
				if(!response || !response.results){
					this.results = [];
					this.selectedResult = null;
					this.resultsAlgorithm = '';
					this.submitStatus = 'No results.';
					return null;
				}
				const firstSuccessfulResult = response.results.find(x => !!x.bin);
				this.results = response.results;
				this.selectedResult = firstSuccessfulResult || null;
				this.resultsAlgorithm = request.parameters.algorithm;
				this.submitStatus = this.results.length > 0 ? '' : 'No results.';
				return {
					bin: firstSuccessfulResult?.bin,
					items: firstSuccessfulResult?.packedItems || []
				};
			} finally {
				this.submitting = false;
			}
		});

	},
	isSelected(result: PackBinResponse) {
		return this.selectedResult === result;
	},
	selectResult(result: PackBinResponse) {
		this.selectedResult = result;
		this.$dispatch('update-scene', async () => {
			return {
				bin: result?.bin,
				items: result?.packedItems || []
			};
		});
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
	}
}));
