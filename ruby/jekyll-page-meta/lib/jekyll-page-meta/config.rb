# frozen_string_literal: true

module Jekyll
  module PageMeta
    class Config
      SEPARATOR = ' - '
      FROM = %w[description excerpt site].freeze
      TRUNCATE = 160
      CARD = 'summary'

      attr_reader :title_separator, :from, :truncate, :twitter_card

      def self.from_site(site)
        raw = site.config['page_meta']
        new(raw.is_a?(Hash) ? raw : {})
      end

      def initialize(raw)
        @title_separator = raw['title_separator'].nil? ? SEPARATOR : raw['title_separator'].to_s
        description = raw['description'].is_a?(Hash) ? raw['description'] : {}
        @from = build_from(description['from'])
        @truncate = build_truncate(description['truncate'])
        @twitter_card = raw['twitter_card'].nil? ? CARD : raw['twitter_card'].to_s
        @twitter_site = raw['twitter_site'].to_s.strip
      end

      def twitter_site?
        !@twitter_site.empty?
      end

      # The card wants a handle, and a site writing the bare name is the common mistake.
      def twitter_site
        @twitter_site.start_with?('@') ? @twitter_site : "@#{@twitter_site}"
      end

      private

      def build_from(from)
        return FROM if from.nil?
        raise Error, 'page_meta description from must be a non-empty list' unless from.is_a?(Array) && !from.empty?

        from.map(&:to_s)
      end

      def build_truncate(value)
        return TRUNCATE if value.nil?
        return 0 if value == false
        unless value.is_a?(Integer)
          raise Error,
                "page_meta description truncate must be a whole number: #{value.inspect}"
        end
        raise Error, "page_meta description truncate cannot be negative: #{value.inspect}" if value.negative?

        value
      end
    end
  end
end
