# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    module Tags
      # Liquid hands a tag its markup as text, so a filter in it is never run - it just never resolves.
      def self.lookup(context, markup, tag_name)
        if markup.empty? || markup.include?('|')
          raise Error, "{% #{tag_name} %} takes a variable name, not an expression: #{markup.inspect}"
        end

        found = context[markup]
        raise Error, "{% #{tag_name} %} found nothing named #{markup.inspect}" if found.nil?

        found
      end
    end
  end
end
