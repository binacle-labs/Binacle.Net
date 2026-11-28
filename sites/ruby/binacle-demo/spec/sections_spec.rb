# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Demo::Sections do
  let(:filter) { Class.new { include Binacle::Demo::Sections }.new }

  def page(html)
    Nokogiri::HTML5.fragment(filter.demo_sections(html))
  end

  it 'is a Liquid filter' do
    html = Liquid::Template.parse('{{ html | demo_sections }}').render('html' => '<h2>A</h2>')

    expect(html).to include('<section class="band">')
  end

  it 'turns a tile list into a nav named by its heading' do
    html = page(%(<h2 class="band--tint">More</h2><ul class="doors doors--row"><li><a href="/a/">A</a> - Text.</li></ul>))

    expect(html.at_css('.band__body > nav.doors.doors--row')['aria-label']).to eq('More')
  end

  it 'keeps an empty page' do
    expect(filter.demo_sections(nil)).to eq('')
  end

  describe 'the shared sections' do
    it 'build a wide section with a label' do
      html = page('<h2 class="band--wide" id="run" data-label="For developers">Run it</h2><p>a</p>')

      expect(html.at_css('section')['class']).to eq('band band--wide')
      expect(html.at_css('section')['id']).to eq('run')
      expect(html.at_css('.band__label .label').text).to eq('For developers')
      expect(html.at_css('.band__body.prose > h2').text).to eq('Run it')
    end

    it 'stop on a look nobody knows, and name it' do
      expect { page('<h2 class="band--tnt">A</h2>') }.to raise_error(ArgumentError, /band--tnt, on: A/)
    end
  end

  describe 'the hero' do
    let(:marked) do
      '<h1 class="hero">Pick</h1><p>words</p><nav class="box-cards"><a href="/">S</a></nav>' \
        '<p class="actions"><a class="button" href="/">Go</a></p>' \
        '<figure class="pack-view"></figure><dialog class="lightbox"></dialog>'
    end
    let(:sass) do
      root = File.expand_path('../../../../shared/packages/design-system', __dir__)
      parts = %w[layout/split layout/hero components/box-cards components/pack-view components/lightbox]
      parts.map { |part| File.read(File.join(root, part, '_index.scss')) }.join +
        File.read(File.join(root, 'utilities/_typography.scss'))
    end

    it 'builds the copy beside the picture' do
      html = page(marked)

      expect(html.at_css('section')['class']).to eq('band band--split')
      expect(html.at_css('.container')['class']).to eq('container band__grid')
      expect(html.at_css('.container > div')['class']).to eq('band__body split hero split--text-left split--centred')
      expect(html.css('.band__body > *').map { |block| block['class'] }).to eq(['hero__copy', 'pack-view', 'lightbox'])
      expect(html.css('.hero__copy > *').map { |block| block['class'] })
        .to eq(['split__title display-1', 'prose prose--marketing', 'box-cards', 'actions'])
    end

    it 'takes one picture' do
      expect { page('<h1 class="hero">A</h1><p>a</p>') }.to raise_error(ArgumentError, /one picture, on: A/)
      expect { page("#{marked}<figure></figure>") }.to raise_error(ArgumentError, /one picture, on: Pick/)
    end

    it 'takes no other look and no label' do
      expect { page('<h1 class="hero band--tint">A</h1><figure></figure>') }
        .to raise_error(ArgumentError, /no other look and no label, on: A/)
      expect { page('<h1 class="hero" data-label="L">A</h1><figure></figure>') }
        .to raise_error(ArgumentError, /no other look and no label, on: A/)
    end

    it 'stops on a mark that is a typo, and names it' do
      expect { page(marked.sub('class="hero"', 'class="heroo"')) }.to raise_error(ArgumentError, /heroo, on: Pick/)
    end

    it 'writes only classes the design system has' do
      converter = described_class::Converter
      written = [*converter::HERO_BODY, *converter::HERO_TITLE, 'hero__copy'] - %w[hero band__body]

      expect(written.reject { |name| sass.match?(/\.#{name}(?![\w-])/) }).to be_empty
    end
  end
end
