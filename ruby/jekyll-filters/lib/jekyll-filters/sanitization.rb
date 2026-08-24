# frozen_string_literal: true

module Jekyll
  module SiteFilters
    module Sanitization
      TAG = %r{</?[^>]*>}

      def clean_content(input, length = 160)
        input.to_s
             .gsub(TAG, '')
             .gsub(/\n+/, ' ')
             .gsub(/ {2,}/, ' ')
             .strip[0...length]
             .strip
      end
    end
  end
end
