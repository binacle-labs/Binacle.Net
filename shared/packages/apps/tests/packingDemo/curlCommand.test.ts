import {curlCommand} from "../../src/packingDemo/curlCommand";
import type {SentRequest} from "../../src/packingDemo/packingDemo";

const origin = "https://binacle.test";

function requestWith(body: unknown, path = "/api/pack"): SentRequest {
	return {method: "POST", path, body} as SentRequest;
}

function lineElements(html: string): string[] {
	return html.match(/<span class="code-block__line"[\s\S]*?(?=<span class="code-block__line"|$)/g) ?? [];
}

function plainText(html: string): string {
	return html
		.replace(/<[^>]*>/g, "")
		.replace(/&lt;/g, "<")
		.replace(/&amp;/g, "&");
}

test("the first line is the method and the absolute address", () => {
	const html = curlCommand(requestWith({}, "/api/v3/pack"), origin);

	const first = lineElements(html)[0];

	expect(plainText(first)).toBe("curl -X POST https://binacle.test/api/v3/pack \\\n");
});

test("the method comes from the request", () => {
	const request = {method: "PUT", path: "/x", body: {}} as unknown as SentRequest;

	const html = curlCommand(request, origin);

	expect(plainText(html)).toContain("curl -X PUT https://binacle.test/x \\");
});

test("the closing backslash stays with the word before it", () => {
	const html = curlCommand(requestWith({}), origin);

	expect(html).toContain('<span class="code-block__word">https:/<wbr>/<wbr>binacle.test/<wbr>api/<wbr>pack \\</span>');
	expect(html).toContain("<span class=\"code-block__word\">application/<wbr>json' \\</span>");
});

test("a break point goes after a slash only", () => {
	const html = curlCommand(requestWith({}, "/a/b"), origin);

	const address = html.match(/<span class="code-block__word">(https[^<]*(?:<wbr>[^<]*)*) \\<\/span>/)![1];

	expect(address).toBe("https:/<wbr>/<wbr>binacle.test/<wbr>a/<wbr>b");
	expect(html.match(/<wbr>/g)!.length).toBe(5);
});

test("an ampersand and a less-than are escaped in strings", () => {
	const html = curlCommand(requestWith({id: "a&b<c"}), origin);

	expect(html).toContain("a&amp;b&lt;c");
	expect(html).not.toContain("a&b");
	expect(html).not.toContain("b<c");
});

test("a single quote in the body is escaped for a shell", () => {
	const html = curlCommand(requestWith({id: "it's"}), origin);

	expect(plainText(html)).toContain(`"it'\\''s"`);
});

test("a key gets the property class and its colon the punctuation class", () => {
	const html = curlCommand(requestWith({id: "x"}), origin);

	expect(html).toContain('<span class="code-block__property">"id"</span><span class="code-block__punctuation">:</span>');
});

test("a string value is not weighted", () => {
	const html = curlCommand(requestWith({id: "x"}), origin);

	expect(html).toContain(' "x"');
	expect(html).not.toContain('<span class="code-block__property">"x"');
});

test("numbers are weighted, negative and decimal included", () => {
	const html = curlCommand(requestWith({a: 7, b: -3, c: 2.5}), origin);

	expect(html).toContain('<span class="code-block__number">7</span>');
	expect(html).toContain('<span class="code-block__number">-3</span>');
	expect(html).toContain('<span class="code-block__number">2.5</span>');
});

test("braces, brackets and commas are punctuation", () => {
	const html = curlCommand(requestWith({a: [1, 2]}), origin);

	for (const mark of ["{", "[", "]", ",", "}"]) {
		expect(html).toContain(`<span class="code-block__punctuation">${mark}</span>`);
	}
});

test("digits inside a string are not weighted as numbers", () => {
	const html = curlCommand(requestWith({id: "item-12 and 3.5"}), origin);

	expect(html).toContain('"item-12 and 3.5"');
	expect(html).not.toContain("code-block__number");
});

test("each line carries its indent and the indent is its own element", () => {
	const html = curlCommand(requestWith({a: {b: 1}}), origin);

	const lines = lineElements(html);

	expect(lines[0]).toContain('style="--indent:0ch"><span class="code-block__indent"></span>');
	expect(lines[1]).toContain('style="--indent:2ch"><span class="code-block__indent">  </span>');
	expect(lines[3]).toContain('style="--indent:2ch"');
	expect(lines[4]).toContain('style="--indent:4ch"><span class="code-block__indent">    </span>');
});

test("the last line has no newline and the others do", () => {
	const html = curlCommand(requestWith({a: 1}), origin);

	const lines = lineElements(html);

	for (const line of lines.slice(0, -1)) {
		expect(line.endsWith("\n</span>")).toBe(true);
	}
	expect(lines[lines.length - 1].endsWith("\n</span>")).toBe(false);
});

test("stripping the tags and unescaping gives back the plain command", () => {
	const body = {
		parameters: {algorithm: "FirstFit"},
		bins: [{id: "bin-1", length: 10, width: -2.5, height: 30}],
		items: [
			{id: `it's <b> & "quoted" \\ back/slash`, note: "1,2 {x} [y]: z", list: [1, [2, {k: null}]], ok: true},
			{id: "a&amp;b &lt; c", empty: {}, none: []}
		]
	};
	const expected = [
		"curl -X POST https://binacle.test/api/pack \\",
		"  -H 'Content-Type: application/json' \\",
		`  -d '${JSON.stringify(body, null, 2).replace(/'/g, "'\\''")}'`
	].join("\n");

	const html = curlCommand(requestWith(body), origin);

	expect(plainText(html)).toBe(expected);
});
