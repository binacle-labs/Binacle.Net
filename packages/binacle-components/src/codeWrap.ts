interface Word {
	html: string;
	text: string;
}

const LONG_INLINE_CODE = 24;

// A command breaks only between words: each word is kept whole.
function wrapCommands(): void {
	document.querySelectorAll("[data-command] code").forEach((code) => {
		code.innerHTML = code.innerHTML.split(/(\s+)/).map((part) => {
			return !part || /^\s+$/.test(part) ? part : '<span class="command__word">' + part + "</span>";
		}).join("");
	});
}

// Shorter code never breaks.
function markLongInlineCode(): Element[] {
	const long = Array.from(document.querySelectorAll(":not(pre) > code")).filter((code) => {
		return (code.textContent ?? "").length > LONG_INLINE_CODE;
	});
	long.forEach((code) => code.classList.add("text-break"));
	return long;
}

// A name splits at its parts before it splits inside a word.
function breakAtNameParts(long: Element[]): void {
	document.querySelectorAll(":not(pre) > code, [data-table] code").forEach((code) => {
		if (!long.includes(code) && !code.closest("[data-table]")) {
			return;
		}
		if (code.children.length) {
			return;
		}
		code.innerHTML = code.innerHTML.replace(/([._=]+)(?=[^._=])/g, "$1<wbr>");
	});
}

// A string with no spaces, like base64, breaks only between groups of four, so its closing = never sits
// alone on the last line.
function breakUnspacedStrings(): void {
	document.querySelectorAll("[data-code-block] > pre[data-wrap] > code").forEach((code) => {
		const text = (code.textContent ?? "").trim();
		if (!text || /\s/.test(text)) {
			return;
		}
		code.innerHTML = text.match(/.{1,4}/g)!.join("<wbr>");
	});
}

// Split after each comma and slash; a hyphenated part is kept whole like a word.
function breakInsideWord(text: string): string {
	return text.split(/(?<=[,/])(?=.)/).map((part) => {
		return /-/.test(part) ? '<span class="code-block__word">' + part + "</span>" : part;
	}).join("<wbr>");
}

// A wrapped line breaks only between words, never at a hyphen. A key stays with its value, and a closing
// bracket or a line-continuing backslash with the word before it. A word wider than the line breaks after
// a comma or a slash first.
function wrapWords(html: string): string {
	// Markup inside a word stays with it. A span holding a space cannot, so that line is left alone.
	if (/<span[^>]*>[^<]*\s[^<]*<\/span>/.test(html)) {
		return html;
	}
	const words: Word[] = [];
	const gaps: string[] = [];
	let word: Word | null = null;
	const tokens = /(<[^>]*>)|(\s+)|([^<\s]+)/g;
	let token: RegExpExecArray | null;
	while ((token = tokens.exec(html)) !== null) {
		const [all, tag, gap] = token;
		if (gap) {
			if (word) {
				words.push(word);
			}
			word = null;
			gaps[words.length] = gap;
			continue;
		}
		word = word ?? {html: "", text: ""};
		if (/,$/.test(word.text) && !/^<\//.test(all)) {
			word.html += "<wbr>";
		}
		word.html += tag ? all : breakInsideWord(all);
		if (!tag) {
			word.text += all;
		}
	}
	if (word) {
		words.push(word);
	}
	if (!words.length) {
		return html;
	}
	let out = (gaps[0] || "") + '<span class="code-block__word">';
	words.forEach((current, index) => {
		if (index > 0) {
			const joins = /:$/.test(words[index - 1].text) || /^([}\]]+,?|\\)$/.test(current.text);
			out += joins ? gaps[index] : "</span>" + gaps[index] + '<span class="code-block__word">';
		}
		out += current.html;
	});
	return out + "</span>" + (gaps[words.length] || "");
}

// Each line in its own element, so a wrapped line can hang under its start. The indent is drawn as
// padding; its spaces and the newline stay in the text, so Copy gives the code back as it was.
function wrapLines(code: Element): void {
	const lines = code.innerHTML.split("\n");
	// A closing newline ends the last line; it does not open an empty one under it.
	const closed = lines.length > 1 && lines[lines.length - 1] === "";
	if (closed) {
		lines.pop();
	}
	code.innerHTML = lines.map((line, index) => {
		const indent = /^ */.exec(line)![0].length;
		const end = index < lines.length - 1 || closed ? "\n" : "";
		return '<span class="code-block__line" style="--indent:' + indent + 'ch">' +
			'<span class="code-block__indent">' + line.slice(0, indent) + "</span>" +
			(wrapWords(line.slice(indent)) || "&#8203;") + end + "</span>";
	}).join("");
}

// A command wraps at every width, so none of it is hidden past the edge.
function wrapShellBlock(code: Element): void {
	const pre = code.parentElement!;
	const label = pre.parentElement!.querySelector("[data-code-lang]");
	const lang = code.getAttribute("data-lang") || (label ? (label.textContent ?? "").trim() : "");
	if (/^(bash|cmd)$/i.test(lang)) {
		pre.classList.add("code-block__pre--wrap");
		pre.setAttribute("data-wrap", "");
	}
}

// Sets where commands, inline code and code lines may break. The steps run in this order on purpose: a
// later one reads what an earlier one wrote.
export function initCodeWrap(): void {
	wrapCommands();
	breakAtNameParts(markLongInlineCode());
	breakUnspacedStrings();
	document.querySelectorAll("[data-code-block] > pre:not([data-wrap]) > code").forEach((code) => {
		wrapLines(code);
		wrapShellBlock(code);
	});
}
