# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::ResourceTags::ScriptTags do
  it 'writes one script element per item, with a closing tag' do
    expect(elements('scripts.html')).to eq(
      ['<script type="text/javascript" src="/app/js/main.js" defer></script>']
    )
  end

  it 'keeps the script-only attributes that prefetch_tags drops' do
    expect(elements('script-only.html')).to eq(
      ['<script src="/app/js/script-only.js" type="text/javascript" crossorigin="anonymous" ' \
       'defer async nomodule></script>']
    )
  end

  it 'reads a list held in the page front matter' do
    expect(elements('front-matter.html')).to eq(
      ['<script src="/app/js/from-front-matter.js" type="module"></script>']
    )
  end

  it 'renders nothing when the page does not set the list' do
    expect(built('missing.html')).to eq('AB')
  end
end
