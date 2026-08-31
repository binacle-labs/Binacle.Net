# frozen_string_literal: true

require 'spec_helper'

LINK_PAGE = 'link.html'

RSpec.describe Jekyll::Webmanifest::LinkTag do
  it 'renders the link element from the published url' do
    site = build_site('webmanifest' => {})

    expect(built(site, LINK_PAGE)).to include('<link rel="manifest" href="/site.webmanifest">')
  end

  it 'renders the configured path' do
    site = build_site('webmanifest' => { 'path' => '/app/manifest.json' })

    expect(built(site, LINK_PAGE)).to include('<link rel="manifest" href="/app/manifest.json">')
  end

  it 'carries the baseurl' do
    site = build_site('baseurl' => '/base', 'webmanifest' => {})

    expect(built(site, LINK_PAGE)).to include('<link rel="manifest" href="/base/site.webmanifest">')
  end

  it 'renders nothing when the site has no webmanifest block' do
    site = build_site

    expect(built(site, LINK_PAGE)).not_to include('<link')
  end
end
