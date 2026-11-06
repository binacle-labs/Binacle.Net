import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin} from "./components/logger/logger";
import {packViewPlugin} from "./components/packView";
import {errorsDialogPlugin} from "./components/errorsDialog/errorsDialog";
import {protocolDecoderAppPlugin} from "./apps/protocolDecoder/protocolDecoder";

export function protocolDecoderPlugin(Alpine: AlpineType) {
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packViewPlugin);
	Alpine.plugin(protocolDecoderAppPlugin);
	Alpine.plugin(errorsDialogPlugin);
}


