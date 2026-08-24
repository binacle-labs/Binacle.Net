# frozen_string_literal: true

module Jekyll
  module SiteFilters
    module Capitalization
      def capitalize_all(input)
        input.to_s.split.map(&:capitalize).join(' ')
      end
    end
  end
end
