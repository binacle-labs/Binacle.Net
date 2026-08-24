# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    class UrlsetTag < Liquid::Tag
      def initialize(tag_name, markup, tokens)
        super
        @markup = markup.strip
      end

      def render(context)
        site = context.registers[:site]
        documents = Tags.lookup(context, @markup, 'sitemap_urlset')
        mode = (site.config['sitemaps'] || {})['mode'] || 'opt-in'
        Renderer.urlset(site, documents.select { |document| Selection.listed?(document, mode) })
      end
    end
  end
end
