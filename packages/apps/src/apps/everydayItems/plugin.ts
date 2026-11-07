import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin} from "../../components/logger/logger";
import {packViewPlugin} from "../../components/packView";
import {errorsDialogPlugin} from "../../components/errorsDialog/errorsDialog";
import {everydayItemsAppPlugin} from "./everydayItems";

export function everydayItemsPlugin(Alpine: AlpineType) {
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packViewPlugin);
	Alpine.plugin(everydayItemsAppPlugin);
	Alpine.plugin(errorsDialogPlugin);
}
