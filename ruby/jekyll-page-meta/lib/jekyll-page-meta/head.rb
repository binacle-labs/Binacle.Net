# frozen_string_literal: true

module Jekyll
  module PageMeta
    # Writes what the generator resolved. Nothing here works out a value of its own.
    class Head
      def initialize(site, config, page)
        @site = site
        @config = config
        @page = page
        meta = page['meta']
        @meta = meta.is_a?(Hash) ? meta : {}
      end

      def to_s
        elements.compact.join("\n")
      end

      private

      def elements
        [
          Elements.title(title),
          Elements.meta('name', 'description', description),
          Elements.link('canonical', canonical),
          Elements.meta('name', 'robots', @page['robots']),
          *open_graph,
          *twitter
        ]
      end

      def open_graph
        [
          Elements.meta('property', 'og:type', 'website'),
          Elements.meta('property', 'og:site_name', @site.config['title']),
          Elements.meta('property', 'og:title', title),
          Elements.meta('property', 'og:description', description),
          Elements.meta('property', 'og:url', canonical),
          Elements.meta('property', 'og:image', image),
          Elements.meta('property', 'og:locale', locale)
        ]
      end

      def twitter
        [
          Elements.meta('name', 'twitter:card', @config.twitter_card),
          @config.twitter_site? ? Elements.meta('name', 'twitter:site', @config.twitter_site) : nil,
          Elements.meta('name', 'twitter:title', title),
          Elements.meta('name', 'twitter:description', description),
          Elements.meta('name', 'twitter:url', canonical),
          Elements.meta('name', 'twitter:image', image)
        ]
      end

      def title
        @meta['title']
      end

      def description
        @meta['description']
      end

      def canonical
        @meta['canonical']
      end

      def image
        @meta['image']
      end

      def locale
        @page['locale'] || @site.config['locale']
      end
    end
  end
end
