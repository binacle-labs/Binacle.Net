import { Alpine as AlpineType } from 'alpinejs';
import { Binacle as BinacleType } from '../components/visualizer';

declare global {
	let Alpine: AlpineType;
	let Binacle: BinacleType;


	interface Window {
		binacle: BinacleType;
	}
}


export {};
