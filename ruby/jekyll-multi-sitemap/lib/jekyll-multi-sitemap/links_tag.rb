# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    class LinksTag < Liquid::Tag
      def render(context)
        urls = (context.registers[:site].config['sitemaps'] || {})['urls'] || []
        urls.map { |url| "Sitemap: #{url}" }.join("\n")
      end
    end
  end
end
