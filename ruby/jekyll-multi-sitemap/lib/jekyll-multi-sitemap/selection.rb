# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    module Selection
      def self.documents(site, file_config, mode)
        docs = file_config.include.flat_map { |name| source(site, name) }
        docs = docs.select { |doc| matches?(site, doc, file_config.where) }
        docs.select { |doc| listed?(doc, mode) }
      end

      def self.source(site, name)
        return site.pages if name == 'pages'

        collection = site.collections[name]
        raise Error, "sitemaps include names no collection: #{name.inspect}" if collection.nil?

        collection.docs
      end

      def self.listed?(doc, mode)
        exclude = Doc.sitemap(doc)['exclude']
        return exclude != true if mode == 'opt-out'

        exclude == false
      end

      def self.matches?(site, doc, where)
        where.all? do |key, raw|
          wanted = value(site, raw)
          found = Doc.field(doc, key.to_s)
          found == wanted || found.to_s == wanted.to_s
        end
      end

      def self.value(site, raw)
        return raw unless raw.is_a?(String) && raw.start_with?('$')

        path = raw[1..].split('.')
        path.shift if path.first == 'site'
        raise Error, "a $ value must be a path into site data: #{raw.inspect}" unless path.shift == 'data'

        path.reduce(site.data) do |found, key|
          raise Error, "#{raw} is not set in site data" unless found.is_a?(Hash) && found.key?(key)

          found[key]
        end
      end
    end
  end
end
