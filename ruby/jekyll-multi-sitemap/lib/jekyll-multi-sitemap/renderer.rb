# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    module Renderer
      DECLARATION = '<?xml version="1.0" encoding="utf-8"?>'
      NAMESPACE = 'http://www.sitemaps.org/schemas/sitemap/0.9'

      def self.urlset(site, docs)
        entries = docs.map { |doc| url(site, doc) }.reject(&:empty?)
        [
          DECLARATION,
          %(<urlset xmlns="#{NAMESPACE}">),
          *entries,
          '</urlset>',
          ''
        ].join("\n")
      end

      def self.index(site, urls)
        entries = urls.map { |url| "  <sitemap>\n    <loc>#{escape(url)}</loc>\n  </sitemap>" }
        [
          DECLARATION,
          %(<sitemapindex xmlns="#{NAMESPACE}">),
          *entries,
          '</sitemapindex>',
          ''
        ].join("\n")
      end

      def self.url(site, doc, indent: '  ')
        loc = location(site, doc)
        if loc.nil? || loc.empty?
          Jekyll.logger.warn 'Sitemap:', "skipped an entry with no url: #{doc.inspect}"
          return ''
        end

        meta = Doc.sitemap(doc)
        fields = {
          'loc' => loc,
          'lastmod' => Urls.xmlschema(meta['lastmod'], site),
          'changefreq' => meta['changefreq'],
          'priority' => meta['priority']
        }

        lines = fields.filter_map do |name, value|
          next if value.nil? || value.to_s.empty?

          "#{indent}  <#{name}>#{escape(value.to_s)}</#{name}>"
        end
        ["#{indent}<url>", *lines, "#{indent}</url>"].join("\n")
      end

      def self.location(site, doc)
        url = Doc.url(doc)
        return nil if url.nil?

        Urls.absolute(site, url.to_s.gsub('index.html', ''))
      end

      def self.escape(value)
        value.gsub('&', '&amp;').gsub('<', '&lt;').gsub('>', '&gt;').gsub('"', '&quot;').gsub("'", '&apos;')
      end
    end
  end
end
