# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    MODES = %w[opt-in opt-out].freeze

    class Config
      attr_reader :path, :index, :mode, :files

      def self.from(site)
        raw = site.config['sitemaps']
        return nil unless raw.is_a?(Hash)

        new(raw)
      end

      def initialize(raw)
        @path = raw['path'].nil? ? '/' : raw['path'].to_s
        @index = raw['index'].nil? ? nil : raw['index'].to_s
        @mode = raw['mode'].nil? ? 'opt-in' : raw['mode'].to_s
        raise Error, "sitemaps mode must be one of #{MODES.join(', ')}: #{@mode.inspect}" unless MODES.include?(@mode)

        @files = build_files(raw['files'])
        @raw = raw
      end

      # No files key at all is a site that only uses the tags.
      def build_files(files)
        return [] if files.nil?
        raise Error, 'sitemaps files must be a non-empty list' unless files.is_a?(Array) && !files.empty?

        files.map { |file| FileConfig.new(file, @path) }
      end

      def index_dir
        File.dirname(@index)
      end

      def index_name
        File.basename(@index)
      end

      def index_path
        Urls.leading_slash(@index)
      end

      # Liquid reads this back as site.sitemaps.urls.
      def publish_urls(urls)
        @raw['urls'] = urls
      end
    end

    class FileConfig
      attr_reader :name, :include, :where

      def initialize(raw, base_path)
        raise Error, "each entry in sitemaps files needs a name: #{raw.inspect}" unless raw.is_a?(Hash) && raw['name']

        @name = raw['name'].to_s
        @include = Array(raw['include']).map(&:to_s)
        raise Error, "sitemaps file #{@name} needs an include list" if @include.empty?

        @where = raw['where'].is_a?(Hash) ? raw['where'] : {}
        @base_path = base_path
      end

      def filename
        @name.end_with?('.xml') ? @name : "#{@name}.xml"
      end

      def dir
        Urls.leading_slash(@base_path)
      end

      def path
        File.join(dir, filename)
      end
    end
  end
end
