# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    class UrlTag < Liquid::Tag
      def initialize(tag_name, markup, tokens)
        super
        @markup = markup.strip
      end

      def render(context)
        document = Tags.lookup(context, @markup, 'sitemap_url')
        Renderer.url(context.registers[:site], document)
      end
    end
  end
end
