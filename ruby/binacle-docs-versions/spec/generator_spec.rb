# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::DocsVersions::VersionGenerator do
  it 'stamps the version onto the title suffix' do
    expect(doc(build_site, 'v1.0.x/guide.md').data['title_suffix']).to eq('(v1.0.x)')
  end

  it 'stamps the current version too, which is titled like any other' do
    expect(doc(build_site, 'v2.0.x/guide.md').data['title_suffix']).to eq('(v2.0.x)')
  end

  it 'makes a version that is not current unindexable' do
    expect(doc(build_site, 'v1.0.x/guide.md').data['robots']).to eq('noindex, follow')
  end

  it 'leaves the current version indexable' do
    expect(doc(build_site, 'v2.0.x/guide.md').data['robots']).to be_nil
  end

  it 'never overwrites a value the page set itself' do
    page = doc(build_site, 'v1.0.x/swagger.md')

    expect(page.data['robots']).to eq('noindex, nofollow')
    expect(page.data['title_suffix']).to eq('(by hand)')
  end

  it 'leaves a document with no version alone' do
    page = doc(build_site, 'unversioned.md')

    expect(page.data).not_to have_key('title_suffix')
    expect(page.data).not_to have_key('robots')
  end

  it 'moves which version is indexable when the one knob moves' do
    site = build_with_current('v1.0.x')

    expect(doc(site, 'v1.0.x/guide.md').data['robots']).to be_nil
    expect(doc(site, 'v2.0.x/guide.md').data['robots']).to eq('noindex, follow')
  end

  it 'stamps the suffix before a generator at :low reads it' do
    expect(doc(build_site, 'v1.0.x/guide.md').data['suffix_seen_at_low']).to eq('(v1.0.x)')
  end

  it 'fails the build when the site has no versions data' do
    expect { build_site('data_dir' => '_nothing') }
      .to raise_error(Binacle::DocsVersions::Error, /current is not set/)
  end

  it 'fails the build when current names a version the site does not have' do
    expect { build_with_current('v9.9.x') }
      .to raise_error(Binacle::DocsVersions::Error, /"v9.9.x", which is not one of v1.0.x, v2.0.x/)
  end
end
