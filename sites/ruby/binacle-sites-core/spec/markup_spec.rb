# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::SitesCore::Markup do
  def fragment(html)
    Nokogiri::HTML5.fragment(html)
  end

  describe '.add_class' do
    it 'sets the class on an element with none' do
      element = fragment('<p>a</p>').at_css('p')
      described_class.add_class(element, 'x')

      expect(element['class']).to eq('x')
    end

    it 'keeps the classes it has' do
      element = fragment('<p class="a">a</p>').at_css('p')
      described_class.add_class(element, 'b')

      expect(element['class']).to eq('a b')
    end
  end

  describe '.name_labelled_lists' do
    it 'takes the item that opens in bold as the cost' do
      html = fragment('<ul class="labelled-list"><li>gets</li><li><strong>Costs</strong> time</li></ul>')
      described_class.name_labelled_lists(html)

      expect(html.css('li').map(&:to_html)).to eq(
        ['<li class="labelled-list__item">gets</li>',
         '<li class="labelled-list__item labelled-list__cost"><span class="label">Costs</span>time</li>']
      )
    end

    it 'leaves a plain list alone' do
      html = fragment('<ul><li><strong>b</strong> c</li></ul>')
      described_class.name_labelled_lists(html)

      expect(html.at_css('li').to_html).to eq('<li><strong>b</strong> c</li>')
    end
  end

  describe '.grey_out' do
    def cells(html)
      fragment("<table><tbody><tr>#{html}</tr></tbody></table>").css('td')
    end

    it 'greys out a cell that is only emphasis marked table__off' do
      cell = cells('<td><em class="table__off">b</em></td>').first
      described_class.grey_out(cell)

      expect(cell.to_html).to eq('<td class="table__off">b</td>')
    end

    it 'leaves a cell with more than the emphasis' do
      cell = cells('<td><em class="table__off">d</em> e</td>').first
      described_class.grey_out(cell)

      expect(cell.key?('class')).to be(false)
    end
  end

  describe '.name_doors' do
    let(:list) { '<ul class="doors"><li><a href="/api/">API</a> - The <code>v3</code> endpoints.</li></ul>' }

    it 'turns a marked list into a nav of links, each with its title and text' do
      html = fragment(list)
      described_class.name_doors(html)
      link = html.at_css('nav.doors > a.doors__link')

      expect(link['href']).to eq('/api/')
      expect(link.at_css('h3.doors__title').text).to eq('API')
      expect(link.at_css('p.doors__text').inner_html).to eq('The <code>v3</code> endpoints.')
    end

    it 'keeps the classes the list has and gives back the navs' do
      html = fragment(list.sub('"doors"', '"doors doors--pictured"'))
      navs = described_class.name_doors(html)

      expect(navs.map { |nav| nav['class'] }).to eq(['doors doors--pictured'])
    end

    it 'takes the label from the heading above' do
      html = fragment(%(<h2 id="a"> Where next </h2>#{list}))
      described_class.name_doors(html)

      expect(html.at_css('nav')['aria-label']).to eq('Where next')
    end

    it 'takes the label from the nearest heading, with words between' do
      html = fragment(%(<h2>Setups</h2><p>a</p><h3>Six setups</h3><p>b</p>#{list}))
      described_class.name_doors(html)

      expect(html.at_css('nav')['aria-label']).to eq('Six setups')
    end

    it 'has no label with no heading above' do
      html = fragment("<p>a</p>#{list}")
      described_class.name_doors(html)

      expect(html.at_css('nav').key?('aria-label')).to be(false)
    end

    it 'puts a picture before the title' do
      html = fragment('<ul class="doors"><li><figure><svg></svg></figure><a href="/b/">B</a> - b</li></ul>')
      described_class.name_doors(html)

      expect(html.at_css('a.doors__link').inner_html).to eq(
        '<figure class="doors__picture"><svg></svg></figure><h3 class="doors__title">B</h3><p class="doors__text">b</p>'
      )
    end

    it 'leaves an unmarked list a list' do
      html = fragment('<ul><li><a href="/api/">API</a> - text</li></ul>')
      described_class.name_doors(html)

      expect(html.at_css('nav')).to be_nil
    end

    it 'fails on an item with no link, or no text' do
      expect { described_class.name_doors(fragment('<ul class="doors"><li>API - text</li></ul>')) }
        .to raise_error(ArgumentError, /API - text/)
      expect { described_class.name_doors(fragment('<ul class="doors"><li><a href="/">API</a></li></ul>')) }
        .to raise_error(ArgumentError, /API/)
    end
  end

  describe '.name_code_blocks' do
    def rouge(code, attributes = '')
      %(<div class="language-json highlighter-rouge"#{attributes}><div class="highlight"><pre class="highlight">) +
        %(<code class="language-json"><span class="nl">"a"</span>\n#{code}\n</code></pre></div></div>)
    end

    def blocks(html, **options, &)
      page = fragment(html)
      described_class.name_code_blocks(page, selector: 'pre', **options, &)
      page
    end

    it 'wraps the block, with the copy button floating when it has no name' do
      block = blocks(rouge('x')).at_css('div.code-block')

      expect(block.key?('data-code-block')).to be(true)
      expect(block.at_css('pre.code-block__pre')).not_to be_nil
      expect(block.at_css('> button')['class']).to eq('copy-button code-block__copy')
      expect(block.at_css('.code-block__head')).to be_nil
      expect(block.at_css('code').key?('class')).to be(false)
    end

    it 'has a head with the name the markdown gives, escaped' do
      block = blocks(rouge('x', ' data-name="a &lt;b&gt;"')).at_css('.code-block')

      expect(block.at_css('.code-block__head > [data-code-lang]').inner_html).to eq('a &lt;b&gt;')
      expect(block.at_css('.code-block__head > button')['class']).to eq('copy-button')
    end

    it 'drops the empty last line and keeps the spans' do
      code = blocks(rouge('x')).at_css('code')

      expect(code.text).to eq(%("a"\nx))
      expect(code.at_css('span.nl')).not_to be_nil
    end

    it 'strips the spans when asked for plain text' do
      code = blocks(rouge('x'), plain: true).at_css('code')

      expect(code.to_html).to eq(%(<code>"a"\nx</code>))
    end

    it 'lets the caller work on the block and give its own head' do
      page = blocks(rouge('x')) do |pre, code, outer|
        pre['data-seen'] = code['class'] + outer['class'][/language-\w+/]
        described_class.code_head(described_class.code_label('mine'))
      end

      expect(page.at_css('pre')['data-seen']).to eq('language-jsonlanguage-json')
      expect(page.at_css('.code-block__head > [data-code-lang]').text).to eq('mine')
    end

    it 'only wraps what the selector finds' do
      page = fragment('<pre class="x"><code>a</code></pre><pre><code>b</code></pre>')
      described_class.name_code_blocks(page, selector: 'pre:not([class])')

      expect(page.css('.code-block').size).to eq(1)
    end

    it 'leaves a pre with no code' do
      expect(blocks('<pre>a</pre>').at_css('.code-block')).to be_nil
    end
  end

  describe '.name_tables' do
    def tables(html, selector: 'table', cell: nil)
      page = fragment(html)
      described_class.name_tables(page, selector: selector, cell: cell)
      page
    end

    let(:table) do
      '<table><thead><tr><th>Yes</th><th style="text-align: right">No</th></tr></thead><tbody>' \
        '<tr><td>a</td><td style="text-align: right"><em class="table__off">b</em></td></tr></tbody></table>'
    end

    it 'gives the table its class and the header cells their scope' do
      html = tables(table)

      expect(html.at_css('table')['class']).to eq('table')
      expect(html.at_css('table').key?('data-table')).to be(true)
      expect(html.css('th').map { |cell| cell['scope'] }).to eq(%w[col col])
    end

    it 'keeps a class the table has' do
      expect(tables(table.sub('<table', '<table class="x"')).at_css('table')['class']).to eq('x table')
    end

    it 'says the column in every body cell' do
      expect(tables(table).css('td').map { |cell| cell['data-label'] }).to eq(%w[Yes No])
    end

    it 'gives a right-aligned cell the number class and no style' do
      html = tables(table)

      expect(html.css('th').last.to_html).to eq('<th scope="col" class="table__number">No</th>')
      expect(html.css('td').last['style']).to be_nil
      expect(html.css('td').last['class']).to eq('table__number table__off')
    end

    it 'greys out a cell that is only marked emphasis' do
      expect(tables(table).css('td').last.text).to eq('b')
    end

    it 'runs the caller on each body cell first' do
      html = tables(table, cell: ->(cell) { cell['class'] = 'first' })

      expect(html.css('td').first.to_html).to eq('<td class="first" data-label="Yes">a</td>')
    end

    it 'gives back the tables, and only takes what the selector finds' do
      page = fragment(%(<table class="x"><tr><td>a</td></tr></table><table><tr><td>b</td></tr></table>))
      found = described_class.name_tables(page, selector: 'table:not([class])')

      expect(found.size).to eq(1)
      expect(page.css('table').map { |node| node['class'] }).to eq(['x', 'table'])
    end
  end
end
