# frozen_string_literal: true

require 'cgi'

module Jekyll
  module ResourceTags
    module Element
      def self.write(name, attributes, closing:)
        written = attributes.filter_map { |key, value| attribute(key, value) }
        opening = ["<#{name}", *written].join(' ') + '>'
        closing ? "#{opening}</#{name}>" : opening
      end

      def self.attribute(key, value)
        return if value.nil? || value == false
        return key.to_s if value == true

        text = value.to_s
        return if text.empty?

        %(#{key}="#{CGI.escapeHTML(text)}")
      end
    end
  end
end
