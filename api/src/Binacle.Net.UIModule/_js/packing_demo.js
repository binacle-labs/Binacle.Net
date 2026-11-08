import Alpine from 'alpinejs';

import {packingDemoPlugin} from '@binacle/apps';
Alpine.plugin(packingDemoPlugin);

function escape(text) {
	return text.replace(/&/g, '&amp;').replace(/</g, '&lt;');
}

// A command breaks only between words, and the closing \ stays with the word before it. A word wider than the
// box, like a long address, breaks only after a slash.
function words(text) {
	const list = text.split(' ');
	if (list.at(-1) === '\\') {
		list.splice(-2, 2, list.at(-2) + ' \\');
	}
	return list.map(word => '<span class="code-block__word">' + escape(word).replace(/\/(?=.)/g, '/<wbr>') + '</span>').join(' ');
}

// JSON in weight only, as on the docs: keys and numbers heavier, punctuation muted.
function weigh(text) {
	const tokens = /("(?:[^"\\]|\\.)*")(\s*:)?|(-?\d+(?:\.\d+)?)(?![\w.])|([{}\[\],])/g;
	let out = '';
	let last = 0;
	let match;
	while ((match = tokens.exec(text))) {
		out += escape(text.slice(last, match.index));
		if (match[1] && match[2]) {
			out += `<span class="code-block__property">${escape(match[1])}</span><span class="code-block__punctuation">${match[2]}</span>`;
		} else if (match[1]) {
			out += escape(match[1]);
		} else if (match[3]) {
			out += `<span class="code-block__number">${match[3]}</span>`;
		} else {
			out += `<span class="code-block__punctuation">${match[4]}</span>`;
		}
		last = tokens.lastIndex;
	}
	return out + escape(text.slice(last));
}

// The request behind the results. The component hands over method, path and body; the host is this page's
// origin, because the module always calls the API it is served from.
Alpine.data('request_fold', () => ({
	// Each line in its own element, so a wrapped line hangs under its start. The spaces and newlines stay in the
	// text, so Copy gives the command back as it was.
	curl(request) {
		if (!request) {
			return '';
		}
		const body = JSON.stringify(request.body, null, 2).replace(/'/g, "'\\''");
		const lines = [
			`curl -X ${request.method} ${window.location.origin}${request.path} \\`,
			`  -H 'Content-Type: application/json' \\`,
			...`  -d '${body}'`.split('\n')
		];
		return lines.map((line, index) => {
			const indent = line.match(/^ */)[0].length;
			const text = index < 2 ? words(line.slice(indent)) : weigh(line.slice(indent));
			const end = index < lines.length - 1 ? '\n' : '';
			return `<span class="code-block__line" style="--indent:${indent}ch"><span class="code-block__indent">${' '.repeat(indent)}</span>${text}${end}</span>`;
		}).join('');
	}
}));

Alpine.start();
