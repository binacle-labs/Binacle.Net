# frozen_string_literal: true

require 'cgi'

module Jekyll
  module PageMeta
    module Elements
      def self.meta(kind, name, content)
        return nil if blank?(content)

        %(<meta #{kind}="#{name}" content="#{escape(content)}">)
      end

      def self.title(text)
        return nil if blank?(text)

        "<title>#{escape(text)}</title>"
      end

      def self.link(rel, href)
        return nil if blank?(href)

        %(<link rel="#{rel}" href="#{escape(href)}">)
      end

      def self.blank?(value)
        value.nil? || value.to_s.empty?
      end

      def self.escape(value)
        CGI.escapeHTML(value.to_s)
      end
    end
  end
end
