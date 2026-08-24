# frozen_string_literal: true

module Jekyll
  module SiteFilters
    module Dates
      def expand_year(input, placeholder = '{now}')
        input.to_s.gsub(placeholder, build_year.to_s)
      end

      private

      # site.time, not Time.now, so every page of one build carries the same year.
      def build_year
        site = @context.registers[:site] if defined?(@context) && @context

        (site&.time || Time.now).year
      end
    end
  end
end
