import type {Alpine as AlpineType} from "alpinejs";
import {defineComponent} from "../../shared/defineComponent";
import Dimensions from "../../shared/dimensions";
import PackItem from "./packItem";
import {createPackView, PackView, ShowOptions} from "./packView";

export interface PackScene extends ShowOptions {
	bin?: Dimensions | null;
	items?: PackItem[] | null;
}

export function packViewPlugin(Alpine: AlpineType) {
	Alpine.data("pack_view", packViewComponent);
}

export const packViewComponent = defineComponent(() => {
	// Outside the returned object: Alpine would wrap anything in it in a reactive proxy.
	let view: PackView;
	let bin: Dimensions | null = null;

	function draw(scene: PackScene | null) {
		bin = scene?.bin ?? null;
		if (!scene?.bin) {
			view.clear();
		} else if (!scene.items) {
			view.preview(scene.bin);
		} else {
			view.show(scene.bin, scene.items, {kindNames: scene.kindNames, note: scene.note});
		}
	}

	return {
		async updateScene(scenePromise: () => Promise<PackScene | null>) {
			view.wait(bin);
			try {
				draw(await scenePromise());
			} catch (error) {
				this.$logger.error("[Binacle] Error while updating scene", error);
				this.$dispatch("error-occurred", ["Error while updating packing visualizer. Please try again later."]);
				draw(null);
			}
		},
		highlight(kind: number | null) {
			view.highlight(kind);
		},
		init() {
			view = createPackView(this.$el);
		}
	};
});
