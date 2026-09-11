# frozen_string_literal: true

require 'spec_helper'

V1_GUIDE = 'v1.x/guide.md'
V2_GUIDE = 'v2.x/guide.md'
REDIRECT_PAGE = 'redirect.md'
V2_ENTRY = { 'id' => 'v2.x', 'url_segment' => '2.0.4', 'label' => 'v2.0.4', 'version_tag' => '2.0' }.freeze
V1_ENTRY = { 'id' => 'v1.x', 'url_segment' => '1.0.3', 'label' => 'v1.0.3', 'version_tag' => '1.0.3' }.freeze

RSpec.describe Binacle::DocsVersions::VersionGenerator do
  describe 'the version comes off the folder' do
    it 'stamps the folder name under _versions as the version' do
      expect(doc(build_site, V1_GUIDE).data['version']).to eq('v1.x')
    end

    it 'takes the first folder only, however deep the page sits' do
      expect(doc(build_site, 'v1.x/deep/nested.md').data['version']).to eq('v1.x')
    end

    it 'leaves a document outside _versions without one' do
      expect(doc(build_site, 'unversioned.md').data).not_to have_key('version')
    end
  end

  describe 'the versions list' do
    it 'stamps the tag listed beside the version id' do
      expect(doc(build_site, V1_GUIDE).data['version_tag']).to eq('1.0.3')
      expect(doc(build_site, V2_GUIDE).data['version_tag']).to eq('2.0')
    end

    it 'stamps the label listed beside the version id' do
      expect(doc(build_site, V1_GUIDE).data['version_label']).to eq('v1.0.3')
      expect(doc(build_site, V2_GUIDE).data['version_label']).to eq('v2.0.4')
    end

    it 'never overwrites a tag the page set itself' do
      expect(doc(build_site, 'v2.x/own.md').data['version_tag']).to eq('9.9')
    end

    it 'fails the build when a version folder has no entry in the list' do
      expect { build_with_list([V2_ENTRY.dup]) }
        .to raise_error(Binacle::DocsVersions::Error, /no entry for v1.x; it has v2.x/)
    end

    it 'fails the build when an entry lacks one of the keys' do
      expect { build_with_list([V2_ENTRY.dup, V1_ENTRY.except('label')]) }
        .to raise_error(Binacle::DocsVersions::Error, /entry v1.x has no label/)
    end

    it 'stamps the index url onto every entry' do
      site = build_site

      expect(site.data['versions']['list'].map { |entry| entry['url'] })
        .to eq(['/version/2.0.4/', '/version/1.0.3/'])
    end
  end

  it 'stamps the label onto the title suffix' do
    expect(doc(build_site, V1_GUIDE).data['title_suffix']).to eq('(v1.0.3)')
  end

  it 'stamps the current version too, which is titled like any other' do
    expect(doc(build_site, V2_GUIDE).data['title_suffix']).to eq('(v2.0.4)')
  end

  it 'makes a version that is not current unindexable' do
    expect(doc(build_site, V1_GUIDE).data['robots']).to eq('noindex, follow')
  end

  it 'leaves the current version indexable' do
    expect(doc(build_site, V2_GUIDE).data['robots']).to be_nil
  end

  it 'never overwrites a value the page set itself' do
    page = doc(build_site, 'v1.x/swagger.md')

    expect(page.data['robots']).to eq('noindex, nofollow')
    expect(page.data['title_suffix']).to eq('(by hand)')
  end

  it 'leaves a document with no version alone' do
    page = doc(build_site, 'unversioned.md')

    expect(page.data).not_to have_key('title_suffix')
    expect(page.data).not_to have_key('robots')
  end

  it 'moves which version is indexable when the one knob moves' do
    site = build_with_current('v1.x')

    expect(doc(site, V1_GUIDE).data['robots']).to be_nil
    expect(doc(site, V2_GUIDE).data['robots']).to eq('noindex, follow')
  end

  it 'stamps the suffix before a generator at :low reads it' do
    expect(doc(build_site, V1_GUIDE).data['suffix_seen_at_low']).to eq('(v1.0.3)')
  end

  describe 'the url' do
    it 'renders a page under /version/<url_segment>/ as a folder with an index' do
      site = build_site

      expect(doc(site, V1_GUIDE).url).to eq('/version/1.0.3/guide/')
      expect(doc(site, V1_GUIDE).destination(site.dest)).to end_with('/version/1.0.3/guide/index.html')
    end

    it 'renders the folder index as the folder' do
      expect(doc(build_site, 'v1.x/index.md').url).to eq('/version/1.0.3/')
    end

    it 'keeps the path below the folder, however deep' do
      expect(doc(build_site, 'v1.x/deep/nested.md').url).to eq('/version/1.0.3/deep/nested/')
    end

    it 'gives a static file the same prefix and leaves its name alone' do
      site = build_site
      file = site.static_files.find { |candidate| candidate.relative_path.end_with?('swagger/v3.json') }

      expect(file.url).to eq('/version/1.0.3/swagger/v3.json')
      expect(File).to exist(File.join(site.dest, 'version/1.0.3/swagger/v3.json'))
    end

    it 'follows the url segment when it moves' do
      site = build_with_list([V2_ENTRY.dup, V1_ENTRY.merge('url_segment' => '1.0.9')])

      expect(doc(site, V1_GUIDE).url).to eq('/version/1.0.9/guide/')
      expect(site.data['versions']['list'].last['url']).to eq('/version/1.0.9/')
    end

    it 'overrides a permalink the page wrote itself' do
      expect(doc(build_site, 'v2.x/hand.md').url).to eq('/version/2.0.4/hand/')
    end

    it 'leaves a document outside _versions alone' do
      expect(doc(build_site, 'unversioned.md').url).to eq('/unversioned.html')
    end
  end

  describe 'the selector data' do
    it 'stamps where the same page is in every version' do
      site = build_site

      expect(doc(site, V1_GUIDE).data['version_urls'])
        .to eq('v1.x' => doc(site, V1_GUIDE).url, 'v2.x' => doc(site, V2_GUIDE).url)
    end

    it 'points at the index of a version that does not have the page' do
      site = build_site

      expect(doc(site, 'v1.x/deep/nested.md').data['version_urls']['v2.x'])
        .to eq(doc(site, 'v2.x/index.md').url)
    end

    it 'never overwrites a value the page set itself' do
      expect(doc(build_site, 'v2.x/own.md').data['version_urls']).to eq('v1.x' => '/pinned/')
    end
  end

  describe 'the collision check' do
    it 'fails the build when a page outside the versions claims a versioned url' do
      expect { build_site({}, COLLISION_SITE) }
        .to raise_error(Binacle::DocsVersions::Error,
                        %r{_versions/v1.x/guide.md and clash.md both render at /version/1.0.3/guide/})
    end

    it 'passes a site where every url is claimed once' do
      expect { build_site }.not_to raise_error
    end
  end

  describe 'the removed-page list' do
    let(:generator) { described_class.new }

    def pages_of(site)
      Binacle::DocsVersions::Pages.new(site.documents.select { |doc| doc.data['version'] })
    end

    it 'names every page and file in the previous version that the current one lacks' do
      site = build_site

      expect(generator.removed_pages(pages_of(site), 'v2.x', 'v1.x'))
        .to eq(['deep/nested.md', 'swagger.md', 'swagger/v3.json'])
    end

    it 'is empty when there is no previous version' do
      expect(generator.removed_pages(pages_of(build_site), 'v1.x', nil)).to eq([])
    end

    it 'prints the list at build, one page per line' do
      allow(Jekyll.logger).to receive(:info)

      build_site

      expect(Jekyll.logger).to have_received(:info)
        .with('Docs versions:', '3 pages in v1.x have no counterpart in v2.x')
      expect(Jekyll.logger).to have_received(:info).with('', '  v1.x/deep/nested.md')
      expect(Jekyll.logger).to have_received(:info).with('', '  v1.x/swagger.md')
      expect(Jekyll.logger).to have_received(:info).with('', '  v1.x/swagger/v3.json')
    end

    it 'prints nothing when the current version is the oldest' do
      allow(Jekyll.logger).to receive(:info)

      build_with_current('v1.x')

      expect(Jekyll.logger).not_to have_received(:info).with('Docs versions:', /counterpart in v1.x/)
    end
  end

  describe 'the redirect stamps' do
    it 'points a redirect page at the current version index' do
      site = build_site

      expect(doc(site, REDIRECT_PAGE).data['redirect_to']).to eq(doc(site, 'v2.x/index.md').url)
    end

    it 'gives it a canonical that is the page it points at, not itself' do
      page = doc(build_site, REDIRECT_PAGE)

      expect(page.data['canonical']).to eq(page.data['redirect_to'])
    end

    it 'makes it unindexable' do
      expect(doc(build_site, REDIRECT_PAGE).data['robots']).to eq('noindex')
    end

    it 'moves the redirect when the one knob moves' do
      site = build_with_current('v1.x')

      expect(doc(site, REDIRECT_PAGE).data['redirect_to']).to eq(doc(site, 'v1.x/index.md').url)
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
        .to raise_error(Binacle::DocsVersions::Error, %r{_versions/v1.x/index has no document})
    end
  end

  it 'fails the build when the site has no versions data' do
    expect { build_site('data_dir' => '_nothing') }
      .to raise_error(Binacle::DocsVersions::Error, /current is not set/)
  end

  it 'fails the build when current names a version the site does not have' do
    expect { build_with_current('v9.x') }
      .to raise_error(Binacle::DocsVersions::Error, /"v9.x", which is not one of v1.x, v2.x/)
  end
end
