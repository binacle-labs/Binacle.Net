# frozen_string_literal: true

module Jekyll
  module ResourceTags
    class ResourceTag < Liquid::Tag
      include Jekyll::Filters::URLFilters

      # A scheme, or the protocol-relative form, means the path is already whole.
      WHOLE_PATH = %r{\A(?:[a-zA-Z][a-zA-Z0-9+.\-]*:|//)}

      SKIP = [].freeze

      def initialize(tag_name, markup, tokens)
        super
        @markup = markup.strip
      end

      def render(context)
        # relative_url reads the site off @context, which Liquid does not set.
        @context = context

        items = @markup.empty? ? nil : context[@markup]
        return '' unless items.is_a?(Array)

        items.filter_map { |item| element(item) }.join("\n")
      end

      private

      def element(item)
        return unless item.is_a?(Hash)
        return if no_path?(item[self.class::PATH_KEY])

        Element.write(self.class::NAME, attributes(item), closing: self.class::CLOSING)
      end

      def attributes(item)
        pairs = self.class::DEFAULTS.dup
        item.each do |key, value|
          next if self.class::SKIP.include?(key)

          if key == self.class::PATH_KEY
            pairs[self.class::PATH_ATTRIBUTE] = path(value)
          else
            pairs[key] = value
          end
        end
        pairs
      end

      def path(value)
        text = value.to_s
        text.match?(WHOLE_PATH) ? text : relative_url(text)
      end

      def no_path?(value)
        value.nil? || value == false || value.to_s.strip.empty?
      end
    end
  end
end
