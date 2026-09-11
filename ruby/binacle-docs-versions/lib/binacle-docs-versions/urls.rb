# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Where a versioned file renders. The folder name never appears in a url: the current version renders at
    # the site root, every other one under /version/<url_segment>/, the segment being what versions.yml lists
    # beside the folder's id - or the id.
    class Urls
      INDEX = 'index'

      def initialize(site)
        @segments = segments_of(site)
        @current = site.data.dig('versions', 'current').to_s
      end

      def for(item)
        rest = item.relative_path.to_s.sub(VersionGenerator::FOLDER, '')
        "#{prefix(item.data['version'].to_s)}/#{item.is_a?(Jekyll::StaticFile) ? rest : page_path(rest)}"
      end

      private

      def prefix(version)
        return '' if version == @current

        "/version/#{@segments.fetch(version, version)}"
      end

      # A page renders as a folder with an index inside, so its url ends in a slash. The folder's own index
      # page is the folder.
      def page_path(rest)
        without_ext = rest.sub(/\.\w+\z/, '')
        segments = without_ext.split('/')
        segments.pop if segments.last == INDEX
        segments.empty? ? '' : "#{segments.join('/')}/"
      end

      def segments_of(site)
        list = site.data.dig('versions', 'list')
        return {} unless list.is_a?(Array)

        list.each_with_object({}) do |entry, segments|
          next unless entry.is_a?(Hash) && entry['id'] && entry['url_segment']

          segments[entry['id'].to_s] = entry['url_segment'].to_s
        end
      end
    end
  end
end
