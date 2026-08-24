# frozen_string_literal: true

module Jekyll
  module GTM
    class Tag < Liquid::Tag
      # The shape of a container id. A bare argument only counts as one if it matches; anything else is a
      # variable name, and a variable that resolves to nothing renders nothing.
      ID = /\AGTM-[A-Z0-9]+\z/

      def initialize(tag_name, markup, tokens)
        super

        @markup = markup.strip
      end

      def render(context)
        id = resolve(context)
        return '' if id.empty?

        snippet(id)
      end

      private

      def resolve(context)
        value = context[@markup]
        value = @markup if value.nil? && @markup.match?(ID)

        value.to_s.strip
      end
    end
  end
end
