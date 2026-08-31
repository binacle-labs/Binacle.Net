# frozen_string_literal: true

require 'spec_helper'

ROBOTS_FILE = 'robots.txt'

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

    expect(built(site, ROBOTS_FILE)).to include(
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
    lines = built(site, ROBOTS_FILE).scan(/^Sitemap: .*$/)

    expect(lines).to eq(['Sitemap: https://example.com/sitemap.xml'])
  end

  it 'writes nothing when the site has no sitemaps block' do
    expect(built(build_site, ROBOTS_FILE)).not_to include('Sitemap:')
  end

  it 'leaves the generated urls readable as site.sitemaps.urls' do
    site = build_site(one_file)

    expect(built(site, ROBOTS_FILE)).to include('# urls: https://example.com/sitemap.xml')
  end
end
