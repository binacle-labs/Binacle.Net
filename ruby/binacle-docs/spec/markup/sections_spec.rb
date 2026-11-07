# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Docs::Markup::Sections do
  let(:filter) { Class.new { include Binacle::Docs::Markup::Sections }.new }

  describe '#doc_sections' do
    it 'lists each h2 with its id and text, in order' do
      html = '<h2 id="one">One</h2><p>text</p><h2 id="two">Two</h2>'

      expect(filter.doc_sections(html)).to eq([{ 'id' => 'one', 'title' => 'One' }, { 'id' => 'two', 'title' => 'Two' }])
    end

    it 'finds the id after another attribute' do
      expect(filter.doc_sections('<h2 class="a" id="one">One</h2>').first['id']).to eq('one')
    end

    it 'drops the tags inside a heading' do
      expect(filter.doc_sections('<h2 id="one">The <code>pack</code> call</h2>').first['title']).to eq('The pack call')
    end

    it 'skips other levels and an h2 with no id' do
      expect(filter.doc_sections('<h3 id="a">A</h3><h2>B</h2>')).to eq([])
    end

    it 'gives an empty list for nil' do
      expect(filter.doc_sections(nil)).to eq([])
    end
  end
end
