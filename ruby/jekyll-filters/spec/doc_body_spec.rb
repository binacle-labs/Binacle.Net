# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::SiteFilters::DocBody do
  let(:filter) { Class.new { include Jekyll::SiteFilters::DocBody }.new }

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

    it 'escapes the code it moves' do
      expect(body(rouge('text', '&lt;b&gt;')).at_css('code').text).to eq('<b>')
    end
  end

  describe 'example heads' do
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

  it 'gives empty output for nil' do
    expect(filter.doc_body(nil)).to eq('')
  end

  it 'leaves doc_sections able to read the result' do
    sections = Class.new { include Jekyll::SiteFilters::Headings }.new

    expect(sections.doc_sections(filter.doc_body('<h2 id="-a">🚀 A</h2>'))).to eq([{ 'id' => '-a', 'title' => 'A' }])
  end
end
