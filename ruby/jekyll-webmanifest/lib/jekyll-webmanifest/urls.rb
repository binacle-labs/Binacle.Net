# frozen_string_literal: true

require 'addressable/uri'

module Jekyll
  module Webmanifest
    # Jekyll's url filters need a Liquid context, which a generator never has.
    module Urls
      def self.relative(site, input)
        return nil if input.nil?

        input = input.to_s
        return input if Addressable::URI.parse(input).absolute?

        parts = [site.config['baseurl'].to_s.chomp('/'), input]
        Addressable::URI.parse(parts.map { |part| leading_slash(part) }.join).normalize.to_s
      end

      def self.leading_slash(input)
        return input if input.empty? || input.start_with?('/')

        "/#{input}"
      end
    end
  end
end
