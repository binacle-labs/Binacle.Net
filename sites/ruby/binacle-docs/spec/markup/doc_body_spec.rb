# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Docs::Markup::DocBody do
  let(:filter) { Class.new { include Binacle::Docs::Markup::DocBody }.new }

  def body(html)
    Nokogiri::HTML5.fragment(filter.doc_body(html))
  end

  def rouge(language, code)
    %(<div class="language-#{language} highlighter-rouge"><div class="highlight"><pre class="highlight"><code>#{code}</code></pre></div></div>)
  end

  describe 'opening emoji' do
    it 'drops the one that opens a heading and keeps the id' do
      heading = body('<h2 id="-setup">🚀 Setup</h2>').at_css('h2')

      expect(heading.text).to eq('Setup')
      expect(heading['id']).to eq('-setup')
    end

    it 'drops one with a variation selector' do
      expect(body('<h3 id="a">⚙️ Settings</h3>').text).to eq('Settings')
    end

    it 'drops a keycap' do
      expect(body('<p>1️⃣ First</p>').text).to eq('First')
    end

    it 'drops one that opens a bullet, and one that opens the link a bullet starts with' do
      html = body('<ul><li>📄 Text</li><li><a href="/a">📄 Link</a></li></ul>')

      expect(html.css('li').map(&:text)).to eq(%w[Text Link])
    end

    it 'keeps one later in the line, and a number that is not a keycap' do
      expect(body('<p>2 boxes 📦 here</p>').text).to eq('2 boxes 📦 here')
    end
  end

  describe 'rules' do
    it 'drops one before a section or at the end' do
      expect(body('<p>a</p><hr><h2 id="b">b</h2><p>c</p><hr>').css('hr')).to be_empty
    end

    it 'keeps one between paragraphs' do
      expect(body('<p>a</p><hr><p>b</p>').at_css('hr')['class']).to eq('doc__rule')
    end
  end

  describe 'callouts' do
    it 'turns a marked quote into a callout with its kind as the label' do
      callout = body('<blockquote class="block-note"><p>Mind this</p></blockquote>').at_css('div')

      expect(callout['class']).to eq('callout callout--note')
      expect(callout.at_css('.label').text).to eq('Note')
      expect(callout.css('p').last.text).to eq('Mind this')
    end

    it 'turns a GitHub alert into a callout, in any case' do
      callout = body("<blockquote><p>[!Note]\nMind <code>this</code></p></blockquote>").at_css('div')

      expect(callout['class']).to eq('callout callout--note')
      expect(callout.at_css('.label').text).to eq('Note')
      expect(callout.css('p').last.inner_html).to eq('Mind <code>this</code>')
    end

    it 'leaves a plain quote a quote' do
      expect(body('<blockquote><p>Said</p></blockquote>').at_css('blockquote')['class']).to eq('doc__quote')
    end
  end

  describe 'code blocks' do
    it 'gives a block its head and keeps the code' do
      html = body(rouge('bash', %(<span class="nb">echo</span> hi\n)))

      expect(html.at_css('[data-code-block] [data-code-lang]').text).to eq('bash')
      expect(html.at_css('[data-code-block] button[data-copy]')).not_to be_nil
      expect(html.at_css('pre.code-block__pre code[data-lang=bash]').text).to eq("echo hi\n")
      expect(html.css('.highlight')).to be_empty
    end

    it 'calls yml yaml and a block with no language text' do
      expect(body(rouge('yml', 'a: 1')).at_css('[data-code-lang]').text).to eq('yaml')
      expect(body('<pre><code>a</code></pre>').at_css('[data-code-lang]').text).to eq('text')
    end

    it 'finds a language Rouge has no lexer for' do
      html = body('<pre><code class="language-cmd">dir</code></pre>')

      expect(html.at_css('[data-code-lang]').text).to eq('cmd')
      expect(html.at_css('code')['data-lang']).to eq('cmd')
    end

    it 'marks the parts of JSON that carry weight and unwraps the rest' do
      code = body(rouge('json', '<span class="nl">"a"</span><span class="p">:</span><span class="w"> </span>' \
                                '<span class="mi">1</span><span class="s2">"b"</span>')).at_css('code')

      expect(code.css('span').map { |span| [span['class'], span.text] }).to eq(
        [['code-block__property', '"a"'], ['code-block__punctuation', ':'], ['code-block__number', '1']]
      )
      expect(code.text).to eq('"a": 1"b"')
    end

    it 'names a file or a field the markdown gives it, in place of the language' do
      block = %(<div class="language-json highlighter-rouge" data-name="Presets.json"><div class="highlight"><pre class="highlight"><code>{}</code></pre></div></div>)

      expect(body(block).at_css('[data-code-lang]').text).to eq('Presets.json')
    end

    it 'wraps a block the markdown marks, and leaves the rest' do
      block = %(<div class="language-text highlighter-rouge" data-wrap=""><div class="highlight"><pre class="highlight"><code>AAAF</code></pre></div></div>)

      expect(filter.doc_body(block)).to include('<pre class="code-block__pre code-block__pre--wrap" data-wrap="">')
      expect(filter.doc_body(block.sub(' data-wrap=""', ''))).not_to include('data-wrap')
    end

    it 'escapes the code it moves' do
      expect(body(rouge('text', '&lt;b&gt;')).at_css('code').text).to eq('<b>')
    end
  end

  describe 'example heads' do
    it 'names a call the markdown gives it' do
      block = %(<div class="language-bash highlighter-rouge" data-call="POST /api/v3/pack"><div class="highlight"><pre class="highlight"><code>curl</code></pre></div></div>)
      line = body(block).at_css('.code-block__head .http-line')

      expect(line.css('.http-line__badge').text).to eq('POST')
      expect(line.css('span').last.text).to eq('/api/v3/pack')
    end

    it 'names a status the markdown gives it' do
      block = %(<div class="language-json highlighter-rouge" data-call="200"><div class="highlight"><pre class="highlight"><code>{}</code></pre></div></div>)
      head = body(block).at_css('.code-block__head')

      expect(head.at_css('.http-line__badge').text).to eq('200')
      expect(head.at_css('[data-code-lang]')).to be_nil
    end

    let(:page) do
      %(<p><code>POST /api/v3/pack</code></p><p><strong>Request Example</strong></p>#{rouge('json', '{}')}) +
        %(<p><strong>Response Example</strong></p>#{rouge('json', '{}')}#{rouge('json', '{}')}) +
        %(<h3 id="n">Next</h3>#{rouge('json', '{}')})
    end

    it 'names the call over a request' do
      expect(body(page).css('.code-block__head').first.text.strip).to start_with('POST/api/v3/pack')
    end

    it 'names the status over a response, and over the JSON after it' do
      expect(body(page).css('.code-block__head .http-line').map(&:text)).to eq(['POST/api/v3/pack', '200', '200'])
    end

    it 'stops at the next heading' do
      expect(body(page).css('.code-block').last.at_css('[data-code-lang]').text).to eq('json')
    end
  end

  describe 'classes' do
    it 'names a table, wherever it sits' do
      table = body('<div><table><tr><td>a</td></tr></table></div>').at_css('table')

      expect(table['class']).to eq('table doc__table')
      expect(table.key?('data-table')).to be(true)
    end

    it 'puts a table in a box that scrolls when the table is too wide' do
      html = body('<table><tr><td>a</td></tr></table>')

      expect(html.at_css('div.doc__table-scroll > table.doc__table')).not_to be_nil
    end

    it 'gives a cell the mono class when all it holds is mono code' do
      cells = body(%(<table><tr><td><a href="#a"><code class="mono">GET</code></a></td><td><code class="mono">0</code> is valid</td><td>text</td></tr></table>)).css('td')

      expect(cells.map { |cell| cell['class'] }).to eq(['mono', nil, nil])
    end

    it 'gives a right-aligned column the number class, and no style' do
      table = body('<table><tr><th style="text-align: right">A</th></tr><tr><td style="text-align: right">1</td><td style="text-align: center">2</td></tr></table>')

      expect(table.at_css('th').to_html).to eq('<th class="table__number">A</th>')
      expect(table.css('td').map { |cell| cell['class'] }).to eq(['table__number', nil])
      expect(table.css('td').last['style']).to eq('text-align: center')
    end

    it 'greys out a cell that is only marked emphasis' do
      cells = body('<table><tr><td><em class="table__off">reserved</em></td><td><em class="table__off">a</em> b</td></tr></table>').css('td')

      expect(cells.first.to_html).to eq('<td class="table__off">reserved</td>')
      expect(cells.last['class']).to be_nil
    end

    it 'names the blocks at the top level and keeps a class they have' do
      html = body('<h2 id="a">A</h2><h5 id="b">B</h5><p class="lede">c</p><ol><li>d<ul><li>e</li></ul></li></ol>')

      expect(html.at_css('h2')['class']).to eq('doc__heading doc__heading--2')
      expect(html.at_css('h5')['class']).to eq('doc__heading doc__heading--4')
      expect(html.at_css('p')['class']).to eq('lede doc__text')
      expect(html.at_css('ol')['class']).to eq('doc__list')
      expect(html.at_css('ul')['class']).to eq('doc__list--nested')
    end

    it 'leaves a paragraph inside something else alone' do
      expect(body('<ul><li><p>a</p></li></ul>').at_css('p')['class']).to be_nil
    end
  end

  describe 'doors' do
    let(:list) { '<ul class="doors"><li><a href="/api/">API</a> - The <code>v3</code> endpoints.</li></ul>' }

    it 'turns a marked list into links, each with its title and text' do
      link = body(list).at_css('nav.doors.doors--full > a.doors__link')

      expect(link['href']).to eq('/api/')
      expect(link.at_css('h3.doors__title').text).to eq('API')
      expect(link.at_css('p.doors__text').inner_html).to eq('The <code>v3</code> endpoints.')
    end

    it 'takes its label from the heading above, without the emoji' do
      nav = body(%(<h2 id="a">➡️ Where to go next</h2>\n#{list})).at_css('nav')

      expect(nav['aria-label']).to eq('Where to go next')
    end

    it 'has no label with no heading above' do
      expect(body("<p>a</p>#{list}").at_css('nav').key?('aria-label')).to be(false)
    end

    it 'leaves an unmarked list a list' do
      expect(body('<ul><li><a href="/api/">API</a> - text</li></ul>').at_css('nav')).to be_nil
    end

    it 'fails on an item with no link, or no text' do
      expect { body('<ul class="doors"><li>API - text</li></ul>') }.to raise_error(ArgumentError, /API - text/)
      expect { body('<ul class="doors"><li><a href="/api/">API</a></li></ul>') }.to raise_error(ArgumentError, /API/)
    end
  end

  describe 'tabs' do
    def tab(code, attributes)
      rouge('bash', code).sub('<div ', "<div #{attributes} ")
    end

    let(:first) { tab('a', 'data-tabs="Operating system" data-tab="Linux &amp; macOS"') }
    let(:second) { tab('b', 'data-tab="Windows"') }

    it 'turns marked code blocks in a row into tabs, with the first one showing' do
      html = body("#{first}\n#{second}")
      box = html.at_css('div.tabs[data-tabs]')

      expect(box.at_css('[role=tablist]')['aria-label']).to eq('Operating system')
      expect(box.css('[role=tab]').map(&:text)).to eq(['Linux & macOS', 'Windows'])
      expect(box.css('[role=tab]').map { |button| button['aria-selected'] }).to eq(%w[true false])
      expect(box.css('[role=tabpanel]').map { |panel| panel.key?('hidden') }).to eq([false, true])
      expect(box.css('[role=tabpanel] > div.code-block code').map(&:text)).to eq(%w[a b])
      expect(html.css('[data-tab]')).to be_empty
    end

    it 'takes a block Rouge has no lexer for' do
      plain = '<pre data-tab="Windows"><code class="language-cmd">b</code></pre>'

      expect(body("#{first}#{plain}").css('[role=tabpanel] code').map { |code| code['data-lang'] }).to eq(%w[bash cmd])
    end

    it 'starts another box at the next data-tabs, and stops at anything else' do
      html = body("#{first}#{second}#{first}<p>a</p>#{rouge('bash', 'c')}")

      expect(html.css('div.tabs').map { |box| box.css('[role=tab]').size }).to eq([2, 1])
      expect(html.element_children.last['class']).to eq('code-block')
    end

    it 'fails on a tab with no box, or a box with an unnamed tab' do
      expect { body(second) }.to raise_error(ArgumentError, /Windows/)
      expect { body(tab('a', 'data-tabs="OS"')) }.to raise_error(ArgumentError, /OS/)
    end
  end

  describe 'http lines' do
    let(:line) { '<p class="http-line"><code>GET /api/{id}</code></p>' }

    it 'shows the method of a marked call as a badge before the path' do
      paragraph = body(line).at_css('p')

      expect(paragraph['class']).to eq('http-line http-line--small doc__text')
      expect(paragraph.css('span').map(&:text)).to eq(['GET', '/api/{id}'])
      expect(paragraph.at_css('span')['class']).to eq('http-line__badge')
      expect(paragraph.at_css('code')).to be_nil
    end

    it 'still names the request example below it' do
      html = body("#{line}<p>Request Example</p>#{rouge('json', '{}')}")

      expect(html.css('.code-block__http-line span').map(&:text)).to eq(['GET', '/api/{id}'])
    end

    it 'fails on a line that is not a call' do
      expect { body('<p class="http-line">Presets</p>') }.to raise_error(ArgumentError, /Presets/)
    end
  end

  describe 'folds' do
    it 'puts a marked code block under its text, closed' do
      html = body(rouge('json', '{}').sub('<div ', '<div data-call="200" data-fold="Response · a &amp; b" '))
      fold = html.at_css('details.disclosure')

      expect(fold.key?('open')).to be(false)
      expect(fold.at_css('summary.disclosure__summary.label').text).to eq('Response · a & b')
      expect(fold.at_css('> div.code-block .http-line__badge').text).to eq('200')
      expect(html.css('[data-fold]')).to be_empty
    end

    it 'takes a block Rouge has no lexer for' do
      html = body('<pre data-fold="More"><code class="language-cmd">a</code></pre>')

      expect(html.at_css('details > div.code-block code')['data-lang']).to eq('cmd')
    end
  end

  describe 'result lists' do
    def results(cells)
      columns = cells.map { |cell| "<td>#{cell}</td>" }.join
      %(<table class="result-list" aria-label="The response"><thead><tr><th>Bin</th></tr></thead><tbody><tr>#{columns}</tr></tbody></table>)
    end

    it 'turns a marked table into result rows, one per row' do
      list = body(results(['bin_1', '10 x 40 x 60', '15.83', 'AllItemsFit', '4 of 4 items'])).at_css('ul')

      expect(list['class']).to eq('result-list result-list--full doc__list')
      expect(list['aria-label']).to eq('The response')
      expect(list.css('li').size).to eq(1)
      expect(list.at_css('.result-row__title').text).to eq('bin_1 · 10 x 40 x 60')
      expect(list.at_css('.result-row__fill-line')['style']).to eq('width:15.83%')
      expect(list.css('.result-row__meta span').map(&:text)).to eq(['AllItemsFit', '4 of 4 items'])
    end

    it 'fails on a row short of a column, or a fill that is not a number' do
      expect { body(results(%w[bin_1 a 1])) }.to raise_error(ArgumentError, /bin_1/)
      expect { body(results(['bin_1', 'a', '15%', 'b', 'c'])) }.to raise_error(ArgumentError, /15%/)
    end
  end

  describe 'labelled lists' do
    let(:items) do
      body('<ul class="labelled-list"><li>Fast.</li><li><strong>Costs</strong> Leaves <code>room</code>.</li></ul>').css('li')
    end

    it 'makes the bold that opens an item its label, and the item the cost' do
      expect(items.last['class']).to eq('labelled-list__item labelled-list__cost')
      expect(items.last.inner_html).to eq('<span class="label">Costs</span>Leaves <code>room</code>.')
    end

    it 'names an item with no bold, and adds nothing to it' do
      expect(items.first['class']).to eq('labelled-list__item')
      expect(items.first.inner_html).to eq('Fast.')
    end

    it 'leaves bold in an unmarked list alone' do
      item = body('<ul><li><strong>Costs</strong> a</li></ul>').at_css('li')

      expect(item['class']).to be_nil
      expect(item.at_css('strong')).not_to be_nil
    end
  end

  describe 'steps' do
    it 'runs a marked list to the edge of the column, and leaves an unmarked one' do
      lists = body('<ol class="steps"><li><h4>a</h4></li></ol><ol><li>b</li></ol>').css('ol')

      expect(lists.map { |list| list['class'] }).to eq(['steps doc__steps doc__list', 'doc__list'])
    end
  end

  it 'gives empty output for nil' do
    expect(filter.doc_body(nil)).to eq('')
  end

  it 'leaves doc_sections able to read the result' do
    sections = Class.new { include Binacle::Docs::Markup::Sections }.new

    expect(sections.doc_sections(filter.doc_body('<h2 id="-a">🚀 A</h2>'))).to eq([{ 'id' => '-a', 'title' => 'A' }])
  end
end
