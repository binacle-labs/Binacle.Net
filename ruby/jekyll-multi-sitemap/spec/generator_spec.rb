# frozen_string_literal: true

require 'spec_helper'
require 'rexml/document'

ROOT_SITEMAP = 'sitemap.xml'
SITEMAP_DIR = '/sitemap'
INDEX_PATH = '/sitemap.xml'
HOME_URL = 'https://example.com/'

RSpec.describe Jekyll::MultiSitemap::SitemapGenerator do
  it 'writes nothing when the site has no sitemaps block' do
    site = build_site
    expect(File.exist?(File.join(site.dest, ROOT_SITEMAP))).to be false
  end

  it 'writes nothing for a block that only sets a mode, which is a site that only uses the tags' do
    site = build_site('sitemaps' => { 'mode' => 'opt-out' })

    expect(Dir.glob(File.join(site.dest, '**/*.xml')).map { |path| File.basename(path) })
      .to eq(%w[by-hand-url.xml by-hand-urlset.xml])
  end

  it 'raises when files is set to something that is not a list' do
    expect { build_site('sitemaps' => { 'files' => 'pages' }) }
      .to raise_error(Jekyll::MultiSitemap::Error, /non-empty list/)
  end

  it 'writes one file per entry, at the configured path' do
    site = build_site(
      'sitemaps' => {
        'path' => SITEMAP_DIR,
        'files' => [
          { 'name' => 'pages', 'include' => %w[pages common_pages] },
          { 'name' => 'versions', 'include' => ['versions'] }
        ]
      }
    )

    expect(built(site, 'sitemap/pages.xml')).to include('<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">')
    expect(built(site, 'sitemap/versions.xml')).to include('</urlset>')
  end

  it 'concatenates two include names into one file' do
    site = build_site(
      'sitemaps' => { 'path' => '/', 'files' => [{ 'name' => 'sitemap', 'include' => %w[pages common_pages] }] }
    )

    expect(locations(built(site, ROOT_SITEMAP))).to include(
      HOME_URL, 'https://example.com/common_pages/legal.html'
    )
  end

  it 'raises when an include names no collection' do
    expect do
      build_site('sitemaps' => { 'files' => [{ 'name' => 'sitemap', 'include' => ['nothing'] }] })
    end.to raise_error(Jekyll::MultiSitemap::Error, /no collection/)
  end

  it 'lists a page only when sitemap.exclude is false, by default' do
    site = build_site(one_file)
    urls = locations(built(site, ROOT_SITEMAP))

    expect(urls).to include(HOME_URL, 'https://example.com/about.html')
    expect(urls).not_to include('https://example.com/loose.html', 'https://example.com/hidden.html')
  end

  it 'lists everything except sitemap.exclude true in opt-out mode' do
    config = one_file
    config['sitemaps']['mode'] = 'opt-out'
    urls = locations(built(build_site(config), ROOT_SITEMAP))

    expect(urls).to include('https://example.com/loose.html')
    expect(urls).not_to include('https://example.com/hidden.html')
  end

  it 'raises on an unknown mode' do
    config = one_file
    config['sitemaps']['mode'] = 'sometimes'
    expect { build_site(config) }.to raise_error(Jekyll::MultiSitemap::Error, /mode/)
  end

  it 'writes no element for an unset lastmod, changefreq or priority' do
    entry = built(build_site(one_file), ROOT_SITEMAP)[%r{<url>\s*<loc>https://example.com/bare.html</loc>.*?</url>}m]

    expect(entry).not_to include('<lastmod>')
    expect(entry).not_to include('<changefreq>')
    expect(entry).not_to include('<priority>')
  end

  it 'writes every element a page does set' do
    xml = built(build_site(one_file), ROOT_SITEMAP)
    entry = xml[%r{<url>\s*<loc>https://example.com/about.html</loc>.*?</url>}m]

    expect(entry).to include('<lastmod>2024-03-04T00:00:00')
    expect(entry).to include('<changefreq>yearly</changefreq>')
    expect(entry).to include('<priority>0.5</priority>')
  end

  it 'reads a lastmod of "current" as the site build time' do
    site = build_site(one_file)
    entry = built(site, ROOT_SITEMAP)[%r{<url>\s*<loc>https://example.com/</loc>.*?</url>}m]

    expect(entry).to include("<lastmod>#{site.time.xmlschema}</lastmod>")
  end

  it 'drops index.html from a location and makes it absolute' do
    expect(locations(built(build_site(one_file), ROOT_SITEMAP))).to include(HOME_URL)
  end

  it 'matches a where value against a data path' do
    site = build_site(
      'sitemaps' => {
        'path' => '/',
        'files' => [
          { 'name' => 'sitemap', 'include' => ['versions'],
            'where' => { 'version' => '$site.data.versions.current' } }
        ]
      }
    )

    expect(locations(built(site, ROOT_SITEMAP))).to eq(['https://example.com/versions/v2-guide.html'])
  end

  it 'matches a where value literally when it has no $' do
    site = build_site(
      'sitemaps' => {
        'path' => '/',
        'files' => [{ 'name' => 'sitemap', 'include' => ['versions'], 'where' => { 'version' => '1.0' } }]
      }
    )

    expect(locations(built(site, ROOT_SITEMAP))).to eq(['https://example.com/versions/v1-guide.html'])
  end

  it 'raises when a $ path is not in site data' do
    expect do
      build_site(
        'sitemaps' => {
          'path' => '/',
          'files' => [{ 'name' => 'sitemap', 'include' => ['versions'],
                        'where' => { 'version' => '$site.data.versions.latest' } }]
        }
      )
    end.to raise_error(Jekyll::MultiSitemap::Error, /site data/)
  end

  it 'writes no index unless index: is set' do
    site = build_site(
      'sitemaps' => { 'path' => SITEMAP_DIR, 'files' => [{ 'name' => 'pages', 'include' => ['pages'] }] }
    )

    expect(File.exist?(File.join(site.dest, ROOT_SITEMAP))).to be false
  end

  it 'writes an index naming every generated file' do
    site = build_site(
      'sitemaps' => {
        'path' => SITEMAP_DIR,
        'index' => INDEX_PATH,
        'files' => [
          { 'name' => 'pages', 'include' => ['pages'] },
          { 'name' => 'versions', 'include' => ['versions'] }
        ]
      }
    )
    index = built(site, ROOT_SITEMAP)

    expect(index).to include('<sitemapindex xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">')
    expect(locations(index)).to eq(
      ['https://example.com/sitemap/pages.xml', 'https://example.com/sitemap/versions.xml']
    )
  end

  it 'writes well formed XML' do
    site = build_site(
      'sitemaps' => {
        'path' => SITEMAP_DIR,
        'index' => INDEX_PATH,
        'files' => [
          { 'name' => 'pages', 'include' => %w[pages common_pages] },
          { 'name' => 'versions', 'include' => ['versions'] }
        ]
      }
    )

    Dir.glob(File.join(site.dest, '**/*.xml')).each do |path|
      expect { REXML::Document.new(File.read(path)) }.not_to raise_error
    end
  end

  it 'raises when the index path is also a generated file' do
    expect do
      build_site(
        'sitemaps' => {
          'path' => '/',
          'index' => INDEX_PATH,
          'files' => [{ 'name' => 'sitemap', 'include' => ['pages'] }]
        }
      )
    end.to raise_error(Jekyll::MultiSitemap::Error, /index path/)
  end
end
