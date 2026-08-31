# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::BreadcrumbTrail::BreadcrumbsTag do
  it 'renders a navigation landmark, an ordered list and the current page marked' do
    expect(nav(build_site(versioned), DEEP_PAGE).split("\n")).to eq(
      [
        '<nav aria-label="breadcrumb">',
        '<ol class="breadcrumb">',
        '<li class="breadcrumb-item"><a href="/section/v1.0.x/">Home</a></li>',
        '<li class="breadcrumb-item"><a href="/section/v1.0.x/getting-started/">Getting Started</a></li>',
        '<li class="breadcrumb-item active" aria-current="page">Quick start</li>',
        '</ol>',
        '</nav>'
      ]
    )
  end

  it 'renders nothing at all for a page that turns breadcrumbs off' do
    expect(nav(build_site(versioned), 'off.html')).to eq('')
  end

  it 'renders nothing at all for a page with no trail' do
    expect(nav(build_site, 'index.html')).to eq('')
  end

  describe 'link_last' do
    it 'leaves the current page unlinked by default' do
      expect(nav(build_site(versioned), DEEP_PAGE)).to include(
        '<li class="breadcrumb-item active" aria-current="page">Quick start</li>'
      )
    end

    it 'links the current page when it is on, keeping aria-current' do
      site = build_site(versioned('link_last' => true))

      expect(nav(site, DEEP_PAGE)).to include(
        '<li class="breadcrumb-item active">' \
        '<a href="/section/v1.0.x/getting-started/quick-start.html" aria-current="page">Quick start</a></li>'
      )
    end
  end

  describe 'the home crumb' do
    it 'renders its title as text by default' do
      expect(nav(build_site(versioned), DEEP_PAGE)).to include('<a href="/section/v1.0.x/">Home</a>')
    end

    it 'renders the configured html in place of the title' do
      site = build_site(versioned('home' => { 'html' => '<i class="small">home</i>' }))

      expect(nav(site, DEEP_PAGE)).to include(
        '<a href="/section/v1.0.x/"><i class="small">home</i></a>'
      )
    end

    it 'takes a configured title' do
      site = build_site(versioned('home' => { 'title' => 'Start' }))

      expect(nav(site, DEEP_PAGE)).to include('>Start</a>')
    end
  end

  describe 'the nav element' do
    it 'carries the configured aria-label' do
      site = build_site(versioned('label' => 'fil d\'Ariane'))

      expect(nav(site, DEEP_PAGE)).to include('<nav aria-label="fil d&#39;Ariane">')
    end

    it 'carries a configured class and no class attribute without one' do
      expect(nav(build_site(versioned('class' => 'tiny-space')), DEEP_PAGE)).to include(
        '<nav aria-label="breadcrumb" class="tiny-space">'
      )
    end
  end

  it 'cannot have its markup broken by a label holding an ampersand, a quote or a tag' do
    expect(nav(build_site(versioned), 'entities.html')).to include(
      '<li class="breadcrumb-item active" aria-current="page">Tom &amp; &quot;Jerry&quot; &lt;b&gt;</li>'
    )
  end

  it 'prefixes every href with the baseurl, which the published trail leaves off' do
    site = build_site(versioned, 'baseurl' => '/docs')

    expect(trail(site, DEEP_PAGE).first['url']).to eq('/section/v1.0.x/')
    expect(nav(site, DEEP_PAGE)).to include('<a href="/docs/section/v1.0.x/">Home</a>')
  end
end
