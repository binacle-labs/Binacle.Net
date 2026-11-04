import {initCodeWrap} from "../src/codeWrap";

const WORD = '<span class="code-block__word">';

function code(): HTMLElement {
	return document.querySelector("code") as HTMLElement;
}

function pre(): HTMLElement {
	return document.querySelector("pre") as HTMLElement;
}

function wrap(html: string): void {
	document.body.innerHTML = html;
	initCodeWrap();
}

function block(text: string): string {
	return "<div data-code-block><pre><code>" + text + "</code></pre></div>";
}

function lines(): HTMLElement[] {
	return Array.from(document.querySelectorAll<HTMLElement>(".code-block__line"));
}

// What follows the indent on the first line, without the indent's own span.
function firstLine(text: string): string {
	wrap(block(text));
	return lines()[0].innerHTML.replace(/^<span class="code-block__indent">[^<]*<\/span>/, "");
}

describe("commands", () => {
	test("each word is kept whole and the spaces stay", () => {
		wrap("<div data-command><code>npm  run build</code></div>");

		expect(code().innerHTML).toBe('<span class="command__word">npm</span>  <span class="command__word">run</span> <span class="command__word">build</span>');
	});
});

describe("inline code", () => {
	test("24 characters is not long", () => {
		wrap("<p><code>" + "x".repeat(24) + "</code></p>");

		expect(code().classList.contains("text-break")).toBe(false);
	});

	test("25 characters is long", () => {
		wrap("<p><code>" + "x".repeat(25) + "</code></p>");

		expect(code().classList.contains("text-break")).toBe(true);
	});

	test("long code may break after a dot, an underscore or an equals sign", () => {
		wrap("<p><code>a_very_long.name=that-goes-past-it</code></p>");

		expect(code().innerHTML).toBe("a_<wbr>very_<wbr>long.<wbr>name=<wbr>that-goes-past-it");
	});

	test("a run of those breaks once, after the run", () => {
		wrap("<p><code>some__long_enough_name.._to_count</code></p>");

		expect(code().innerHTML).toBe("some__<wbr>long_<wbr>enough_<wbr>name.._<wbr>to_<wbr>count");
	});

	test("short code stays as it is", () => {
		wrap("<p><code>a.b</code></p>");

		expect(code().innerHTML).toBe("a.b");
	});

	test("short code in a table cell may break, and is not marked long", () => {
		wrap("<table data-table><tbody><tr><td><code>a.b</code></td></tr></tbody></table>");

		expect(code().innerHTML).toBe("a.<wbr>b");
		expect(code().classList.contains("text-break")).toBe(false);
	});

	test("code with markup in it is left alone", () => {
		const html = "long.long.long.long.long.long.<b>x</b>";

		wrap("<p><code>" + html + "</code></p>");

		expect(code().innerHTML).toBe(html);
	});
});

describe("a wrapped string with no spaces", () => {
	test("breaks between groups of four", () => {
		wrap("<div data-code-block><pre data-wrap><code>  QUJDREVGRw==  </code></pre></div>");

		expect(code().innerHTML).toBe("QUJD<wbr>REVG<wbr>Rw==");
	});

	test("one with spaces is left alone", () => {
		wrap("<div data-code-block><pre data-wrap><code>two words</code></pre></div>");

		expect(code().innerHTML).toBe("two words");
	});

	test("a block already marked to wrap gets no line elements", () => {
		wrap("<div data-code-block><pre data-wrap><code>two words</code></pre></div>");

		expect(lines()).toHaveLength(0);
	});
});

describe("code block lines", () => {
	test("each line gets its own element", () => {
		wrap(block("one\ntwo"));

		expect(lines().map((line) => line.textContent)).toEqual(["one\n", "two"]);
	});

	test("a closing newline opens no empty line", () => {
		wrap(block("one\ntwo\n"));

		expect(lines().map((line) => line.textContent)).toEqual(["one\n", "two\n"]);
	});

	test("the text stays as it was, so Copy gives the code back", () => {
		const text = "{\n  \"a\": 1,\n    \"b\": [\n  ]\n}\n";

		wrap(block(text));

		expect(code().textContent).toBe(text);
	});

	test("the indent becomes padding and its spaces stay in their own element", () => {
		wrap(block("one\n  two"));

		expect(lines()[1].getAttribute("style")).toBe("--indent:2ch");
		expect(lines()[1].querySelector(".code-block__indent")?.textContent).toBe("  ");
	});

	test("an empty line holds its height with a zero width space", () => {
		wrap(block("one\n\ntwo"));

		expect(lines()[1].textContent).toBe("\u200B\n");
	});
});

describe("words on a line", () => {
	test("words are kept apart", () => {
		expect(firstLine("one two")).toBe(WORD + "one</span> " + WORD + "two</span>");
	});

	test("a key stays with its value", () => {
		expect(firstLine('"a": 1')).toBe(WORD + '"a": 1</span>');
	});

	test("a closing bracket stays with the word before it", () => {
		expect(firstLine("one }")).toBe(WORD + "one }</span>");
		expect(firstLine("one ],")).toBe(WORD + "one ],</span>");
	});

	test("a line-continuing backslash stays with the word before it", () => {
		expect(firstLine("one \\")).toBe(WORD + "one \\</span>");
	});

	test("a hyphenated word is kept whole", () => {
		expect(firstLine("pre-wrap")).toBe(WORD + WORD + "pre-wrap</span></span>");
	});

	test("a word may break after a comma or a slash", () => {
		expect(firstLine("a,b/c")).toBe(WORD + "a,<wbr>b/<wbr>c</span>");
	});

	test("a word does not break after its last character", () => {
		expect(firstLine("a,")).toBe(WORD + "a,</span>");
	});

	test("markup after a comma may break before it and inside it", () => {
		expect(firstLine('a,<i class="s">b</i>')).toBe(WORD + 'a,<wbr><i class="s"><wbr>b</i></span>');
	});

	test("a closing tag after a comma stays with the comma", () => {
		expect(firstLine('<i class="s">a,</i>')).toBe(WORD + '<i class="s">a,</i></span>');
	});

	test("a span holding a space leaves the line as it is", () => {
		const html = '<span class="s">"a b"</span>';

		expect(firstLine(html)).toBe(html);
	});

	test("space before the first word and after the last stays outside", () => {
		expect(firstLine("\tone  ")).toBe("\t" + WORD + "one</span>  ");
	});
});

describe("shell blocks", () => {
	function shell(lang: string, attribute = ""): string {
		return "<div data-code-block><span data-code-lang>" + lang + "</span><pre><code" + attribute + ">npm i</code></pre></div>";
	}

	test.each(["bash", "cmd", "Bash", " CMD "])("%s wraps at every width", (lang) => {
		wrap(shell(lang));

		expect(pre().classList.contains("code-block__pre--wrap")).toBe(true);
		expect(pre().hasAttribute("data-wrap")).toBe(true);
	});

	test("it still gets its line elements", () => {
		wrap(shell("bash"));

		expect(lines()).toHaveLength(1);
	});

	test("another language does not", () => {
		wrap(shell("json"));

		expect(pre().hasAttribute("data-wrap")).toBe(false);
	});

	test("data-lang on the code wins over the label", () => {
		wrap(shell("json", ' data-lang="bash"'));

		expect(pre().hasAttribute("data-wrap")).toBe(true);
	});

	test("a block with no label does not", () => {
		wrap(block("npm i"));

		expect(pre().hasAttribute("data-wrap")).toBe(false);
	});
});
