# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::MultiSitemap::LinksTag do
  it 'writes one line per generated file when there is no index' do
    site = build_site(
      'sitemaps' => {
        'path' => '/sitemap',
        'files' => [
          { 'name' => 'pages', 'include' => ['pages'] },
          { 'name' => 'versions', 'include' => ['versions'] }
        ]
      }
    )

    expect(built(site, 'robots.txt')).to include(
      "Sitemap: https://example.com/sitemap/pages.xml\nSitemap: https://example.com/sitemap/versions.xml"
    )
  end

  it 'writes the index alone when there is one' do
    site = build_site(
      'sitemaps' => {
        'path' => '/sitemap',
        'index' => '/sitemap.xml',
        'files' => [
          { 'name' => 'pages', 'include' => ['pages'] },
          { 'name' => 'versions', 'include' => ['versions'] }
        ]
      }
    )
    lines = built(site, 'robots.txt').scan(/^Sitemap: .*$/)

    expect(lines).to eq(['Sitemap: https://example.com/sitemap.xml'])
  end

  it 'writes nothing when the site has no sitemaps block' do
    expect(built(build_site, 'robots.txt')).not_to include('Sitemap:')
  end

  it 'leaves the generated urls readable as site.sitemaps.urls' do
    site = build_site(one_file)

    expect(built(site, 'robots.txt')).to include('# urls: https://example.com/sitemap.xml')
  end
end
