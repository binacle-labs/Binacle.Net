import {createPackView} from "@binacle/design-system";
import type {Dimensions, PackItem, PackView, ShowOptions} from "@binacle/design-system";
import type {Alpine as AlpineType} from "alpinejs";
import {defineComponent} from "../../shared/defineComponent";

// The width below which the view stacks above the form, from the design system.
const STACKED = "(width < 800px)";

export interface PackScene extends ShowOptions {
	bin?: Dimensions | null;
	items?: PackItem[] | null;
	// Takes the page to the view when it sits far from the button that asked for this scene.
	reveal?: boolean;
}

export function packViewPlugin(Alpine: AlpineType) {
	Alpine.data("pack_view", packViewComponent);
}

export const packViewComponent = defineComponent(() => {
	// Outside the returned object: Alpine would wrap anything in it in a reactive proxy.
	let view: PackView;
	let figure: HTMLElement;
	let bin: Dimensions | null = null;
	let latest = 0;

	function reveal() {
		if (matchMedia(STACKED).matches) {
			const still = matchMedia("(prefers-reduced-motion: reduce)").matches;
			figure.scrollIntoView({behavior: still ? "auto" : "smooth", block: "start"});
		}
	}

	function draw(scene: PackScene | null) {
		bin = scene?.bin ?? null;
		if (!scene?.bin) {
			view.clear();
		} else if (!scene.items) {
			view.preview(scene.bin);
		} else {
			view.show(scene.bin, scene.items, {note: scene.note});
		}
		if (scene?.reveal) {
			reveal();
		}
	}

	return {
		async updateScene(scenePromise: () => Promise<PackScene | null>) {
			// A slow answer must not draw over a newer scene.
			const update = ++latest;
			view.wait(bin);
			try {
				const scene = await scenePromise();
				if (update === latest) {
					draw(scene);
				}
			} catch (error) {
				this.$logger.error("[Binacle] Error while updating scene", error);
				this.$dispatch("error-occurred", ["Error while updating packing visualizer. Please try again later."]);
				if (update === latest) {
					draw(null);
				}
			}
		},
		highlight(itemType: string | null) {
			view.highlight(itemType);
		},
		init() {
			figure = this.$el;
			view = createPackView(figure);
		}
	};
});
