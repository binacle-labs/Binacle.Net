# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    # The generator sees Jekyll pages and documents; a tag sees the Liquid drops of the same things.
    module Doc
      def self.url(doc)
        return doc.url if doc.respond_to?(:url)
        field(doc, 'url')
      end

      def self.field(doc, key)
        data = doc.data if doc.respond_to?(:data)
        return data[key] if data.is_a?(Hash) && data.key?(key)
        return doc[key] if doc.respond_to?(:[])

        nil
      end

      def self.sitemap(doc)
        value = field(doc, 'sitemap')
        value.is_a?(Hash) ? value : {}
      end
    end
  end
end
