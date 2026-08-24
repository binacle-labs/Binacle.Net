# frozen_string_literal: true

module Jekyll
  module Webmanifest
    class Manifest
      KNOWN = %w[name description start_url display background_color theme_color icons].freeze
      DEFAULT_ICONS = [{ 'src' => '/android-chrome-192x192.png', 'sizes' => '192x192', 'type' => 'image/png' }].freeze

      def initialize(site, config)
        @site = site
        @config = config
      end

      def to_h
        members = {
          'name' => text(@config['name'] || @site.config['title']),
          'description' => text(@config['description'] || @site.config['description']),
          'start_url' => Urls.relative(@site, @config['start_url'] || '/'),
          'display' => text(@config['display'] || 'standalone'),
          'background_color' => text(@config['background_color']),
          'theme_color' => text(@config['theme_color']),
          'icons' => icons
        }.compact

        @config.extras.each { |key, value| members[key] = value }
        members
      end

      private

      def icons
        raw = @config['icons']
        return DEFAULT_ICONS.map { |icon| with_src(icon) } if raw.nil?
        raise Error, 'webmanifest icons must be a list of maps' unless list_of_maps?(raw)
        return nil if raw.empty?

        raw.map { |icon| with_src(icon) }
      end

      def list_of_maps?(raw)
        raw.is_a?(Array) && raw.all?(Hash)
      end

      # merge keeps the order the config declared, so an icon comes out reading the way it was written.
      def with_src(icon)
        return icon unless icon.key?('src')

        icon.merge('src' => Urls.relative(@site, icon['src']))
      end

      def text(value)
        return nil if value.nil?

        stripped = value.to_s.strip
        stripped.empty? ? nil : stripped
      end
    end
  end
end
