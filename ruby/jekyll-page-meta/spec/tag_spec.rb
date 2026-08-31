# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::PageMeta::MetaTag do
  it 'writes the whole head block from the resolved keys' do
    site = build_site('og_image' => '/media/card.png')

    expect(head(site, HOME_PAGE).split("\n")).to include(
      '<title>Home - Example</title>',
      '<meta name="description" content="The front page.">',
      '<link rel="canonical" href="https://example.com/">',
      '<meta property="og:type" content="website">',
      '<meta property="og:site_name" content="Example">',
      '<meta property="og:title" content="Home - Example">',
      '<meta property="og:description" content="The front page.">',
      '<meta property="og:url" content="https://example.com/">',
      '<meta property="og:image" content="https://example.com/media/card.png">',
      '<meta property="og:locale" content="en_US">',
      '<meta name="twitter:card" content="summary">',
      '<meta name="twitter:title" content="Home - Example">',
      '<meta name="twitter:description" content="The front page.">',
      '<meta name="twitter:url" content="https://example.com/">',
      '<meta name="twitter:image" content="https://example.com/media/card.png">'
    )
  end

  it 'writes no canonical and no og:url for a page with no url' do
    output = head(build_site, 'urlless.html')

    expect(output).to include('<title>Urlless - Example</title>')
    expect(output).not_to include('canonical')
    expect(output).not_to include('og:url')
    expect(output).not_to include('twitter:url')
  end

  it 'writes no image elements when nothing sets an image' do
    output = head(build_site, HOME_PAGE)

    expect(output).not_to include('og:image')
    expect(output).not_to include('twitter:image')
  end

  it 'writes robots only for a page that sets it' do
    site = build_site

    expect(head(site, 'robots.html')).to include('<meta name="robots" content="noindex, follow">')
    expect(head(site, HOME_PAGE)).not_to include('name="robots"')
  end

  it 'writes no twitter:site unless one is configured' do
    expect(head(build_site, HOME_PAGE)).not_to include('twitter:site')
  end

  it 'writes the configured twitter:site as a handle' do
    site = build_site('page_meta' => { 'twitter_site' => 'example' })

    expect(head(site, HOME_PAGE)).to include('<meta name="twitter:site" content="@example">')
  end

  it 'takes the card type from config' do
    site = build_site('page_meta' => { 'twitter_card' => 'summary_large_image' })

    expect(head(site, HOME_PAGE)).to include('<meta name="twitter:card" content="summary_large_image">')
  end

  it 'takes og:locale off the page before the site' do
    expect(head(build_site, 'locale.html')).to include('<meta property="og:locale" content="fr_FR">')
  end

  it 'writes no og:locale when neither the page nor the site sets one' do
    expect(head(build_site('locale' => ''), HOME_PAGE)).not_to include('og:locale')
  end

  it 'escapes a value once, whatever the source did to it' do
    output = head(build_site, 'entities.md')

    expect(output).to include('<title>Salt &amp; pepper - Example</title>')
    expect(output).to include('<meta name="description" content="Pass the salt &amp; pepper.">')
    expect(output).not_to include('&amp;amp;')
  end
end
