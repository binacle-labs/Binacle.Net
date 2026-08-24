# frozen_string_literal: true

module Jekyll
  module Webmanifest
    class LinkTag < Liquid::Tag
      def render(context)
        raw = context.registers[:site].config['webmanifest']
        url = raw.is_a?(Hash) ? raw['url'].to_s : ''
        return '' if url.empty?

        %(<link rel="manifest" href="#{url}">)
      end
    end
  end
end
