import Alpine from 'alpinejs';

import {curlCommand, packingDemoPlugin} from '@binacle/apps';
Alpine.plugin(packingDemoPlugin);

// The request behind the results. The component hands over method, path and body; the host is this page's
// origin, because the module always calls the API it is served from.
Alpine.data('request_fold', () => ({
	curl(request) {
		if (!request) {
			return '';
		}
		return curlCommand(request, window.location.origin);
	}
}));

Alpine.start();
