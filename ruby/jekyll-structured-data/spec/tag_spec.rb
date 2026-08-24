# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::StructuredData::StructuredDataTag do
  it 'writes one block holding the organisation, the page and the trail' do
    expect(graph(build_site(configured), 'index.html').map { |entry| entry['@type'] })
      .to eq(%w[Organization WebApplication BreadcrumbList])
  end

  it 'writes the schema.org context once, at the top' do
    parsed = JSON.parse(block(build_site(configured), 'index.html'))

    expect(parsed['@context']).to eq('https://schema.org')
  end

  it 'writes nothing at all for a page carrying none of the keys' do
    expect(output(build_site(configured), 'bare.html')).not_to include('<script')
  end

  it 'writes nothing when the only node left would be the organisation' do
    site = build_site(configured)

    expect(output(site, 'typed-only.html')).to include('WebApplication')
    expect(output(site, 'bare.html')).not_to include('<script')
  end

  describe 'the page node' do
    it 'reads its values from the page meta keys' do
      expect(node(build_site(configured), 'index.html', 'WebApplication')).to include(
        '@id' => 'https://example.com/#page',
        'name' => 'The Home Page',
        'description' => 'What the page is about.',
        'url' => 'https://example.com/',
        'image' => 'https://example.com/media/card.png'
      )
    end

    it 'links to the organisation and to the trail by @id' do
      expect(node(build_site(configured), 'index.html', 'WebApplication')).to include(
        'publisher' => { '@id' => 'https://www.example.com/#organization' },
        'breadcrumb' => { '@id' => 'https://example.com/#breadcrumbs' }
      )
    end

    it 'is absent for a page with no type and no default type' do
      expect(node(build_site(configured), 'no-type.html', 'WebApplication')).to be_nil
      expect(graph(build_site(configured), 'no-type.html').map do |entry|
        entry['@type']
      end).to eq(%w[Organization BreadcrumbList])
    end

    it 'appears for a typeless page once a default type is configured' do
      site = build_site(configured('default_type' => 'WebPage'))

      expect(node(site, 'no-type.html', 'WebPage')).to include('name' => 'No Type')
    end

    it 'takes the per-type defaults from config' do
      expect(node(build_site(configured), 'index.html', 'WebApplication')).to include(
        'applicationCategory' => 'DeveloperApplication', 'operatingSystem' => 'Any'
      )
    end

    it 'lets the page keys win over the defaults' do
      found = node(build_site(configured), 'extras.html', 'WebApplication')

      expect(found['applicationCategory']).to eq('BusinessApplication')
      expect(found['name']).to eq('Packing Demo')
    end

    it 'holds only what it was given when the page carries no meta' do
      found = node(build_site(configured), 'typed-only.html', 'WebApplication')

      expect(found.keys).to contain_exactly('@type', 'publisher', 'applicationCategory', 'operatingSystem')
    end
  end

  describe 'the organisation' do
    it 'keeps the same @id whatever the site url is' do
      one = node(build_site(configured.merge('url' => 'https://one.example.com')), 'index.html', 'Organization')
      two = node(build_site(configured.merge('url' => 'https://two.example.com')), 'index.html', 'Organization')

      expect(one['@id']).to eq('https://www.example.com/#organization')
      expect(two['@id']).to eq(one['@id'])
    end

    it 'writes same_as out as sameAs' do
      expect(node(build_site(configured), 'index.html', 'Organization'))
        .to include('sameAs' => ['https://github.com/example'])
    end

    it 'makes the logo absolute against the organisation url' do
      expect(node(build_site(configured), 'index.html', 'Organization'))
        .to include('logo' => 'https://www.example.com/media/logo.png')
    end

    it 'is absent when no organisation is configured' do
      site = build_site

      expect(node(site, 'index.html', 'Organization')).to be_nil
      expect(node(site, 'index.html', 'WebApplication')).not_to have_key('publisher')
    end

    it 'raises when it has no @id' do
      expect { build_site('structured_data' => { 'organization' => { 'name' => 'Example Ltd' } }) }
        .to raise_error(Jekyll::StructuredData::Error, /@id/)
    end
  end

  describe 'the breadcrumb list' do
    it 'numbers every crumb and writes its url in full' do
      expect(node(build_site(configured), 'index.html', 'BreadcrumbList')['itemListElement']).to eq(
        [
          { '@type' => 'ListItem', 'position' => 1, 'name' => 'Home', 'item' => 'https://example.com/' },
          { '@type' => 'ListItem', 'position' => 2, 'name' => 'Guides', 'item' => 'https://example.com/guides/' },
          { '@type' => 'ListItem', 'position' => 3, 'name' => 'Quick start',
            'item' => 'https://example.com/guides/quick-start.html' }
        ]
      )
    end

    it 'is absent on a noindex page, and nothing links to it' do
      expect(node(build_site(configured), 'noindex.html', 'BreadcrumbList')).to be_nil
      expect(node(build_site(configured), 'noindex.html', 'WebApplication')).not_to have_key('breadcrumb')
    end
  end

  describe 'the json' do
    it 'cannot be broken out of by a title' do
      expect(block(build_site(configured), 'nasty.html')).not_to include('</script>')
    end

    it 'parses whatever the title contains' do
      title = node(build_site(configured), 'nasty.html', 'WebApplication')['name']

      expect(title).to eq(%(A </script> title with "quotes", a newline\nand ελληνικά))
    end
  end
end
