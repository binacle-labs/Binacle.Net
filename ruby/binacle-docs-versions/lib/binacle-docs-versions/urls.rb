# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Where a versioned file renders. The folder name never appears in a url: a version renders under
    # /version/<url_segment>/, and the segment is what versions.yml lists beside the folder's id - or the id.
    class Urls
      INDEX = 'index'

      def initialize(site)
        @segments = segments_of(site)
      end

      def for(item)
        rest = item.relative_path.to_s.sub(VersionGenerator::FOLDER, '')
        "#{prefix(item.data['version'].to_s)}/#{item.is_a?(Jekyll::StaticFile) ? rest : page_path(rest)}"
      end

      private

      def prefix(version)
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
