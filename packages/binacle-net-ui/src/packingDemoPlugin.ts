import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin} from "./components/logger/logger";
import {fieldPlugin} from "./components/field/field";
import {packingVisualizerPlugin} from "./components/visualizer";
import {errorsDialogPlugin} from "./components/errorsDialog/errorsDialog";
import {packingDemoAppPlugin} from "./apps/packingDemo/packingDemo";

export function packingDemoPlugin(Alpine: AlpineType) {
	Alpine.plugin(fieldPlugin);
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packingDemoAppPlugin);
	Alpine.plugin(packingVisualizerPlugin);
	Alpine.plugin(errorsDialogPlugin);
}


