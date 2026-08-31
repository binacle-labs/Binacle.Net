# frozen_string_literal: true

require 'spec_helper'

VERSION_ROOT = '/section/v1.0.x/'

RSpec.describe Jekyll::BreadcrumbTrail::TrailGenerator do
  it 'publishes the trail as name and url pairs, the first crumb first' do
    expect(trail(build_site(versioned), DEEP_PAGE)).to eq(
      [
        { 'name' => 'Home', 'url' => VERSION_ROOT },
        { 'name' => 'Getting Started', 'url' => '/section/v1.0.x/getting-started/' },
        { 'name' => 'Quick start', 'url' => '/section/v1.0.x/getting-started/quick-start.html' }
      ]
    )
  end

  describe 'the home crumb' do
    it 'is the deepest excluded segment above the current page' do
      expect(trail(build_site(versioned), DEEP_PAGE).first['url']).to eq(VERSION_ROOT)
    end

    it 'stops at the excluded segment above a page that is itself excluded' do
      expect(trail(build_site(versioned), 'version.html').first['url']).to eq('/section/')
    end

    it 'is the site root when no segment above the page is excluded' do
      expect(trail(build_site(versioned), 'flat.html').first['url']).to eq('/')
    end

    it 'is the site root when nothing is excluded' do
      expect(trail(build_site, DEEP_PAGE).first['url']).to eq('/')
    end
  end

  it 'never excludes the current page, whatever it matches' do
    expect(trail(build_site(versioned), 'version.html').map { |crumb| crumb['name'] }).to eq(
      ['Home', 'The one point oh line']
    )
  end

  it 'keeps an excluded segment in every crumb url below it' do
    urls = trail(build_site(versioned), DEEP_PAGE).map { |crumb| crumb['url'] }

    expect(urls).to all(start_with(VERSION_ROOT))
  end

  it 'gives a crumb for a file no trailing slash' do
    expect(trail(build_site(versioned), DEEP_PAGE).last['url']).to eq(
      '/section/v1.0.x/getting-started/quick-start.html'
    )
  end

  it 'gives a crumb for a directory a trailing slash' do
    expect(trail(build_site(versioned), 'folder.html').last['url']).to eq('/section/v1.0.x/getting-started/')
  end

  it 'writes an empty trail for a page that turns breadcrumbs off' do
    expect(trail(build_site(versioned), 'off.html')).to eq([])
  end

  it 'writes an empty trail for a page with no segments' do
    expect(trail(build_site, 'index.html')).to eq([])
  end

  describe 'the current page label' do
    it 'takes the first key in title_from that the page sets' do
      expect(trail(build_site(versioned), 'crumbtitle.html').last['name']).to eq('Custom crumb')
    end

    it 'follows a title_from a site configures for its own vocabulary' do
      site = build_site(versioned('title_from' => %w[menu_title title]))

      expect(trail(site, 'crumbtitle.html').last['name']).to eq('Menu crumb')
    end

    it 'falls back to the segment when the page sets none of them' do
      expect(trail(build_site(versioned), 'untitled.html').last['name']).to eq('First Steps')
    end

    it 'sees a key another generator stamped, which is what the low priority is for' do
      expect(trail(build_site(versioned), 'stamped.html').last['name']).to eq('Stamped crumb')
    end
  end

  describe 'the labels of the crumbs above it' do
    it 'turns hyphens into spaces before capitalizing, not after' do
      expect(trail(build_site(versioned), 'untitled.html')[1]['name']).to eq('Api Reference')
    end

    it 'drops the file extension' do
      expect(Jekyll::BreadcrumbTrail::Labels.humanize('quick-start.html')).to eq('Quick Start')
    end
  end

  describe 'the config' do
    it 'rejects an exclude that is not a list' do
      expect { build_site('exclude' => 'section') }.to raise_error(Jekyll::BreadcrumbTrail::Error, /exclude/)
    end

    it 'rejects an empty title_from' do
      expect { build_site('title_from' => []) }.to raise_error(Jekyll::BreadcrumbTrail::Error, /title_from/)
    end
  end
end
