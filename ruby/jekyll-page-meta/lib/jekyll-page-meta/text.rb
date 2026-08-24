# frozen_string_literal: true

require 'cgi'

module Jekyll
  module PageMeta
    # One pipeline for every source: markdown, then tags out, then one line, then cut.
    module Text
      # A generator sees content before Liquid has run, so the tags are still in the text.
      LIQUID = /\{%.*?%\}|\{\{.*?\}\}/m
      BLOCK_END = %r{</(?:p|h[1-6]|li|div|blockquote|pre|tr|td|th)>|<br\s*/?>}i
      TAG = %r{</?[^>]*>}

      def self.description(site, input, limit)
        text = input.to_s
        return nil if text.strip.empty?

        text = markdown(site, text.gsub(LIQUID, ' '))
        text = CGI.unescapeHTML(text.gsub(BLOCK_END, ' ').gsub(TAG, '')).gsub(/\s+/, ' ').strip
        return nil if text.empty?

        limit.positive? ? text[0...limit] : text
      end

      def self.markdown(site, text)
        site.find_converter_instance(Jekyll::Converters::Markdown).convert(text)
      end
    end
  end
end
