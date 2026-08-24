# frozen_string_literal: true

require 'addressable/uri'

module Jekyll
  module StructuredData
    # Jekyll's url filters need a Liquid context, and a crawler resolves a relative url against the page.
    module Urls
      def self.absolute(site, input)
        return nil if input.nil?

        input = input.to_s
        return input if Addressable::URI.parse(input).absolute?

        site_url = site.config['url'].to_s
        path = relative(site, input)
        return path if site_url.empty?

        Addressable::URI.parse(site_url + path).normalize.to_s
      end

      def self.relative(site, input)
        parts = [site.config['baseurl'].to_s.chomp('/'), input.to_s]
        Addressable::URI.parse(parts.map { |part| leading_slash(part) }.join).normalize.to_s
      end

      def self.leading_slash(input)
        return input if input.empty? || input.start_with?('/')

        "/#{input}"
      end

      def self.join(base, input)
        return input unless input.is_a?(String)
        return input if base.to_s.empty?

        uri = Addressable::URI.parse(input)
        return input if uri.absolute?

        Addressable::URI.parse(base.to_s).join(uri).normalize.to_s
      end
    end
  end
end
