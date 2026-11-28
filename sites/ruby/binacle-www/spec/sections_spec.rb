# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Www::Sections do
  let(:filter) { Class.new { include Binacle::Www::Sections }.new }

  def page(html)
    Nokogiri::HTML5.fragment(filter.www_sections(html))
  end

  it 'is a Liquid filter' do
    html = Liquid::Template.parse('{{ html | www_sections }}').render('html' => '<h2>A</h2>')

    expect(html).to include('<section class="band">')
  end

  describe 'what it leaves alone' do
    it 'keeps an empty page' do
      expect(filter.www_sections(nil)).to eq('')
    end
  end

  describe 'the looks' do
    let(:sass) do
      root = File.expand_path('../../../../shared/packages/design-system', __dir__)
      parts = %w[layout/hero components/statement layout/columns]
      parts.map { |part| File.read(File.join(root, part, '_index.scss')) }.join
    end

    it 'stops on one it does not know, and names it' do
      expect { page('<h2 class="band--tnt">A</h2>') }.to raise_error(ArgumentError, /band--tnt, on: A/)
      expect { page('<h2 class="heroo">A</h2>') }.to raise_error(ArgumentError, /heroo, on: A/)
    end

    it 'are all in the design system' do
      converter = described_class::Converter
      looks = [converter::STATEMENT, 'hero__grid', *converter::HERO_PARTS.grep(/hero/)]

      expect(looks.reject { |look| sass.match?(/\.#{look}(?![\w-])/) }).to be_empty
    end

    it 'has the columns look in the design system' do
      expect(sass).to match(/\.#{described_class::Converter::COLUMNS}(?![\w-])/)
    end
  end

  describe 'columns' do
    let(:html) do
      page('<h2 class="band--wide columns">A</h2><p>a</p><h3>One</h3><p>1</p>' \
           '<ul class="labelled-list"><li>x</li></ul><h3>Two</h3><p>2</p><hr><p>after</p>')
    end

    it 'has one for each h3, between the words before and the words after the rule' do
      expect(html.at_css('section')['class']).to eq('band band--wide')
      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[prose columns prose])
      expect(html.css('.columns__column > h3').map(&:text)).to eq(%w[One Two])
      expect(html.at_css('hr')).to be_nil
    end

    it 'names what is in them' do
      column = html.at_css('.columns__column')

      expect(column.at_css('p')['class']).to eq('columns__text')
      expect(column.at_css('ul')['class']).to eq('labelled-list labelled-list--small')
    end

    it 'run to the end when there is no rule' do
      html = page('<h2 class="columns">A</h2><h3>One</h3><p>1</p>')

      expect(html.css('.band__body > *').last['class']).to eq('columns')
    end

    it 'stops at a second rule, and at a rule in a section with no columns' do
      second = '<h2 class="columns">A</h2><h3>One</h3><hr><p>a</p><hr>'

      expect { page(second) }.to raise_error(ArgumentError, /only ends columns, in: A/)
      expect { page('<h2>B</h2><p>b</p><hr>') }.to raise_error(ArgumentError, /only ends columns, in: B/)
    end

    it 'stops when there is no h3' do
      expect { page('<h2 class="columns">A</h2><p>a</p>') }.to raise_error(ArgumentError, /no h3 in: A/)
    end
  end

  describe 'labelled lists' do
    it 'takes the item that opens in bold as the cost' do
      html = page('<h2>A</h2><ul class="labelled-list"><li>gets</li><li><strong>Costs</strong> time</li></ul>')

      expect(html.css('li').map(&:to_html)).to eq(
        ['<li class="labelled-list__item">gets</li>',
         '<li class="labelled-list__item labelled-list__cost"><span class="label">Costs</span>time</li>']
      )
    end

    it 'leaves a plain list alone' do
      item = page('<h2>A</h2><ul><li><strong>b</strong> c</li></ul>').at_css('li')

      expect(item.to_html).to eq('<li><strong>b</strong> c</li>')
    end
  end

  describe 'statements' do
    it 'are the heading at display size, a span for each sentence, and the words under it' do
      html = page('<h2 class="statement band--deep">A yes. Not a <em>no</em>.</h2><p class="muted">a</p>')

      expect(html.at_css('section')['class'].split).to contain_exactly('band', 'statement', 'band--deep')
      expect(html.at_css('section > .container > p.display-2').inner_html)
        .to eq('<span>A yes.</span> <span>Not a <em>no</em>.</span>')
      expect(html.at_css('section > .container > p:last-child').classes).to contain_exactly('statement__text', 'muted')
    end

    it 'stop on a label, and on a look that is not a band' do
      labelled = '<h2 class="statement" data-label="L">A</h2>'

      expect { page(labelled) }.to raise_error(ArgumentError, /only band looks, on: A/)
      expect { page('<h2 class="statement columns">B</h2>') }.to raise_error(ArgumentError, /only band looks, on: B/)
    end
  end

  describe 'heroes' do
    let(:html) do
      page('<h1 class="hero">A</h1><p>a</p><p class="muted">b</p><p class="actions">c</p><div class="command"></div>' \
           '<p class="hero__source">d</p><figure></figure>')
    end

    it 'have the words and the actions on one side and the picture on the other' do
      expect(html.at_css('section')['class']).to eq('band hero')
      expect(html.css('.container.hero__grid--picture-first > *').map { |block| block['class'] })
        .to eq(%w[hero__copy hero__picture])
      expect(html.css('.hero__copy > *').map { |block| block['class'] })
        .to eq(['display-1', 'prose prose--marketing', 'hero__actions', 'hero__source'])
    end

    it 'keep the words together, and the actions together' do
      expect(html.css('.prose > *').map(&:text)).to eq(%w[a b])
      expect(html.css('.hero__actions > *').map { |block| block['class'] }).to eq(%w[actions command])
    end

    it 'stop with no picture, and on a label' do
      expect { page('<h1 class="hero">A</h1><p>a</p>') }.to raise_error(ArgumentError, /one picture, on: A/)
      labelled = '<h1 class="hero" data-label="L">B</h1><figure></figure>'
      expect { page(labelled) }.to raise_error(ArgumentError, /only band looks, on: B/)
    end
  end

  describe 'paths' do
    let(:list) { '<ol class="path" aria-label="Steps"><li><a href="#b">B</a></li></ol>' }

    it 'are the row of steps after their section' do
      html = page("<h1>A</h1><p>a</p>#{list}<h2>C</h2>")

      expect(html.element_children.map(&:name)).to eq(%w[section nav section])
      expect(html.at_css('nav')['aria-label']).to eq('Steps')
      expect(html.at_css('nav.band.path > .container > ol.path__list').inner_html)
        .to eq('<li class="path__item"><a href="#b" class="path__link">B</a></li>')
    end

    it 'stop on an item that is not only a link' do
      more = '<h1>A</h1><ol class="path"><li><a href="#b">B</a> lost</li></ol>'

      expect { page(more) }.to raise_error(ArgumentError, /B lost/)
    end
  end

  describe 'code blocks' do
    let(:rouge) do
      '<div class="language-json highlighter-rouge"%s><div class="highlight"><pre class="highlight">' \
        "<code><span class=\"nl\">\"a\"</span>\n&lt;b&gt;\n</code></pre></div></div>"
    end

    it 'have a head with their name' do
      block = page("<h2>A</h2>#{format(rouge, ' data-name="Signature"')}").at_css('.band__body > .code-block')

      expect(block.key?('data-code-block')).to be(true)
      expect(block.at_css('.code-block__head > [data-code-lang]').text).to eq('Signature')
      expect(block.at_css('.code-block__head > button')['class']).to eq('copy-button')
    end

    it 'have only the copy button without one' do
      block = page("<h2>A</h2>#{format(rouge, '')}").at_css('.code-block')

      expect(block.at_css('.code-block__head')).to be_nil
      expect(block.at_css('> button')['class']).to eq('copy-button code-block__copy')
    end

    it 'keep the code as plain text, with no line at the end' do
      code = page("<h2>A</h2>#{format(rouge, '')}").at_css('pre.code-block__pre > code')

      expect(code.to_html).to eq("<code>\"a\"\n&lt;b&gt;</code>")
    end

    it 'escape the name' do
      block = page("<h2>A</h2>#{format(rouge, ' data-name="a &lt;b&gt;"')}").at_css('.code-block')

      expect(block.at_css('[data-code-lang]').inner_html).to eq('a &lt;b&gt;')
    end

    it 'leave the code of an include alone' do
      html = page('<h2>A</h2><pre class="command__pre"><code id="run">a</code></pre>')

      expect(html.at_css('.code-block')).to be_nil
    end
  end

  describe 'tables' do
    let(:table) do
      page('<h2>A</h2><table><thead><tr><th>Yes</th><th>No</th></tr></thead><tbody>' \
           '<tr><td>a</td><td><em class="table__off">b</em></td></tr>' \
           '<tr><td>c</td><td><em class="table__off">d</em> e</td></tr></tbody></table>').at_css('table')
    end

    it 'are the design system\'s' do
      expect(table['class']).to eq('table')
      expect(table.key?('data-table')).to be(true)
      expect(table.css('th').map { |cell| cell['scope'] }).to eq(%w[col col])
    end

    it 'say the column in every cell' do
      expect(table.css('td').map { |cell| cell['data-label'] }).to eq(%w[Yes No Yes No])
      expect(table.css('td').map { |cell| cell.classes.first }.uniq).to eq(%w[table__text])
    end

    it 'grey out a cell that is only emphasis marked table__off' do
      expect(table.css('tr:first-child td').last.to_html)
        .to eq('<td class="table__text table__off" data-label="No">b</td>')
      expect(table.css('tr:last-child td').last['class']).to eq('table__text')
    end

    it 'set a cell that is only code as a value' do
      html = page('<h2>A</h2><table><tbody><tr><td><code>a</code></td><td><code>b</code> c</td></tr></tbody></table>')

      expect(html.css('td').map(&:to_html))
        .to eq(['<td class="mono">a</td>', '<td class="table__text"><code>b</code> c</td>'])
    end

    it 'are left alone in an include' do
      html = page('<h2>A</h2><table class="x"><tbody><tr><td>a</td></tr></tbody></table>')

      expect(html.at_css('td').key?('class')).to be(false)
    end
  end

  describe 'doors' do
    it 'are links with a title and a text' do
      html = page('<h2>A</h2><ul class="doors"><li><a href="/b/">B</a> - the b</li></ul>')

      expect(html.at_css('nav.doors > a').to_html).to eq(
        '<a href="/b/" class="doors__link"><h3 class="doors__title">B</h3><p class="doors__text">the b</p></a>'
      )
    end

    it 'take the picture written before the link' do
      html = page('<h2>A</h2><ul class="doors doors--pictured"><li><figure><svg></svg></figure>' \
                  "\n<a href=\"/b/\">B</a> - the b</li></ul>")

      expect(html.at_css('nav.doors--pictured > a.doors__link').inner_html).to eq(
        '<figure class="doors__picture"><svg></svg></figure>' \
        '<h3 class="doors__title">B</h3><p class="doors__text">the b</p>'
      )
    end

    it 'are named by the heading above them, and keep the classes the markdown wrote' do
      nav = page('<h2>A</h2><ul class="doors doors--pictured"><li><a href="/b/">B</a> - the b</li></ul>').at_css('nav')

      expect(nav['aria-label']).to eq('A')
      expect(nav['class']).to eq('doors doors--pictured')
    end

    it 'stop on an item written any other way' do
      no_dash = '<h2>A</h2><ul class="doors"><li><a href="/">B</a> b</li></ul>'

      expect { page('<h2>A</h2><ul class="doors"><li>lost</li></ul>') }.to raise_error(ArgumentError, /lost/)
      expect { page(no_dash) }.to raise_error(ArgumentError, /B b/)
    end
  end

  it 'takes the highlighter off code in a line' do
    html = page('<h2>A</h2><p><code class="language-plaintext highlighter-rouge">a</code></p>')

    expect(html.at_css('p').inner_html).to eq('<code>a</code>')
  end

  describe 'verdicts' do
    it 'goes into the figure or the exchange before it' do
      html = page('<h2>A</h2><figure></figure><p class="verdict">a</p>' \
                  '<div class="exchange"></div><p class="verdict">b</p>')

      expect(html.at_css('figure > .verdict').text).to eq('a')
      expect(html.at_css('.exchange > .verdict').text).to eq('b')
    end

    it 'is the caption of a fit-row' do
      html = page('<h2>A</h2><figure class="fit-row"></figure><p class="verdict">a</p>')

      expect(html.at_css('figure').inner_html).to eq('<figcaption class="fit-row__caption verdict">a</figcaption>')
    end

    it 'stands on its own after anything else' do
      html = page('<h2>A</h2><p>a</p><div class="code-block"></div><p class="verdict">b</p>')

      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[prose code-block verdict])
    end
  end
end
