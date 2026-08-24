# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::ResourceTags::PrefetchTags do
  it 'takes a script list and writes the path as href' do
    expect(elements('prefetch.html')).to eq(
      ['<link rel="prefetch" as="script" type="text/javascript" href="/app/js/main.js">']
    )
  end

  it 'drops the attributes that mean nothing on a link, and keeps the rest' do
    expect(elements('prefetch-script-only.html')).to eq(
      [
        '<link rel="prefetch" as="script" href="/app/js/script-only.js" type="text/javascript" ' \
        'crossorigin="anonymous">'
      ]
    )
  end

  it 'lets an item carry its own rel' do
    expect(elements('prefetch-own-rel.html')).to eq(
      ['<link rel="preload" as="script" href="/app/js/preloaded.js">']
    )
  end
end
