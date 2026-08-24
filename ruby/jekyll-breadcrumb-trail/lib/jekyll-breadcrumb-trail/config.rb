# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    class Config
      EXCLUDE = [].freeze
      TITLE_FROM = %w[crumbtitle title].freeze
      LABEL = 'breadcrumb'
      HOME_TITLE = 'Home'

      attr_reader :exclude, :title_from, :label, :extra_class, :home_title, :home_html

      def self.from_site(site)
        raw = site.config['breadcrumbs']
        new(raw.is_a?(Hash) ? raw : {})
      end

      def initialize(raw)
        @exclude = build_exclude(raw['exclude'])
        @title_from = build_title_from(raw['title_from'])
        @link_last = raw['link_last'] == true
        @label = raw['label'].nil? ? LABEL : raw['label'].to_s
        @extra_class = raw['class'].to_s
        home = raw['home'].is_a?(Hash) ? raw['home'] : {}
        @home_title = home['title'].nil? ? HOME_TITLE : home['title'].to_s
        @home_html = home['html'].to_s
      end

      def link_last?
        @link_last
      end

      def home_html?
        !@home_html.empty?
      end

      def extra_class?
        !@extra_class.empty?
      end

      def excluded?(segment)
        @exclude.any? { |pattern| File.fnmatch?(pattern, segment) }
      end

      private

      def build_exclude(exclude)
        return EXCLUDE if exclude.nil?
        raise Error, "breadcrumbs exclude must be a list of patterns: #{exclude.inspect}" unless exclude.is_a?(Array)

        exclude.map(&:to_s)
      end

      def build_title_from(from)
        return TITLE_FROM if from.nil?
        raise Error, 'breadcrumbs title_from must be a non-empty list' unless from.is_a?(Array) && !from.empty?

        from.map(&:to_s)
      end
    end
  end
end
