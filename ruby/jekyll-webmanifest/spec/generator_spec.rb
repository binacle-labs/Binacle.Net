# frozen_string_literal: true

require 'spec_helper'

DEFAULT_MANIFEST = 'site.webmanifest'
CONFIGURED_PATH = '/app/manifest.json'
ICON_TYPE = 'image/png'

RSpec.describe Jekyll::Webmanifest::WebmanifestGenerator do
  it 'writes no file at all when the site has no webmanifest block' do
    site = build_site

    expect(written?(site, DEFAULT_MANIFEST)).to be false
  end

  it 'writes the file at the default path for a block with nothing in it' do
    site = build_site('webmanifest' => {})

    expect(written?(site, DEFAULT_MANIFEST)).to be true
  end

  it 'moves the file to a configured path' do
    site = build_site('webmanifest' => { 'path' => CONFIGURED_PATH })

    expect(written?(site, 'app/manifest.json')).to be true
    expect(written?(site, DEFAULT_MANIFEST)).to be false
  end

  it 'raises when the path names a directory' do
    expect { build_site('webmanifest' => { 'path' => '/app/' }) }
      .to raise_error(Jekyll::Webmanifest::Error, /name a file/)
  end

  it 'raises when the site already writes something to that path' do
    expect { build_site('webmanifest' => { 'path' => '/taken.webmanifest' }) }
      .to raise_error(Jekyll::Webmanifest::Error, /already written/)
  end

  it 'falls back to the site title and description' do
    site = build_site('webmanifest' => {})

    expect(manifest(site)['name']).to eq('Example')
    expect(manifest(site)['description']).to eq('An example site, kept here so the specs have something to read.')
  end

  it 'takes a name and description from the block over the site' do
    site = build_site('webmanifest' => { 'name' => 'Other', 'description' => 'Something else.' })

    expect(manifest(site)['name']).to eq('Other')
    expect(manifest(site)['description']).to eq('Something else.')
  end

  it 'defaults start_url to the site root and display to standalone' do
    site = build_site('webmanifest' => {})

    expect(manifest(site)['start_url']).to eq('/')
    expect(manifest(site)['display']).to eq('standalone')
  end

  it 'writes the colours it is given and no key for the ones it is not' do
    site = build_site('webmanifest' => { 'theme_color' => '#101010' })

    expect(manifest(site)['theme_color']).to eq('#101010')
    expect(manifest(site)).not_to have_key('background_color')
  end

  it 'writes one default icon when none is configured' do
    site = build_site('webmanifest' => {})

    expect(manifest(site)['icons'])
      .to eq([{ 'src' => '/android-chrome-192x192.png', 'sizes' => '192x192', 'type' => ICON_TYPE }])
  end

  it 'writes every configured icon, keys in the order the config declares them' do
    icons = [
      { 'src' => '/icon-192.png', 'sizes' => '192x192', 'type' => ICON_TYPE },
      { 'src' => '/icon-512.png', 'sizes' => '512x512', 'type' => ICON_TYPE, 'purpose' => 'maskable' }
    ]
    site = build_site('webmanifest' => { 'icons' => icons })

    expect(manifest(site)['icons']).to eq(icons)
    expect(built(site, DEFAULT_MANIFEST)).to include(%("src": "/icon-512.png",\n      "sizes"))
  end

  it 'writes no icons key for an empty list' do
    site = build_site('webmanifest' => { 'icons' => [] })

    expect(manifest(site)).not_to have_key('icons')
  end

  it 'raises when icons is not a list of maps' do
    expect { build_site('webmanifest' => { 'icons' => '/icon-192.png' }) }
      .to raise_error(Jekyll::Webmanifest::Error, /list of maps/)
  end

  it 'passes a key it has never heard of straight through' do
    extras = {
      'orientation' => 'portrait',
      'categories' => %w[utilities productivity],
      'shortcuts' => [{ 'name' => 'Second page', 'url' => '/second/' }]
    }
    site = build_site('webmanifest' => extras)
    written = manifest(site)

    expect(written['orientation']).to eq('portrait')
    expect(written['categories']).to eq(%w[utilities productivity])
    expect(written['shortcuts']).to eq([{ 'name' => 'Second page', 'url' => '/second/' }])
  end

  it 'writes the keys it knows first, then the ones it does not' do
    site = build_site('webmanifest' => { 'orientation' => 'portrait' })

    expect(manifest(site).keys)
      .to eq(%w[name description start_url display icons orientation])
  end

  it 'writes JSON that parses' do
    site = build_site('webmanifest' => { 'name' => 'A "quoted" name', 'orientation' => 'portrait' })

    expect { JSON.parse(built(site, DEFAULT_MANIFEST)) }.not_to raise_error
    expect(manifest(site)['name']).to eq('A "quoted" name')
  end

  it 'publishes the built path as site.webmanifest.url' do
    site = build_site('webmanifest' => { 'path' => CONFIGURED_PATH })

    expect(site.config['webmanifest']['url']).to eq(CONFIGURED_PATH)
  end

  it 'carries the baseurl into the published url, start_url and every icon' do
    site = build_site('baseurl' => '/base', 'webmanifest' => {})

    expect(site.config['webmanifest']['url']).to eq('/base/site.webmanifest')
    expect(manifest(site)['start_url']).to eq('/base/')
    expect(manifest(site)['icons'].first['src']).to eq('/base/android-chrome-192x192.png')
  end

  it 'leaves an icon src that already has a scheme alone' do
    icons = [{ 'src' => 'https://cdn.example.com/i.png' }]
    site = build_site('baseurl' => '/base', 'webmanifest' => { 'icons' => icons })

    expect(manifest(site)['icons'].first['src']).to eq('https://cdn.example.com/i.png')
  end

  it 'keeps the published url out of the manifest on a rebuild' do
    site = build_site('webmanifest' => {})
    site.process

    expect(manifest(site)).not_to have_key('url')
  end
end
