# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::MultiSitemap::UrlsetTag do
  def render(markup, site, assigns = {})
    payload = { 'site' => Jekyll::Drops::SiteDrop.new(site) }.merge(assigns)
    Liquid::Template.parse(markup).render!(payload, registers: { site: site })
  end

  it 'writes a whole document: declaration, urlset and one url per entry' do
    output = built(build_site, 'by-hand-urlset.xml')

    expect(output).to start_with('<?xml version="1.0" encoding="utf-8"?>')
    expect(output).to include('<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">')
    expect(locations(output)).to eq(['https://example.com/versions/v2-guide.html'])
  end

  it 'applies the mode from the config block, not from the tag' do
    site = build_site('sitemaps' => { 'mode' => 'opt-out', 'files' => [{ 'name' => 's', 'include' => ['pages'] }] })
    output = render('{% sitemap_urlset site.pages %}', site)

    expect(locations(output)).to include('https://example.com/loose.html')
    expect(locations(output)).not_to include('https://example.com/hidden.html')
  end

  it 'lists only pages with sitemap.exclude false when the site does not set a mode' do
    site = build_site
    output = render('{% sitemap_urlset site.pages %}', site)

    expect(locations(output)).not_to include('https://example.com/loose.html')
  end

  it 'writes an empty urlset for an empty list' do
    site = build_site
    output = render('{% assign none = site.tags %}{% sitemap_urlset none %}', site)

    expect(locations(output)).to be_empty
    expect(output).to include('</urlset>')
  end

  it 'raises when the markup is an expression rather than a variable name' do
    site = build_site

    expect { render(%({% sitemap_urlset site.versions | where: "version", "2.0" %}), site) }
      .to raise_error(Jekyll::MultiSitemap::Error, /variable name/)
  end
end
