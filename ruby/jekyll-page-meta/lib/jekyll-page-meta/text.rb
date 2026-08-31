# frozen_string_literal: true

require 'cgi'

module Jekyll
  module PageMeta
    # One pipeline for every source: markdown, then tags out, then one line, then cut.
    module Text
      # A generator sees content before Liquid has run, so the tags are still in the text.
      # CodeQL calls this polynomial. Ruby memoizes the backtrack from 3.2 and the gemspec floor is 3.4,
      # so it is linear here. I measured a bounded {0,4096} version 600x slower. Leave it.
      LIQUID = /\{%.*?%\}|\{\{.*?\}\}/m
      BLOCK_END = %r{</(?:p|h[1-6]|li|div|blockquote|pre|tr|td|th)>|<br\s*/?>}i
      # [^<>] rather than [^>]: '<a <b>' is one tag to [^>]* and two to a browser. Also measured 2x
      # faster on angle-bracket-heavy input, and it is what CodeQL wants.
      TAG = %r{</?[^<>]*>}

      def self.description(site, input, limit)
        text = input.to_s
        return nil if text.strip.empty?

        text = markdown(site, text.gsub(LIQUID, ' '))
        text = CGI.unescapeHTML(text.gsub(BLOCK_END, ' ').gsub(TAG, ''))
        # After the unescape, so an escaped &lt;script&gt; cannot come back as a tag. One pass of gsub
        # leaves '<script' behind on '<<script>script>', and unescaping is a second way in.
        text = text.delete('<>').gsub(/\s+/, ' ').strip
        return nil if text.empty?

        limit.positive? ? text[0...limit] : text
      end

      def self.markdown(site, text)
        site.find_converter_instance(Jekyll::Converters::Markdown).convert(text)
      end
    end
  end
end
