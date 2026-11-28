# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::SitesCore::Sections do
  def page(html)
    Nokogiri::HTML5.fragment(described_class.new(html).to_html)
  end

  describe 'what it leaves alone' do
    it 'keeps a page with no heading' do
      expect(described_class.new('<p>a</p>').to_html).to eq('<p>a</p>')
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

    it 'has the grid only when there is a label, or a band with one column' do
      expect(page('<h2>A</h2>').at_css('section > div')['class']).to eq('container')
      expect(page('<h2 class="band--tint">A</h2>').at_css('section > div')['class']).to eq('container')
      expect(page('<h2 data-label="">A</h2>').at_css('section > div')['class']).to eq('container band__grid')
      expect(page('<h2 class="band--wide">A</h2>').at_css('section > div')['class']).to eq('container band__grid')
    end

    it 'puts the label over the heading when it is mirrored' do
      html = page('<h2 class="band--mirrored split--text-right" data-label="L">A</h2><p>a</p><figure></figure>')

      expect(html.at_css('.band__label')).to be_nil
      expect(html.css('.prose > *').map(&:text)).to eq(%w[L A a])
      expect(html.at_css('.prose > p')['class']).to eq('label')
    end

    it 'stops on a page that starts with anything else' do
      expect { page('<p>lost</p><h2>A</h2>') }.to raise_error(ArgumentError, /lost/)
    end

    it 'stops on a --- that no site part takes' do
      expect { page('<h2>B</h2><p>b</p><hr>') }.to raise_error(ArgumentError, /only ends columns, in: B/)
    end
  end

  describe 'the looks' do
    let(:sass) do
      root = File.expand_path('../../../../shared/packages/design-system', __dir__)
      parts = %w[layout/band layout/split]
      parts.map { |part| File.read(File.join(root, part, '_index.scss')) }.join
    end

    it 'stops on one it does not know, and names it' do
      expect { page('<h2 class="band--tnt">A</h2>') }.to raise_error(ArgumentError, /band--tnt, on: A/)
      expect { page('<h1 class="split--left">A</h1>') }.to raise_error(ArgumentError, /split--left, on: A/)
    end

    it 'stops on a look only a site adds' do
      expect { page('<h2 class="columns">A</h2>') }.to raise_error(ArgumentError, /columns, on: A/)
    end

    it 'are all in the design system' do
      looks = [*described_class::BANDS, *described_class::SPLITS, *described_class::BODIES, described_class::ONE_LINE]

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

    it 'boxes what is beside the words of a split when it is more than one block' do
      html = page('<h1 class="split--text-left">A</h1><figure></figure><div class="command"></div>')

      expect(html.css('.split > *').map { |block| block['class'] }).to eq(%w[prose band__body])
      expect(html.css('.band__body .band__body > *').map(&:name)).to eq(%w[figure div])
    end

    it 'boxes the words after a block of a split, inside the box beside' do
      html = page('<h2 class="split--text-right">A</h2><p>a</p><div class="command"></div><p>b</p>')

      expect(html.css('.split > *').map { |block| block['class'] }).to eq(['prose split__text', 'band__body'])
      expect(html.css('.band__body .band__body > *').map { |block| block['class'] }).to eq(%w[command prose])
    end

    it 'leaves a heading on its own out of the box' do
      html = page('<h2 data-label="L">A</h2><div class="command"></div>')

      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(%w[band__heading command])
    end

    it 'names the one line under a heading' do
      html = page('<h2 class="band--wide prose--one-line">A</h2><p>a</p><div class="command"></div><p>b</p>')

      expect(html.css('.band__body > *').map { |block| block['class'] })
        .to eq(['prose prose--one-line', 'command', 'prose'])
      expect(html.css('p').map { |text| text['class'] }).to eq(['band__text', nil])
    end

    it 'names the words of a split that has them on the right' do
      html = page('<h2 class="split--text-right">A</h2><p>a</p><figure></figure>')

      expect(html.at_css('.split > div')['class']).to eq('prose split__text')
    end

    it 'takes the code to the left when the heading says so' do
      html = page('<h2 class="band--wide band__body--code-left">A</h2><p>a</p><pre><code>b</code></pre>')

      expect(html.at_css('section')['class']).to eq('band band--wide')
      expect(html.at_css('.band__body')['class']).to eq('band__body band__body--code-left')
    end

    it 'leaves an h1 plain when nothing is split' do
      expect(page('<h1>A</h1>').at_css('h1').key?('class')).to be(false)
    end
  end

  describe 'a site that extends it' do
    let(:site) do
      Class.new(described_class) do
        def site_looks = ['mine']

        def site_section(blocks, looks, _label, _id)
          %(<aside>#{blocks.first.text}</aside>) if looks.include?('mine')
        end
      end
    end

    it 'may add looks, and build a section its own way' do
      html = site.new('<h2 class="mine">A</h2><h2>B</h2>').to_html

      expect(html).to include('<aside>A</aside>', '<section class="band">')
    end

    it 'still stops on a look that nobody knows' do
      expect { site.new('<h2 class="mien">A</h2>').to_html }.to raise_error(ArgumentError, /mien, on: A/)
    end
  end
end
