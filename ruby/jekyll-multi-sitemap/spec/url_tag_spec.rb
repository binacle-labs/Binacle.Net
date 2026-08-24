# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::MultiSitemap::UrlTag do
  def render(markup, site, assigns = {})
    payload = { 'site' => Jekyll::Drops::SiteDrop.new(site) }.merge(assigns)
    Liquid::Template.parse(markup).render!(payload, registers: { site: site })
  end

  it 'writes one url element for the document it is given' do
    entry = built(build_site, 'by-hand-url.xml')

    expect(entry).to include('<loc>https://example.com/common_pages/legal.html</loc>')
    expect(entry).to include('<changefreq>yearly</changefreq>')
    expect(entry).to include('<priority>0.3</priority>')
  end

  it 'writes no urlset, xmlns or declaration of its own' do
    site = build_site
    page = site.pages.find { |candidate| candidate.url == '/about.html' }
    output = render('{% sitemap_url page %}', site, 'page' => page)

    expect(output).not_to include('urlset')
    expect(output).not_to include('<?xml')
    expect(output).to include('<loc>https://example.com/about.html</loc>')
  end

  it 'renders the document it is given, even one every sitemap excludes' do
    site = build_site
    page = site.pages.find { |candidate| candidate.url == '/hidden.html' }
    output = render('{% sitemap_url page %}', site, 'page' => page)

    expect(output).to include('<loc>https://example.com/hidden.html</loc>')
  end

  it 'raises when the markup is an expression rather than a variable name' do
    site = build_site

    expect { render("{% sitemap_url site.pages | first %}", site) }
      .to raise_error(Jekyll::MultiSitemap::Error, /variable name/)
  end

  it 'raises when the variable resolves to nothing' do
    site = build_site

    expect { render('{% sitemap_url missing %}', site) }
      .to raise_error(Jekyll::MultiSitemap::Error, /found nothing/)
  end
end
