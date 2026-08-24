# frozen_string_literal: true

module Jekyll
  module Webmanifest
    class Config
      DEFAULT_PATH = '/site.webmanifest'
      # `url` is written back into this same block after the generator runs, so it is not a manifest member.
      RESERVED = %w[path url].freeze

      attr_reader :path

      def self.from(site)
        raw = site.config['webmanifest']
        return nil unless raw.is_a?(Hash)

        new(raw)
      end

      def initialize(raw)
        @raw = raw
        @path = Urls.leading_slash(raw['path'].nil? ? DEFAULT_PATH : raw['path'].to_s)
        raise Error, "webmanifest path must name a file: #{@path.inspect}" if @path.end_with?('/')
      end

      def [](key)
        @raw[key]
      end

      # Anything the gem does not know is a manifest member it has never heard of, and goes out untouched.
      def extras
        @raw.reject { |key, _| RESERVED.include?(key) || Manifest::KNOWN.include?(key) }
      end

      def dir
        File.dirname(@path)
      end

      def filename
        File.basename(@path)
      end

      # Liquid reads this back as site.webmanifest.url.
      def publish_url(url)
        @raw['url'] = url
      end
    end
  end
end
