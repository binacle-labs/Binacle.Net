# frozen_string_literal: true

module Jekyll
  module PageMeta
    # The four keys every page publishes. Another gem reads these rather than resolving its own.
    class Resolver
      # Neither is front matter, so neither is in data.
      READABLE = %w[excerpt content].freeze

      def initialize(site, config)
        @site = site
        @config = config
      end

      def write(doc)
        resolved = {
          'title' => title(doc),
          'description' => description(doc),
          'canonical' => canonical(doc),
          'image' => image(doc)
        }
        doc.data['meta'] = resolved.reject { |_key, value| value.nil? || value.to_s.empty? }
      end

      private

      def title(doc)
        seo_title = doc.data['seo_title'].to_s
        return seo_title unless seo_title.empty?

        site_title = @site.config['title'].to_s
        page_title = doc.data['title'].to_s
        return site_title if page_title.empty?

        suffix = doc.data['title_suffix'].to_s
        page_title = "#{page_title} #{suffix}" unless suffix.empty?
        return page_title if site_title.empty?

        "#{page_title}#{@config.title_separator}#{site_title}"
      end

      def description(doc)
        @config.from.each do |key|
          text = Text.description(@site, source(doc, key), @config.truncate)
          return text unless text.nil?
        end
        nil
      end

      def source(doc, key)
        return @site.config['description'] if key == 'site'

        found = doc.data[key]
        return found unless found.nil?

        doc.public_send(key) if READABLE.include?(key) && doc.respond_to?(key)
      end

      def canonical(doc)
        override = doc.data['canonical'].to_s
        return Urls.absolute(@site, override) unless override.empty?

        url = doc.url.to_s
        # An empty href self-references the wrong page, which is worse than no canonical at all.
        return nil if url.empty?

        Urls.absolute(@site, url.sub(/index\.html\z/, ''))
      end

      def image(doc)
        found = doc.data['og_image'] || @site.config['og_image']
        return nil if found.nil? || found.to_s.empty?

        Urls.absolute(@site, found)
      end
    end
  end
end
