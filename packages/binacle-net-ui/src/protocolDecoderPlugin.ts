import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin} from "./components/logger/logger";
import {packingVisualizerPlugin} from "./components/visualizer";
import {errorsDialogPlugin} from "./components/errorsDialog/errorsDialog";
import {protocolDecoderAppPlugin} from "./apps/protocolDecoder/protocolDecoder";

export function protocolDecoderPlugin(Alpine: AlpineType) {
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packingVisualizerPlugin);
	Alpine.plugin(protocolDecoderAppPlugin);
	Alpine.plugin(errorsDialogPlugin);
}


