import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin} from "../../components/logger/logger";
import {fieldPlugin} from "../../components/field/field";
import {packViewPlugin} from "../../components/packView";
import {errorsDialogPlugin} from "../../components/errorsDialog/errorsDialog";
import {packingDemoAppPlugin} from "./packingDemo";

export function packingDemoPlugin(Alpine: AlpineType) {
	Alpine.plugin(fieldPlugin);
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packingDemoAppPlugin);
	Alpine.plugin(packViewPlugin);
	Alpine.plugin(errorsDialogPlugin);
}
