# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::ResourceTags::LinkTags do
  it 'writes one link element per item' do
    expect(elements('links.html')).to eq(
      [
        '<link rel="stylesheet" type="text/css" href="/app/css/main.css">',
        '<link rel="icon" type="image/png" href="/app/icon.png">'
      ]
    )
  end

  it 'reads the path from href and puts it through relative_url' do
    expect(built('links.html')).to include('href="/app/css/main.css"')
  end
end
