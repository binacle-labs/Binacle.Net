import VisualizerState from "./visualizerState";

export default interface Binacle {
	visualizerContainer: HTMLElement | null;
	rendererContainer: HTMLElement | null;
	visualizerState: VisualizerState | null;
}
