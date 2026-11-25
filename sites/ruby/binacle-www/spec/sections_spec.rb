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
    it 'keeps a page that is sections already, byte for byte' do
      html = "<section class=\"band\"><h2>A</h2>&middot;</section>\n"

      expect(filter.www_sections(html)).to eq(html)
    end

    it 'keeps a page with no heading' do
      expect(filter.www_sections('<p>a</p>')).to eq('<p>a</p>')
    end

    it 'keeps an empty page' do
      expect(filter.www_sections(nil)).to eq('')
    end
  end

  describe 'sections' do
    it 'starts one at every h1 and h2' do
      html = page('<h1>A</h1><p>a</p><h2>B</h2><p>b</p><h3>c</h3>')

      expect(html.css('section').map { |section| section.at_css('h1, h2').text }).to eq(%w[A B])
      expect(html.css('section').last.at_css('h3').text).to eq('c')
    end

    it 'takes its look and its id from the heading' do
      html = page('<h2 class="band--wide band--tint" id="run">A</h2>')

      expect(html.at_css('section')['class']).to eq('band band--wide band--tint')
      expect(html.at_css('section')['id']).to eq('run')
      expect(html.at_css('h2').to_html).to eq('<h2 class="band__heading">A</h2>')
    end

    it 'has no id when the heading has none' do
      expect(page('<h2>A</h2>').at_css('section').key?('id')).to be(false)
    end

    it 'puts the label in its own column' do
      html = page('<h2 data-label="Fit &amp; pack">A</h2>')

      expect(html.at_css('.band__grid > .band__label > p.label').text).to eq('Fit & pack')
      expect(html.at_css('h2').key?('data-label')).to be(false)
    end

    it 'keeps the label column when the label is empty, and has none without one' do
      expect(page('<h2 data-label="">A</h2>').at_css('.band__label').element_children).to be_empty
      expect(page('<h2>A</h2>').at_css('.band__label')).to be_nil
    end

    it 'stops on a page that starts with anything else' do
      expect { page('<p>lost</p><h2>A</h2>') }.to raise_error(ArgumentError, /lost/)
    end
  end

  describe 'the looks' do
    let(:sass) do
      root = File.expand_path('../../../../shared/packages/design-system/layout', __dir__)
      %w[band split].map { |name| File.read(File.join(root, name, '_index.scss')) }.join
    end

    it 'stops on one it does not know, and names it' do
      expect { page('<h2 class="band--tnt">A</h2>') }.to raise_error(ArgumentError, /band--tnt, on: A/)
      expect { page('<h1 class="split--left">A</h1>') }.to raise_error(ArgumentError, /split--left, on: A/)
    end

    it 'are all in the design system' do
      looks = described_class::Converter::BANDS + described_class::Converter::SPLITS

      expect(looks.reject { |look| sass.match?(/\.#{look}(?![\w-])/) }).to be_empty
    end
  end

  describe 'the body' do
    it 'is prose when it is words from end to end' do
      html = page('<h2>A</h2><p>a</p><ul><li>b</li></ul>')

      expect(html.at_css('.band__body')['class']).to eq('band__body prose')
      expect(html.css('.band__body > *').map(&:name)).to eq(%w[h2 p ul])
    end

    it 'boxes each run of words around anything else' do
      html = page('<h2>A</h2><p>a</p><div class="command"></div><p>b</p>')

      expect(html.at_css('.band__body')['class']).to eq('band__body')
      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[prose command prose])
    end

    it 'keeps a block that brings its own prose out of the box' do
      html = page('<h2>A</h2><p>a</p><p class="prose note">b</p>')

      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(['prose', 'prose note'])
    end

    it 'splits when the heading says so' do
      html = page('<h1 class="band--split split--text-left">A</h1><p>a</p><figure></figure>')

      expect(html.at_css('section')['class']).to eq('band band--split')
      expect(html.at_css('.band__body')['class']).to eq('band__body split split--text-left')
      expect(html.css('.band__body > *').map(&:name)).to eq(%w[div figure])
      expect(html.at_css('h1')['class']).to eq('split__title')
    end

    it 'boxes the words of a split that has nothing else' do
      html = page('<h1 class="split--text-left">A</h1><p>a</p>')

      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[prose])
    end

    it 'leaves an h1 plain when nothing is split' do
      expect(page('<h1>A</h1>').at_css('h1').key?('class')).to be(false)
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

    it 'stands on its own after anything else' do
      html = page('<h2>A</h2><p>a</p><div class="code-block"></div><p class="verdict">b</p>')

      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[prose code-block verdict])
    end
  end
end
