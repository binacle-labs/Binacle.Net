# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    module Labels
      # Three operations, and the order is load-bearing: capitalize touches only the first letter, so a
      # segment humanized before the hyphens go is "Getting-started" and every label on the site shifts.
      def self.humanize(segment)
        segment.to_s.gsub('-', ' ').gsub('.html', '').split.map(&:capitalize).join(' ')
      end

      def self.first_of(data, keys)
        keys.map { |key| data[key].to_s }.find { |value| !value.strip.empty? }
      end
    end
  end
end
