# frozen_string_literal: true

require 'spec_helper'

V1_GUIDE = 'v1.0.x/guide.md'
V2_GUIDE = 'v2.0.x/guide.md'
REDIRECT_PAGE = 'redirect.md'

RSpec.describe Binacle::DocsVersions::VersionGenerator do
  it 'stamps the version onto the title suffix' do
    expect(doc(build_site, V1_GUIDE).data['title_suffix']).to eq('(v1.0.x)')
  end

  it 'stamps the current version too, which is titled like any other' do
    expect(doc(build_site, V2_GUIDE).data['title_suffix']).to eq('(v2.0.x)')
  end

  it 'makes a version that is not current unindexable' do
    expect(doc(build_site, V1_GUIDE).data['robots']).to eq('noindex, follow')
  end

  it 'leaves the current version indexable' do
    expect(doc(build_site, V2_GUIDE).data['robots']).to be_nil
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

    expect(doc(site, V1_GUIDE).data['robots']).to be_nil
    expect(doc(site, V2_GUIDE).data['robots']).to eq('noindex, follow')
  end

  it 'stamps the suffix before a generator at :low reads it' do
    expect(doc(build_site, V1_GUIDE).data['suffix_seen_at_low']).to eq('(v1.0.x)')
  end

  describe 'the redirect stamps' do
    it 'points a redirect page at the current version index' do
      site = build_site

      expect(doc(site, REDIRECT_PAGE).data['redirect_to']).to eq(doc(site, 'v2.0.x/index.md').url)
    end

    it 'gives it a canonical that is the page it points at, not itself' do
      page = doc(build_site, REDIRECT_PAGE)

      expect(page.data['canonical']).to eq(page.data['redirect_to'])
    end

    it 'makes it unindexable' do
      expect(doc(build_site, REDIRECT_PAGE).data['robots']).to eq('noindex')
    end

    it 'moves the redirect when the one knob moves' do
      site = build_with_current('v1.0.x')

      expect(doc(site, REDIRECT_PAGE).data['redirect_to']).to eq(doc(site, 'v1.0.x/index.md').url)
    end

    it 'never overwrites a value the page set itself' do
      page = doc(build_site, 'redirect-own.md')

      expect(page.data['redirect_to']).to eq('/pinned/')
      expect(page.data['canonical']).to eq('/elsewhere/')
      expect(page.data['robots']).to eq('none')
    end

    it 'leaves a page on another layout alone' do
      expect(doc(build_site, 'unversioned.md').data).not_to have_key('redirect_to')
    end

    it 'fails the build when the current version has no index to point at' do
      expect { build_site({}, NO_INDEX_SITE) }
        .to raise_error(Binacle::DocsVersions::Error, %r{_versions/v1.0.x/index has no document})
    end
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
