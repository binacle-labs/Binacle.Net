import {optionsFromDocument} from "./options";
import {readStored} from "./storage";
import {applyTheme, resolveDefault} from "./theme";

// Runs in <head>, before first paint. It cannot touch document.body - that does not exist yet - which is
// why the theme lives on the html element.
export default function initTheme(): void {
	const options = optionsFromDocument();
	applyTheme(readStored(options) ?? resolveDefault(options.defaultTheme));
}
