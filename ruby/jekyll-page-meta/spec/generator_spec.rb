# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::PageMeta::MetaGenerator do
  it 'writes the four keys onto every page' do
    site = build_site

    expect(meta(site, 'index.html')).to eq(
      'title' => 'Home - Example',
      'description' => 'The front page.',
      'canonical' => 'https://example.com/'
    )
  end

  it 'writes the image key when there is an image' do
    site = build_site('og_image' => '/media/card.png')

    expect(meta(site, 'index.html')['image']).to eq('https://example.com/media/card.png')
  end

  describe 'the title' do
    it 'joins the page title to the site title with the separator' do
      expect(meta(build_site, 'about.md')['title']).to eq('About - Example')
    end

    it 'takes the separator from config' do
      site = build_site('page_meta' => { 'title_separator' => ' | ' })

      expect(meta(site, 'about.md')['title']).to eq('About | Example')
    end

    it 'appends the suffix before the separator' do
      expect(meta(build_site, 'suffix.html')['title']).to eq('Quick start (v2.1.x) - Example')
    end

    it 'lets seo_title win over the title, the suffix and the separator' do
      expect(meta(build_site, 'seo-title.html')['title']).to eq('Exactly This')
    end

    it 'falls back to the site title alone' do
      expect(meta(build_site, 'bare.html')['title']).to eq('Example')
    end
  end

  describe 'the description' do
    it 'reads the description key first' do
      expect(meta(build_site, 'index.html')['description']).to eq('The front page.')
    end

    it 'falls through to the excerpt' do
      expect(meta(build_site, 'guide.md')['description'])
        .to eq('The first paragraph, which Jekyll makes the excerpt.')
    end

    it 'falls through to the site description' do
      expect(meta(build_site, 'about.md')['description']).to eq('The site description, in one plain sentence.')
    end

    it 'reads the page content when the chain names it' do
      site = build_site(from('description', 'content', 'site'))

      expect(meta(site, 'about.md')['description'])
        .to eq('A markdown paragraph with a tag in it. A second paragraph nobody reads.')
    end

    it 'skips a link that resolves to nothing but whitespace' do
      site = build_site(from('title_suffix', 'description'))

      expect(meta(site, 'index.html')['description']).to eq('The front page.')
    end

    it 'writes no key when every link is empty' do
      site = build_site(from('description'))

      expect(meta(site, 'about.md')).not_to have_key('description')
    end

    it 'cuts on characters, not on words' do
      site = build_site('page_meta' => { 'description' => { 'truncate' => 20 } })

      expect(meta(site, 'long.html')['description']).to eq('Bin packing keeps bo')
    end

    it 'cuts nothing when truncate is zero' do
      site = build_site('page_meta' => { 'description' => { 'truncate' => 0 } })

      expect(meta(site, 'long.html')['description'])
        .to eq('Bin packing keeps boxes off the floor and pallets on the truck.')
    end

    it 'cuts nothing when truncate is false' do
      site = build_site('page_meta' => { 'description' => { 'truncate' => false } })

      expect(meta(site, 'long.html')['description'].length).to eq(63)
    end

    it 'raises when truncate is not a whole number' do
      expect { build_site('page_meta' => { 'description' => { 'truncate' => 'lots' } }) }
        .to raise_error(Jekyll::PageMeta::Error, /whole number/)
    end

    it 'raises when the chain is not a list' do
      expect { build_site('page_meta' => { 'description' => { 'from' => 'description' } }) }
        .to raise_error(Jekyll::PageMeta::Error, /non-empty list/)
    end
  end

  describe 'the canonical' do
    it 'is absolute and drops index.html' do
      expect(meta(build_site, 'index.html')['canonical']).to eq('https://example.com/')
    end

    it 'takes the front matter value over the derived url' do
      expect(meta(build_site, 'canonical.html')['canonical']).to eq('https://example.com/elsewhere/')
    end

    it 'is absent on a page with no url' do
      expect(meta(build_site, 'urlless.html')).not_to have_key('canonical')
    end
  end

  describe 'the image' do
    it 'reads og_image off the page' do
      expect(meta(build_site, 'image.html')['image']).to eq('https://example.com/media/page.png')
    end

    it 'falls back to the site image' do
      site = build_site('og_image' => '/media/card.png')

      expect(meta(site, 'about.md')['image']).to eq('https://example.com/media/card.png')
    end

    it 'is absent when nothing sets one' do
      expect(meta(build_site, 'about.md')).not_to have_key('image')
    end
  end
end
